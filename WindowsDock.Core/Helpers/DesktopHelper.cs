using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows;

namespace WindowsDock.Core
{
    public class DesktopHelper
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern long ShowWindow(IntPtr hwnd, int nCmdShow);

        public static void ShowIcons(bool visible)
        {
            IntPtr hWnd_DesktopIcons;
            hWnd_DesktopIcons = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);
            if (visible)
            {
                ShowWindow(hWnd_DesktopIcons, SW_SHOW);
            }
            else
            {
                ShowWindow(hWnd_DesktopIcons, SW_HIDE);
            }
        }
    }
}
