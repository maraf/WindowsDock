using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Media;
using System.Windows.Threading;
using System.ComponentModel;
using WindowsDock.Core;
using DesktopCore;

namespace WindowsDock.GUI
{
    public partial class MainWindow : Window
    {
        public DockHelper Helper { get; protected set; }

        public MainWindow()
        {
            InitializeComponent();

            Helper = new DockHelper(this);
            DataContext = Helper.Manager;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.OnSourceInitialized(e);
        }

        private void btnShortcut_Click(object sender, RoutedEventArgs e)
        {
            Helper.RunShortcut((sender as Button).Tag as Shortcut);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenConfigWindow();
        }

        private void window_MouseEnter(object sender, MouseEventArgs e)
        {
            Helper.ShowMainPanel();
        }

        private void window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Helper.Manager.AutoHiding && Helper.CurrentAutoHiding && Helper.CurrentExtension == ExtensionType.None)
                Helper.HideMainPanel();
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Helper.DoCentralization();
        }

        private void btnTextNotes_Click(object sender, RoutedEventArgs e)
        {
            Helper.ToggleExtension(ExtensionType.TextNotes);
        }

        private void btnNewTextNote_Click(object sender, RoutedEventArgs e)
        {
            Helper.Manager.TextNotes.Add(new TextNote(tbxNewTextNoteHeader.Text));
            tbxNewTextNoteHeader.Text = "";
            tbxNewTextNoteHeader.Focus();
        }

        private void btnRemoveTextNote_Click(object sender, RoutedEventArgs e)
        {
            Helper.Manager.TextNotes.Remove((sender as Button).Tag as TextNote);
        }

        private void tbxNewTextNoteHeader_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !String.IsNullOrEmpty(tbxNewTextNoteHeader.Text.Trim()))
                btnNewTextNote_Click(sender, e);
        }

        private void lvwTextNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvwTextNotes.SelectedItem != null)
            {
                Helper.OpenTextNoteDetail(lvwTextNotes.SelectedItem as TextNote);
                lvwTextNotes.SelectedItem = null;
            }
        }

        private void btnNewTextNoteFull_Click(object sender, RoutedEventArgs e)
        {
            Helper.IsNewTextNote = true;
            Helper.OpenTextNoteDetail(new TextNote());
        }

        private void btnScripts_Click(object sender, RoutedEventArgs e)
        {
            Helper.ToggleExtension(ExtensionType.Scripts);
        }

        private void btnRemoveScript_Click(object sender, RoutedEventArgs e)
        {
            Helper.Manager.Scripts.Remove((sender as Button).Tag as Script);
        }

        private void btnRunScript_Click(object sender, RoutedEventArgs e)
        {
            Script s = (sender as Button).Tag as Script;
            Process p = new Process();
            p.StartInfo.WorkingDirectory = s.WorkingDirectory;
            p.StartInfo.FileName = s.Path;
            p.Start();
        }

        private void btnNewScript_Click(object sender, RoutedEventArgs e)
        {
            Helper.IsNewScript = true;
            Helper.OpenScriptDetail(new Script());
        }

        private void btnEditScript_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenScriptDetail((sender as Button).Tag as Script);
        }

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            Helper.ToggleExtension(ExtensionType.Browser);
        }

        private void btnBrowseOpen_Click(object sender, RoutedEventArgs e)
        {
            BrowseHelper.SetPath(tbxBrowsePath, ((sender as Button).Tag as BrowseItem).Path);
            lvwCommands.IsEnabled = BrowseHelper.OpenFolder(tbxBrowsePath.Text, Helper.Manager);
        }

        private void tbxBrowsePath_KeyUp(object sender, KeyEventArgs e)
        {
            RunResult result = BrowseHelper.TryToRun(e.Key, lvwBrowse, tbxBrowsePath, Helper.Manager);
            if (result.Handled)
            {
                if (result.Process != null)
                    Helper.ToggleExtension(ExtensionType.Browser);

                lvwCommands.IsEnabled = result.Runnable;
                return;
            }
            lvwCommands.IsEnabled = BrowseHelper.OpenFolder(tbxBrowsePath.Text, Helper.Manager);
        }

        private void tbxBrowsePath_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                if (BrowseHelper.HandleBack(tbxBrowsePath, Helper.Manager))
                    e.Handled = true;
        }

        private void btnCommandOpen_Click(object sender, RoutedEventArgs e)
        {
            Command c = (sender as Button).Tag as Command;

            Process p = new Process();
            p.StartInfo.FileName = c.Path;
            p.StartInfo.Arguments = String.Format(c.Args, tbxBrowsePath.Text);
            p.Start();
            Helper.ToggleExtension(ExtensionType.Browser);
        }

        private void btnClearBrowse_Click(object sender, RoutedEventArgs e)
        {
            tbxBrowsePath.Text = "";
            Helper.Manager.BrowseItems.Clear();
            tbxBrowsePath.Focus();
        }

        private void window_Drop(object sender, DragEventArgs e)
        {
            IDataObject d = e.Data;
            string[] data = (string[])d.GetData(DataFormats.FileDrop);
            if (data != null && data.Length > 0)
            {
                foreach (string item in data)
                {
                    Helper.Manager.Shortcuts.Add(new Shortcut(item));
                }
            }
        }

        private void btnDesktop_Click(object sender, RoutedEventArgs e)
        {
            Helper.ToggleExtension(ExtensionType.Desktop);
        }

        private void btnDesktopOpen_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ((sender as Button).Tag as DesktopItem).Name));
            Helper.ToggleExtension(ExtensionType.Desktop, true);
        }

        private void btnTextNotesFilter_Click(object sender, RoutedEventArgs e)
        {
            Helper.FilterTextNotes();
        }

        private void tbxTextNotesFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Helper.FilterTextNotes();
        }

        private void btnDesktopFilter_Click(object sender, RoutedEventArgs e)
        {
            Helper.FilterDesktop();
        }

        private void tbxDesktopFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Helper.FilterDesktop();
        }

        private void window_Closed(object sender, EventArgs e)
        {
            Helper.Manager.SaveTo(Helper.Manager.DefaultLocation);
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (FocusManager.GetFocusedElement(this) is TextBox || !Helper.IsShown)
                return;

            if (e.Key == Helper.Manager.CloseKey)
                Close();
            else if (e.Key == Helper.Manager.ConfigKey)
                btnConfig_Click(sender, e);
            else if (e.Key == Helper.Manager.FolderBrowserKey && Helper.Manager.BrowserEnabled)
                Helper.ToggleExtension(ExtensionType.Browser);
            else if (e.Key == Helper.Manager.DesktopBrowserKey && Helper.Manager.DesktopEnabled)
                Helper.ToggleExtension(ExtensionType.Desktop);
            else if (e.Key == Helper.Manager.DesktopExplorerKey)
                Helper.RunExplorerWithDesktop();
            else if (e.Key == Helper.Manager.TextNotesKey && Helper.Manager.TextNotesEnabled)
                Helper.ToggleExtension(ExtensionType.TextNotes);
            else if (e.Key == Helper.Manager.ScriptsKey && Helper.Manager.ScriptsEnabled)
                Helper.ToggleExtension(ExtensionType.Scripts);
            else
                foreach (Shortcut item in Helper.Manager.Shortcuts)
                    if (item.Key != Key.None && item.Key == e.Key)
                        Helper.RunShortcut(item);

            e.Handled = true;
        }

        private void btnDesktopGo_Click(object sender, RoutedEventArgs e)
        {
            Helper.RunExplorerWithDesktop(true);
        }
    }
}
