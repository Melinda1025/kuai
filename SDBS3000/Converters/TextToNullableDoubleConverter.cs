using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDBS3000.Converters
{
    public class TextToNullableDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            if (value is null)
            {
                return string.Empty;
            }
            else if (parameter is string fmt && !string.IsNullOrWhiteSpace(fmt))
            {
                return ((double)value).ToString(fmt);
            }
            else
            {
                return ((double)value).ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && double.TryParse(str, out var doubleValue))
            {
                return doubleValue;
            }
            else
            {
                return null;
            }
        }

    }
}
