using System.Globalization;
using System.Windows.Data;

namespace SDBS3000.Converters
{
    public class TextToNullableFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return string.Empty;
            }
            else if (parameter is string fmt && !string.IsNullOrWhiteSpace(fmt))
            {
                return ((float)value).ToString(fmt);
            }
            else
            {
                return ((float)value).ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && float.TryParse(str, out var floatValue))
            {
                return floatValue;
            }
            else
            {
                return null;
            }
        }

    }
}
