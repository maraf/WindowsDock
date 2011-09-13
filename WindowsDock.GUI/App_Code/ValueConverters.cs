using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using WindowsDock.Core;
using System.Windows.Input;
using DesktopCore;

namespace WindowsDock.GUI
{
    public class InverseBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!(bool)value) ? Resource.Get("Show") : Resource.Get("Hide");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class KeyToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Key k = (Key)value;
            return k != Key.None ? k.ToString() : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class KeyToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Key)value) == Key.None ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class MiliSecsToTimespanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan? span = value as TimeSpan?;
            if(span == null)
                return 0;

            return span.Value.Milliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int num;
            TimeSpan t = new TimeSpan();
            if (Int32.TryParse(value.ToString(), out num))
                return new TimeSpan(0, 0, 0, 0, num);

            return new TimeSpan(0, 0, 0, 0, 0);
        }
    }

    public class IntToMainWindowThicknessValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int num;
            if(Int32.TryParse(value.ToString(), out num))
                return new Thickness(num, 0, num, num);
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
