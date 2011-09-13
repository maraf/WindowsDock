using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DesktopCore;

namespace WindowsDock.Core
{
    [Serializable]
    public class Scripts : ObservableCollection<Script>
    {
    }

    [Serializable]
    public class Script : NotifyPropertyChanged
    {
        private string header;
        private string path;
        private string workingDirectory;
        private DateTime modified;

        public Script()
        {
            modified = DateTime.Now;
        }

        public Script(string header, string path)
            : this()
        {
            Header = header;
            Path = path;
        }

        public Script(string header, string path, string workingDirectory)
            : this(header, path)
        {
            WorkingDirectory = workingDirectory;
        }

        public Script(string header, string path, string workingDirectory, DateTime modified)
            : this(header, path, workingDirectory)
        {
            Modified = modified;
        }

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                Modified = DateTime.Now;
                FirePropertyChanged("Header");
            }
        }

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                Modified = DateTime.Now;
                FirePropertyChanged("Path");
            }
        }

        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                workingDirectory = value;
                Modified = DateTime.Now;
                FirePropertyChanged("WorkingDirectory");
            }
        }

        public DateTime Modified
        {
            get { return modified; }
            set
            {
                modified = value;
                FirePropertyChanged("Modified");
            }
        }

    }
}
