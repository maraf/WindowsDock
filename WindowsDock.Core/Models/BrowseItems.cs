using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesktopCore;

namespace WindowsDock.Core
{
    public class BrowseItems : ObservableCollection<BrowseItem>
    {
    }

    public class BrowseItem : NotifyPropertyChanged {
        private string name;
        private string path;

        public BrowseItem() { }

        public BrowseItem(string name)
        {
            Name = name;
        }

        public BrowseItem(string name, string path)
            : this(name)
        {
            Path = path;
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
    }
}
