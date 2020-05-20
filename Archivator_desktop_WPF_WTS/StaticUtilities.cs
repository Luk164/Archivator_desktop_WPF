using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Archivator_desktop_WPF_WTS.Converters;
using ArchivatorDb.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Archivator_desktop_WPF_WTS
{
    /// <summary>
    /// Holds static functions for the code to increase re-usability
    /// </summary>

    //Can be made private check suppressed, all static functions are to be accessible to all parts of code
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class StaticUtilities
    {
        public const double MAX_FILE_SIZE = 2.5e+7;

        public const string DEFAULT_CONNECTION_STRING =
            @"Server=(localdb)\\mssqllocaldb;Database=Archivator_autoMigTest;Trusted_Connection=True;";

        public const string CONN_STRING_KEY = "DbConnString";

        public const string FILE_FILTER_STRING =
            "All files (*.*)|*.*|" +
            "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
            "Text Files(*.txt)|*.txt|" +
            "Office documents(*.docx;*.pptx,*.xlsx)|*.docx;*.pptx,*.xlsx|" +
            "Office 2013 documents(*.doc;*.ppt,*.xls)|*.doc;*.ppt,*.xls|" +
            "PDF Documents(*.pdf)|*.pdf";

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Tests connectivity of DbContext object.
        /// </summary>
        /// <param name="context">DbContext object to be tested</param>
        /// <returns>True if connectible, false if there is a problem</returns>
        public static bool TestConnection(this DbContext context)
        {
            DbConnection conn = context.Database.GetDbConnection();

            try
            {
                conn.Open(); // Check the database connection

                return true;
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);

                return false;
            }
        }

        /// <summary>
        /// Prepares options builder with all the required settings
        /// </summary>
        /// <param name="builder">Builder for settings to be applied to</param>
        /// <param name="configuration">Application configuration object</param>
        public static void SetupDatabase(DbContextOptionsBuilder builder, IConfiguration configuration)
        {
            try
            {
                builder.UseLazyLoadingProxies();
                builder.UseSqlServer(configuration.GetSection(StaticUtilities.CONN_STRING_KEY).Value,
                    optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(App).Namespace));
            }
            catch (Exception e)
            {
                MessageBox.Show("ERROR: Unknown error has occured: " + e.Message, "Unknown error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates an image object from byte[] data
        /// </summary>
        /// <param name="imageData">Byte[] used for image object creation</param>
        /// <returns></returns>
        public static BitmapImage LoadImage(this byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }

        /// <summary>
        /// Synchronizes tags of Event with tags selected by user in GUI
        /// </summary>
        /// <param name="Event">Event to be synced</param>
        /// <param name="listOfSelectedTags">List of tags that are supposed to be in Event</param>
        public static void SyncEventWithTags(EventEntity Event, List<Tag> listOfSelectedTags)
        {
            //add all required tags that are not already present
            foreach (var tag in listOfSelectedTags.Where(tag => Event.Tags.All(event2Tag => event2Tag.Tag != tag)))
            {
                Event.Tags.Add(new Event2Tag {Event = Event, Tag = tag});
            }

            //remove all tags that are not supposed to be there
            foreach (var event2Tag in Event.Tags.ToList()
                .Where(event2Tag => !listOfSelectedTags.Contains(event2Tag.Tag)))
            {
                Event.Tags.Remove(event2Tag);
            }
        }

        /// <summary>
        /// Generates object of type FlowDocument which is then used for ticket printing.
        /// </summary>
        /// <param name="identifier">Id of object to be printed (for item entity alternate key)</param>
        /// <param name="name">Name of object to be printed</param>
        /// <param name="qrCode">QR code to be printed on the ticket (can be any image)</param>
        /// <param name="dialog">Dialog with data regarding printing information</param>
        /// <returns>Flow document with all relevant values ready for printing</returns>
        public static FlowDocument GenerateFlowDocument(string identifier, string name, byte[] qrCode,
            PrintDialog dialog)
        {
            var flowDoc = new FlowDocument
            {
                PageWidth = dialog.PrintableAreaWidth,
                PageHeight = dialog.PrintableAreaHeight + 100,
                PagePadding = new Thickness(15, 20, 0, 0)
            };
            flowDoc.Blocks.Add(new Paragraph(new Run(identifier + "\n" + name))
            {
                FontSize = 19
            });
            flowDoc.Blocks.Add(new BlockUIContainer(new Image()
            {
                Source = qrCode.LoadImage(),
                Height = 160,
                Width = 160,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(-10, -20, 0, 0),
                ClipToBounds = true
            }));

            return flowDoc;
        }


        /// <summary>
        /// Prints passed object. Only Item and FileEntity is allowed
        /// </summary>
        /// <param name="objectToPrint">Item or FileEntity to be printed. Throws exception if a different type is passed</param>
        public static void PrintObject(object objectToPrint)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() != true) return;
            IDocumentPaginatorSource idpSource;

            var converter = new DbObjectToQRCodeConverter();
            var image = (byte[]) converter.Convert(objectToPrint, null, null, null);

            switch (objectToPrint)
            {
                case Item item:
                    idpSource = GenerateFlowDocument(
                        item.AlternateKey,
                        item.Name,
                        (byte[]) converter.Convert(objectToPrint, null, null, null), dialog);
                    break;
                case FileEntity fileEntity:
                    idpSource = GenerateFlowDocument(fileEntity.ParentItem.AlternateKey + "/" + fileEntity.Id,
                        fileEntity.FileName, (byte[]) converter.Convert(objectToPrint, null, null, null), dialog);
                    break;
                default:
                    throw new Exception(
                        "Unknown type passed to printObject, did you add another allowed type to DbObjectToQRCodeConverter?");
            }

            dialog.PrintDocument(idpSource.DocumentPaginator, "Archivator print job singular");
        }

        /// <summary>
        /// Print list of objects. List must consist of entirely Items or FileEntities or exception will occur.
        /// </summary>
        /// <param name="objectsToPrint">List of objects, either Item or EventEntity to be printed</param>
        public static void PrintMultipleObjects(List<Item> objectsToPrint)
        {
            if (objectsToPrint.Count == 0) return;

            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() != true) return;

            var converter = new DbObjectToQRCodeConverter();

            foreach (var idpSource in objectsToPrint.Select(i => GenerateFlowDocument(i.AlternateKey, i.Name,
                (byte[])converter.Convert(i, null, null, null), dialog)))
            {
                dialog.PrintDocument(((IDocumentPaginatorSource)idpSource).DocumentPaginator, "Item print job");
            }
        }

        /// <summary>
        /// Print list of objects. List must consist of entirely Items or FileEntities or exception will occur.
        /// </summary>
        /// <param name="objectsToPrint">List of objects, either Item or EventEntity to be printed</param>
        public static void PrintMultipleObjects(List<FileEntity> objectsToPrint)
        {
            if (objectsToPrint.Count == 0) return;

            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() != true) return;

            var converter = new DbObjectToQRCodeConverter();

            foreach (var idpSource in objectsToPrint.Cast<FileEntity>().ToList().Select(f => GenerateFlowDocument(f.ParentItem.AlternateKey, f.FileName,
                (byte[])converter.Convert(f, null, null, null), dialog)))
            {
                dialog.PrintDocument(((IDocumentPaginatorSource)idpSource).DocumentPaginator, "FileEntity print job");
            }
        }
    }
}