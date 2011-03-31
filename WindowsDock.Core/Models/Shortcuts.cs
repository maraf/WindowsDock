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
using System.Windows.Input;

namespace WindowsDock.Core
{
    public class Shortcuts : ObservableCollection<Shortcut>
    {
        public static readonly Key[] PermitedKeys = new Key[] {
            Key.None,
            Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0, 
            Key.A, Key.C, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, 
            Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.U, Key.V, Key.W, Key.Y
        };

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
        private Key key = Key.None;

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

        public Key Key
        {
            get { return key; }
            set
            {
                key = value;
                FirePropertyChanged("Key");
            }
        }
    }
}
