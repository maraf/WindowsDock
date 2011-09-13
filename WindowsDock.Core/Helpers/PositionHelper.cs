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
        public static bool IsShown(Window window, WindowPosition position, double edgeValue, double hiddenOffset) 
        {
            switch (position)
            {
                case WindowPosition.Top:
                    return window.Top == edgeValue;
                case WindowPosition.Left:
                    return window.Left == edgeValue;
                case WindowPosition.Right:
                    return GetEdgeValue(window, position) != (edgeValue - hiddenOffset);
                case WindowPosition.Bottom:
                    return GetEdgeValue(window, position) != (edgeValue - hiddenOffset);
                default:
                    return false;
            }
        }

        public static double GetEdgeValue(Window window, WindowPosition position)
        {
            switch (position)
            {
                case WindowPosition.Top:
                    return window.Top;
                case WindowPosition.Left:
                    return window.Left;
                case WindowPosition.Right:
                    return SystemParameters.PrimaryScreenWidth - window.Left;
                case WindowPosition.Bottom:
                    return SystemParameters.PrimaryScreenHeight - window.Top;
                default:
                    return Double.NaN;
            }
        }

        public static double GetComputedEgdeValue(WindowPosition position, double normalEdge, double hiddenEdge)
        {
            switch (position)
            {
                case WindowPosition.Top:
                case WindowPosition.Left:
                    return normalEdge;
                case WindowPosition.Right:
                    return SystemParameters.PrimaryScreenWidth + hiddenEdge;
                case WindowPosition.Bottom:
                    return SystemParameters.PrimaryScreenHeight + hiddenEdge;
                default:
                    return Double.NaN;
            }
        }

        public static DependencyProperty GetEdgeProperty(WindowPosition position)
        {
            switch (position)
            {
                case WindowPosition.Top:
                case WindowPosition.Bottom:
                    return Window.TopProperty;
                case WindowPosition.Left:
                case WindowPosition.Right:
                    return Window.LeftProperty;
                default:
                    return null;
            }
        }

        public static void SetPosition(double position, double height, WindowPosition windowPosition, Window window, StackPanel panel)
        {
            switch (windowPosition)
            {
                case WindowPosition.Top:
                case WindowPosition.Bottom:
                    window.Top = position;
                    panel.Height = height;
                    break;
                case WindowPosition.Left:
                case WindowPosition.Right:
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
                case WindowPosition.Bottom:
                    window.Left = position;
                    break;
                case WindowPosition.Left:
                case WindowPosition.Right:
                    window.Top = position;
                    break;
            }
        }

        public static void SetAlign(WindowPosition position, WindowAlign align, int offset, Window window)
        {
            if (position == WindowPosition.Top || position == WindowPosition.Bottom)
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
