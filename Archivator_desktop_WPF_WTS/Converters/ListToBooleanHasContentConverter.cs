using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Archivator_desktop_WPF_WTS.Converters
{
    [ValueConversion(typeof(IEnumerable<>), typeof(bool))]
    internal class ListToBooleanHasContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<object> list)
            {
                return list.Any();
            }

            throw new Exception("ERROR: Only type of IEnumerable is allowed in this converter!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
