using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WorkWatcher.Views
{
    public class BoolColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "#FFA0FF8C" : "#FFA7A7A7";
            }
            return "#FFA7A7A7";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.Equals("#FFA0FF8C", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
