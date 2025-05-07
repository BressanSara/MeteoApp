using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace MeteoApp.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long unixTime)
            {
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                return dateTime.ToString("dddd, MMM dd", CultureInfo.CurrentCulture);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WeatherIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string weatherMain)
            {
                return weatherMain.ToLower() + ".png";
            }
            return "clear.png"; // Default icon
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 