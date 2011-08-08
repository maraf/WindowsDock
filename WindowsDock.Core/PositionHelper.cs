using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WindowsDock.Core
{
    public class PositionHelper
    {
        public static double GetEdgeValue(Window window, WindowPosition position)
        {
            switch (position)
            {
                case WindowPosition.Top:
                    return window.Top;
                case WindowPosition.Left:
                    return window.Left;
                //case WindowPosition.Right:
                //    return SystemParameters.PrimaryScreenWidth - window.Left;
                default:
                    return Double.NaN;
            }
        }

        public static DependencyProperty GetEdgeProperty(WindowPosition position)
        {
            switch (position)
            {
                case WindowPosition.Top:
                    return Window.TopProperty;
                case WindowPosition.Left:
                    return Window.LeftProperty;
                //case WindowPosition.Right:
                //    return null;
                default:
                    return null;
            }
        }

        public static void SetPosition(double position, double height, WindowPosition windowPosition, Window window, StackPanel panel)
        {
            switch (windowPosition)
            {
                case WindowPosition.Top:
                    window.Top = position;
                    panel.Height = height;
                    break;
                case WindowPosition.Left:
                    window.Left = position;
                    panel.Width = height;
                    break;
                default:
                    break;
            }
        }

        public static void SetToCenter(WindowPosition windowPosition, Window window)
        {
            double screenValue = windowPosition == WindowPosition.Top ? SystemParameters.PrimaryScreenWidth : SystemParameters.PrimaryScreenHeight;
            double thisValue = windowPosition == WindowPosition.Top ? window.ActualWidth : window.ActualHeight;
            double position = (screenValue - thisValue) / 2;

            switch (windowPosition)
            {
                case WindowPosition.Top:
                    window.Left = position;
                    break;
                case WindowPosition.Left:
                    window.Top = position;
                    break;
            }
        }

        public static void SetAlign(WindowPosition position, WindowAlign align, int offset, Window window)
        {
            if (position == WindowPosition.Top)
            {
                if (align == WindowAlign.Left)
                    window.Left = offset;
                else
                    window.Left = SystemParameters.PrimaryScreenWidth - window.ActualWidth - offset;
            }
            else
            {
                if (align == WindowAlign.Left)
                    window.Top = offset;
                else
                    window.Top = SystemParameters.PrimaryScreenHeight - window.ActualHeight - offset;
            }
        }
    }
}
