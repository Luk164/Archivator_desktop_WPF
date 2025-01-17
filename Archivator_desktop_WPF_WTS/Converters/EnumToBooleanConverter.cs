﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Archivator_desktop_WPF_WTS.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public Type EnumType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string enumString)
            {
                if (Enum.IsDefined(EnumType, value))
                {
                    object enumValue = Enum.Parse(EnumType, enumString);

                    return enumValue.Equals(value);
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(EnumType, enumString);
            }

            return null;
        }
    }
}
