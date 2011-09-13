using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;

namespace WindowsDock.Core
{
    public delegate void OnHotkeyPress();

    public class HotkeyHelper
    {
        private static IList<HotkeyMapping> mappings = new List<HotkeyMapping>();

        public static void RegisterHotKey(Window window, Key key, uint modifiers, OnHotkeyPress deleg)
        {
            mappings.Add(new HotkeyMapping(window, modifiers, (uint)KeyInterop.VirtualKeyFromKey(key), deleg));
        }

        public static void UnRegisterHotKey(Window window)
        {
            HotkeyMapping mapping = null;
            foreach (HotkeyMapping mpp in mappings)
            {
                if (mpp.Window == window)
                    mapping = mpp;
            }
            if (mapping != null)
            {
                mappings.Remove(mapping);
                mapping.UnRegister();
            }
        }

        public const int Alt = 1;
        public const int Control = 2;
        public const int Shift = 4;
        public const int Win = 8;
    }

    internal class HotkeyMapping
    {
        public Window Window { get; protected set; }
        public HwndSource HWndSource { get; protected set; }
        public short Atom { get; protected set; }
        public event OnHotkeyPress onHotkey;

        public HotkeyMapping(Window win, uint fsModifiers, uint vk, OnHotkeyPress deleg)
        {
            Window = win;

            Window.Closed += new EventHandler(Window_Closed);
            onHotkey += deleg;

            WindowInteropHelper wih = new WindowInteropHelper(Window);
            HWndSource = HwndSource.FromHwnd(wih.Handle);
            HWndSource.AddHook(MainWindowProc);

            Atom = Win32.GlobalAddAtom(Window.GetType().ToString());

            if (Atom == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!Win32.RegisterHotKey(wih.Handle, Atom, fsModifiers, vk))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public void UnRegister()
        {
            if (Atom != 0)
                Win32.UnregisterHotKey(HWndSource.Handle, Atom);
        }

        public void Window_Closed(object sender, EventArgs e)
        {
            if (Atom != 0)
                Win32.UnregisterHotKey(HWndSource.Handle, Atom);
        }

        public IntPtr MainWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_HOTKEY:
                    onHotkey();
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }
    }

    internal static class Win32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);

        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);

        public const int MOD_ALT = 1;
        public const int MOD_CONTROL = 2;
        public const int MOD_SHIFT = 4;
        public const int MOD_WIN = 8;

        public const uint VK_KEY_C = 0x43;

        public const int WM_HOTKEY = 0x312;
    }
}
