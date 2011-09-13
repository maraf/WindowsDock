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
using DesktopCore;

namespace WindowsDock.Core
{
    public class Shortcuts : ObservableCollection<Shortcut>
    {
        public static readonly Key[] PermitedKeys = new Key[] {
            Key.None,
            Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0, 
            Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, 
            Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T, Key.U, Key.V, Key.W, 
            Key.X, Key.Y, Key.Z, Key.Space
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
            Shortcut item = new Shortcut(item1.Path, item1.ImageSource, item1.Args, item1.WorkingDirectory, item1.Key);
            item1.Path = item2.Path;
            item1.ImageSource = item2.ImageSource;
            item1.Args = item2.Args;
            item1.WorkingDirectory = item2.WorkingDirectory;
            item1.Key = item2.Key;
            item2.Path = item.Path;
            item2.ImageSource = item.ImageSource;
            item2.Args = item.Args;
            item2.WorkingDirectory = item.WorkingDirectory;
            item2.Key = item.Key;

            return item2;
        }
    }

    public class Shortcut : NotifyPropertyChanged
    {
        private string path;
        private string args;
        private string workingDirectory;
        private ImageSource imageSource;
        private Key key = Key.None;

        public Shortcut() { }

        public Shortcut(string path)
        {
            Path = path;
            WorkingDirectory = System.IO.Path.GetDirectoryName(Path);
        }

        public Shortcut(string path, ImageSource icon, string args, string workingDirectory, Key key)
        {
            Path = path;
            WorkingDirectory = workingDirectory;
            ImageSource = icon;
            Args = args;
            Key = key;
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

        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                if(!String.IsNullOrEmpty(value) && !String.IsNullOrWhiteSpace(value))
                {
                    workingDirectory = value;
                    FirePropertyChanged("WorkingDirectory");
                }
            }
        }
    }
}
