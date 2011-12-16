using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesktopCore;

namespace WindowsDock.Core
{
    public class Commands : ObservableCollection<Command>
    {
        private int defaultIndex = -1;

        public Command Default
        {
            get { return DefaultIndex != -1 ? this[DefaultIndex] : null; }
            set {
                if (Contains(value))
                {
                    defaultIndex = IndexOf(value);
                }
                else
                {
                    Add(value);
                    defaultIndex = Count - 1;
                }
            }
        }

        public int DefaultIndex
        {
            get { return defaultIndex; }
            set { defaultIndex = value; }
        }
    }

    public class Command : NotifyPropertyChanged
    {
        private string name;
        private string path;
        private string args;

        public Command() { }

        public Command(string name, string path)
            : this()
        {
            Name = name;
            Path = path;
        }

        public Command(string name, string path, string args)
            : this(name, path)
        {
            Args = args;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                FirePropertyChanged("Name");
            }
        }

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                FirePropertyChanged("Path");
            }
        }

        public string Args
        {
            get { return args; }
            set
            {
                args = value;
                FirePropertyChanged("Args");
            }
        }
    }
}
