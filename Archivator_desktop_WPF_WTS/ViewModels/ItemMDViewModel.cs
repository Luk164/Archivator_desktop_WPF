﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Archivator_desktop_WPF_WTS.Contracts.Services;
using Archivator_desktop_WPF_WTS.Contracts.ViewModels;
using Archivator_desktop_WPF_WTS.Converters;
using Archivator_desktop_WPF_WTS.Helpers;
using Archivator_desktop_WPF_WTS.Models;
using ArchivatorDb;
using ArchivatorDb.Entities;
using Microsoft.Win32;

namespace Archivator_desktop_WPF_WTS.ViewModels
{
    public class ItemMDViewModel : Observable, INavigationAware
    {
        private Item _selected;
        private readonly ArchivatorDbContext _context;
        private readonly INavigationService _navigationService;
        private FileEntity _selectedFile;

        public FileEntity SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }

        public Item Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        public IList<Tag> Tags { get; }

        public ObservableCollection<Item> Items { get; private set; }

        public ItemMDViewModel(ArchivatorDbContext context, INavigationService navigationService)
        {
            _context = context;
            Tags = context.Tags.ToList();
            _navigationService = navigationService;
        }

        public void EditSelected()
        {
            _navigationService.NavigateTo(typeof(MainViewModel).FullName, new EditModel(){context = _context, editedObject = Selected});
        }

        public void DeleteSelected()
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to delete this editedObject?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Remove(Selected);
                    Items.Remove(Selected);
                    Selected = Items.FirstOrDefault() ?? new Item();
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void OnNavigatedTo(object parameter) //can be async
        {
            try
            {
                //Items = new ObservableCollection<Item>(_context.Items.AsNoTracking());
                Items = new ObservableCollection<Item>(_context.Items);

                Selected = Items.FirstOrDefault() ?? new Item();
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Message, "Exception Occured", MessageBoxButton.OK, MessageBoxImage.Warning);
                throw;
            }
        }

        public void OnNavigatedFrom()
        {
        }

        public void SaveFile()
        {
            if (_selectedFile == null)
            {
                MessageBox.Show("ERROR: No file selected!", "General error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = StaticUtilities.FILE_FILTER_STRING,
                    DefaultExt = Path.GetExtension(_selectedFile.FileName) ?? throw new InvalidOperationException(),
                    FileName = Path.GetFileNameWithoutExtension(_selectedFile.FileName) ??
                               throw new InvalidOperationException()
                };
                if (saveFileDialog.ShowDialog() == false) //if cancelled
                {
                    return;
                }
                    
                if (saveFileDialog.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                {
                    MessageBox.Show("Invalid character/s detected in fileName!\nInvalid characters: " + Path.GetInvalidFileNameChars());
                }
                else
                {
                    File.WriteAllBytesAsync(saveFileDialog.FileName, _selectedFile.Data);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ERROR: A following exception has occured!: " + e.Message, "Unexpected exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void PrintSelected()
        {
            PrintObject(Selected);
        }

        public void PrintObject(object objectToPrint)
        {
            var converter = new DbObjectToQRCodeConverter();
            var image = (byte[]) converter.Convert(objectToPrint, null, null, null);

            switch (objectToPrint)
            {
                case Item item:
                    UniversalPrint(item.Id, item.Name, 'I', image);
                    break;
                case FileEntity fileEntity:
                    UniversalPrint(fileEntity.Id, fileEntity.FileName, 'F', image);
                    break;
                default:
                    throw new Exception("Unknown type passed to printObject, did you add another allowed type to DbObjectToQRCodeConverter?");
            }
        }

        private void UniversalPrint(int id, string name, char type, byte[] qrCode)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() != true) return;

            var flowDoc = new FlowDocument
            {
                PageWidth = dialog.PrintableAreaWidth,
                PageHeight = dialog.PrintableAreaHeight + 100,
                PagePadding = new Thickness(15, 20, 0, 0)
            };
            flowDoc.Blocks.Add(new Paragraph(new Run(type + " - " + id + "\n" + name))
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

            IDocumentPaginatorSource idpSource = flowDoc;
            dialog.PrintDocument(idpSource.DocumentPaginator, "");
        }
    }
}
