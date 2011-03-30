﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WindowsDock.Core
{
    public enum ExtensionType { None, TextNotes, Scripts, Browser, Desktop }

    public delegate void EditButtonHandler(object sender, EventArgs e);

    public class RunResult
    {
        public bool Handled = false;
        public bool Runnable = false;
        public Process Process = null;
    }
}
