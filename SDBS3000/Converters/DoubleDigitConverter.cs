using SDBS3000.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SDBS3000.Converters
{
    public class DoubleDigitConverter : IValueConverter
    {
        private static string[] Fmts = new[]
        {
            "f0","f1","f2","f3"
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0f.ToString(Fmts[AppSetting.Default.Digits]);
            return ((double)value).ToString(Fmts[AppSetting.Default.Digits]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
