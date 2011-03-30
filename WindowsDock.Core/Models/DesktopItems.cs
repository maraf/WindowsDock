using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace WindowsDock.Core
{
    public class DesktopItems : ObservableCollection<DesktopItem>
    {
    }

    public class DesktopItem : NotifyPropertyChanged
    {
        private string name;
        private ImageSource imageSource;

        public DesktopItem() { }

        public DesktopItem(string name, ImageSource imageSource)
        {
            Name = name;
            ImageSource = imageSource;
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

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                FirePropertyChanged("ImageSource");
            }
        }
    }
}
