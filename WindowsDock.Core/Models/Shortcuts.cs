using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections;

namespace WindowsDock.Core
{
    public class Shortcuts : ObservableCollection<Shortcut>
    {
        public new void Add(Shortcut item)
        {
            base.Add(item);
        }

        public void Add(Shortcut item, int position)
        {
            if (position > Count)
                position = Count;

            Add(item, position);
        }

        public Shortcut Swap(Shortcut item1, Shortcut item2)
        {
            Shortcut item = new Shortcut(item1.Path, item1.ImageSource);
            item1.Path = item2.Path;
            item1.ImageSource = item2.ImageSource;
            item2.Path = item.Path;
            item2.ImageSource = item.ImageSource;

            return item2;
        }
    }

    public class Shortcut : NotifyPropertyChanged
    {
        private string path;
        private string args;
        private ImageSource imageSource;

        public Shortcut() { }

        public Shortcut(string path)
        {
            Path = path;
        }

        public Shortcut(string path, ImageSource icon)
        {
            Path = path;
            ImageSource = icon;
        }

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                ImageSource = IconHelper.GetIcon(path);
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

    public class IconHelper
    {
        public static ImageSource GetIcon(string filename)
        {
            System.Drawing.Icon extractedIcon = System.Drawing.Icon.ExtractAssociatedIcon(filename);
            ImageSource imgs;

            using (System.Drawing.Icon i = System.Drawing.Icon.FromHandle(extractedIcon.ToBitmap().GetHicon()))
            {
                imgs = Imaging.CreateBitmapSourceFromHIcon(i.Handle, new Int32Rect(0, 0, 32, 32), BitmapSizeOptions.FromEmptyOptions());
            }

            return imgs;
        }
    }
}
