using ArchivatorDb.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Archivator_desktop_WPF_WTS.Converters
{
    [ValueConversion(typeof(List<Event2Tag>), typeof(string))]
    class TagListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return string.Join(";", ((List<Event2Tag>)value ?? throw new InvalidOperationException()).Select(i => i.Tag.Name).ToArray());
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
