using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using WindowsDock.Core;

namespace WindowsDock.GUI
{
    public class ValueMinMaxToIsLargeArcConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double value = (double)values[0];
            double minimum = (double)values[1];
            double maximum = (double)values[2];

            // Only return true if the value is 50% of the range or greater
            return ((value * 2) >= (maximum - minimum));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ValueMinMaxToPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double value = (double)values[0];
            double minimum = (double)values[1];
            double maximum = (double)values[2];

            // Convert the value to one between 0 and 360
            double current = (value / (maximum - minimum)) * 360;

            // Adjust the finished state so the ArcSegment gets drawn as a whole circle
            if (current == 360)
                current = 359.999;

            // Shift by 90 degrees so 0 starts at the top of the circle
            current = current - 90;

            // Convert the angle to radians
            current = current * 0.017453292519943295;

            // Calculate the circle's point
            double x = 10 + 10 * Math.Cos(current);
            double y = 10 + 10 * Math.Sin(current);

            return new Point(x, y);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class InverseBoolToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!(bool)value) ? "Show" : "Hide";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
