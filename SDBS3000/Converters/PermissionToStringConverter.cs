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
    public class PermissionToStringConverter : IValueConverter
    {
        private static readonly Dictionary<int, string> Permissions = new Dictionary<int, string>
        {
            {0, "管理员".Localize() },
            {1, "调试员".Localize() },
            {2, "操作员".Localize() }
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Permissions[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
