using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using DesktopCore;

namespace WindowsDock.Core
{
    public enum ExtensionType { None, TextNotes, Scripts, Browser, Desktop }

    public enum WindowPosition
    {
        [Resource("WindowPosition.Top")]
        Top,

        [Resource("WindowPosition.Left")]
        Left,

        [Resource("WindowPosition.Right")]
        Right,

        [Resource("WindowPosition.Bottom")]
        Bottom
    }

    public enum WindowAlign
    {
        [Resource("WindowAlign.Left")]
        Left,

        [Resource("WindowAlign.Center")]
        Center,

        [Resource("WindowAlign.Right")]
        Right
    }

    public delegate void EditButtonHandler(object sender, EventArgs e);

    public class RunResult
    {
        public bool Handled = false;
        public bool Runnable = false;
        public Process Process = null;
    }
}
