using ArchivatorDb.Entities;
using QRCoder;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Archivator_desktop_WPF_WTS.Converters
{
    [ValueConversion(typeof(object), typeof(byte[]))]
    internal class DbObjectToQRCodeConverter : IValueConverter
    {
        public object Convert(object DbObject, Type targetType, object parameter, CultureInfo culture)
        {
            string identifier = DbObject switch
            {
                Item item => item.AlternateKey,
                FileEntity fileEntity => fileEntity.ParentItem.AlternateKey + "/" + fileEntity.Id,
                _ => throw new Exception("Unexpected object type passed to converter!")
            };

            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(identifier, QRCodeGenerator.ECCLevel.L);
                PngByteQRCode pngByteQrCode = new PngByteQRCode(qrCodeData);

                return pngByteQrCode.GetGraphic(20);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
