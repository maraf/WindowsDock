using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsDock.Core;
using System.IO;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;

namespace WindowsDock.Core
{
    public class BrowseHelper
    {
        public static int LastIndexOfSlash(string path)
        {
            int i = path.LastIndexOf(@"\");
            if (i > 0)
                return i;
            else
                return path.LastIndexOf(@"/");
        }

        public static int LastButOneIndexOfSlash(string path)
        {
            int last = LastIndexOfSlash(path);
            if (last < 0)
                return last;
            return LastIndexOfSlash(path.Substring(0, last - 1));
        }

        public static string ParentPath(string path)
        {
            if (LastButOneIndexOfSlash(path) == -1)
                return path;
            else if (EndsWithSlash(path))
                return path.Substring(0, LastButOneIndexOfSlash(path));
            else
                return path.Substring(0, LastIndexOfSlash(path));
        }

        public static bool EndsWithSlash(string path)
        {
            return path.EndsWith("/") || path.EndsWith(@"\");
        }

        private static BrowseItem CreateItem(string path)
        {
            return new BrowseItem(path.Substring(path.LastIndexOf(@"\") + 1), path);
        }

        public static bool OpenFolder(string path, IManager manager)
        {
            bool result;
            manager.BrowseItems.Clear();
            if (EndsWithSlash(path) && Directory.Exists(path))
            {
                if (LastButOneIndexOfSlash(path) != -1)
                    manager.BrowseItems.Add(new BrowseItem("..", ParentPath(path)));
                try
                {
                    foreach (string item in Directory.EnumerateDirectories(path))
                    {
                        //if(Directory.GetAccessControl(item) == AccessControlType.Allow)
                        manager.BrowseItems.Add(CreateItem(item));
                    }
                }
                catch (UnauthorizedAccessException e) { }
                result = true;
            }
            else
            {
                string dirPath = null;
                if (!EndsWithSlash(path))
                {
                    dirPath = path.Substring(0, LastIndexOfSlash(path) + 1);
                }
                if (dirPath != null && Directory.Exists(dirPath))
                {
                    string search = path.Substring(dirPath.Length, path.Length - dirPath.Length);
                    search += search.EndsWith("*") || search.EndsWith("?") ? "" : "*";
                    foreach (string item in Directory.EnumerateDirectories(dirPath, search))
                    {
                        manager.BrowseItems.Add(CreateItem(item));
                    }
                }
                result = false;
            }
            return result;
        }

        public static RunResult TryToRun(Key key, ListView view, TextBox textBox, IManager manager)
        {
            RunResult result = new RunResult();
            if (key == Key.Down || key == Key.Up || key == Key.Enter)
            {
                if (key == Key.Enter)
                {
                    if (view.SelectedItem == null)
                    {
                        //Open default
                        Process p = new Process();
                        p.StartInfo.FileName = manager.Commands.Default.Path;
                        p.StartInfo.Arguments = String.Format(manager.Commands.Default.Args, textBox.Text);
                        p.Start();
                        result.Process = p;
                    }
                    else
                    {
                        SetPath(textBox, (view.SelectedItem as BrowseItem).Path);
                        result.Runnable = OpenFolder(textBox.Text, manager);
                    }
                }
                else if (key == Key.Down)
                {
                    if (view.SelectedItem == null)
                        view.SelectedIndex = 0;
                    else
                        view.SelectedIndex++;
                }
                else if (key == Key.Up)
                {
                    if (view.SelectedItem == null)
                        view.SelectedIndex = view.Items.Count - 1;
                    else
                        view.SelectedIndex--;
                }
                result.Handled = true;
            } else
                result.Handled = false;
            return result;
        }

        public static void SetPath(TextBox textBox, string path)
        {
            textBox.Text = path + (EndsWithSlash(path) ? "" : @"\");
            textBox.SelectionStart = textBox.Text.Length;
            textBox.Focus();
        }

        public static bool HandleBack(TextBox textBox, IManager manager)
        {
            if (EndsWithSlash(textBox.Text))
            {
                string path = ParentPath(textBox.Text);
                if (textBox.Text != path)
                {
                    SetPath(textBox, path);
                    OpenFolder(textBox.Text, manager);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
