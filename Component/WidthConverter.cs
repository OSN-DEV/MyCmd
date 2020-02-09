using System;

namespace MyCmd.Component {
    public class WidthConverter : System.Windows.Data.IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int columnsCount = System.Convert.ToInt32(parameter);
            double width = (double)value;
            return width * 0.8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
