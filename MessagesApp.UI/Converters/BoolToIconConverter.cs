using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MessagesApp.UI.Converters
{
    public class BoolToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool hasUnreadMessages)
            {
                if (hasUnreadMessages)
                {
                    return "\xE214";
                }
                else
                {
                    return "\xE216"; 
                }
            }

            return null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}