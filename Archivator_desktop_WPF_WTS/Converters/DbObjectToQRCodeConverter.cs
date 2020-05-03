using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ArchivatorDb.Entities;
using QRCoder;

namespace Archivator_desktop_WPF_WTS.Converters
{
    [ValueConversion(typeof(object), typeof(byte[]))]
    internal class DbObjectToQRCodeConverter : IValueConverter
    {
        public object Convert(object DbObject, Type targetType, object parameter, CultureInfo culture)
        {
            var identifier = DbObject switch
            {
                Item item => "I - " + item.Id,
                FileEntity fileEntity => "F - " + fileEntity.Id,
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
