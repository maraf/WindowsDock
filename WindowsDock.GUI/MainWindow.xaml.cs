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
using WindowsDock.Core;
using System.IO;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Media;
using System.Windows.Threading;
using System.ComponentModel;

namespace WindowsDock.GUI
{
    public partial class MainWindow : Window
    {
        private const double NormalHeight = 44;
        private const double NormalTop = -10;
        private const double HiddenTop = -54;

        private IManager manager = ManagerFacory.Create();
        private bool currentAutoHiding = true;
        private bool isNewTextNote = false;
        private bool isNewScript = false;
        private ExtensionType currentExtension = ExtensionType.None;
        private IList<UIElement> hotkeyIgnorables = new List<UIElement>();

        public IManager Manager
        {
            get { return manager; }
        }

        public MainWindow()
        {
            InitializeComponent();

            DoPosition();
            try
            {
                Manager.LoadFrom(Manager.DefaultLocation);
            }
            catch (UnableToLoadConfigurationException e)
            {
                MessageBox.Show("WindowsDock", "Unable to load configuration file, propably because it is damaged. Go to configuration and load one from backup (if have any) or start with this plain one.");
            }

            DesktopHelper.ShowIcons(Manager.DesktopIconsEnabled);
            
            LoadDefaults();
            RunDispatcher();
            FindHotkeyIgnorables();

            DataContext = Manager;
        }

        /// <summary>
        /// Toggle main panel
        /// </summary>
        public void ToggleMainPanel()
        {
            if (Top == NormalTop && IsActive)
                HideMainPanel();
            else
                ShowMainPanel();
        }

        /// <summary>
        /// Shows main panel
        /// </summary>
        public void ShowMainPanel()
        {
            DoubleAnimation d = new DoubleAnimation(NormalTop, Manager.HideDuration);
            BeginAnimation(TopProperty, d);
            Focus();
            Activate();
        }

        /// <summary>
        /// Hides main panel
        /// </summary>
        public void HideMainPanel()
        {
            double top = HiddenTop + Manager.HiddenOffset;
            DoubleAnimation d = new DoubleAnimation(top > -10 ? -10 : top, Manager.HideDuration);
            d.BeginTime = Manager.HideDelay;
            BeginAnimation(TopProperty, d);
        }

        /// <summary>
        /// Toggles current extension, if passed extension is the same as current one, it hides it
        /// </summary>
        /// <param name="extension">Extension to toggle</param>
        public void ToggleExtension(ExtensionType extension, bool hideMainPanel = false)
        {
            if (currentExtension != extension)
                OpenExtension(extension);
            else
                HideExtension(hideMainPanel);
        }

        /// <summary>
        /// Open extension panel and also close opened extension
        /// </summary>
        /// <param name="extension">Extension to open</param>
        public void OpenExtension(ExtensionType extension)
        {
            if (currentExtension != ExtensionType.None)
                HideExtension();

            if (extension == ExtensionType.TextNotes)
            {
                grdTextNotes.Visibility = Visibility.Visible;
                tbxNewTextNoteHeader.Focus();
                currentExtension = ExtensionType.TextNotes;
            }
            else if (extension == ExtensionType.Scripts)
            {
                grdScripts.Visibility = Visibility.Visible;
                currentExtension = ExtensionType.Scripts;
            }
            else if (extension == ExtensionType.Browser)
            {
                grdBrowse.Visibility = Visibility.Visible;
                currentExtension = ExtensionType.Browser;
                tbxBrowsePath.Focus();
            }
            else if (extension == ExtensionType.Desktop)
            {
                if (Manager.DesktopItems.Count == 0)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    foreach (string item in Directory.EnumerateFiles(path))
                    {
                        string name = item.Substring(BrowseHelper.LastIndexOfSlash(item) + 1);
                        ImageSource source = IconHelper.GetIcon(item);
                        Manager.DesktopItems.Add(new DesktopItem(name, source));
                    }
                }
                grdDesktop.Visibility = Visibility.Visible;
                currentExtension = ExtensionType.Desktop;
                tbxDesktopFilter.Focus();
            }

            //currentAutoHiding = false;
        }

        /// <summary>
        /// Closes currently opened extension
        /// </summary>
        public void HideExtension(bool hideMainPanel = false)
        {
            if (currentExtension == ExtensionType.TextNotes)
                grdTextNotes.Visibility = Visibility.Collapsed;
            else if (currentExtension == ExtensionType.Scripts)
                grdScripts.Visibility = Visibility.Collapsed;
            else if (currentExtension == ExtensionType.Browser)
                grdBrowse.Visibility = Visibility.Collapsed;
            else if (currentExtension == ExtensionType.Desktop)
                grdDesktop.Visibility = Visibility.Collapsed;

            if(hideMainPanel)
                HideMainPanel();

            //currentAutoHiding = true;
            FocusManager.SetFocusedElement(this, this);
            currentExtension = ExtensionType.None;
        }

        /// <summary>
        /// Loads default shortcuts to Manager
        /// </summary>
        private void LoadDefaults()
        {
#if DEBUG
            if (Manager.Shortcuts.Count == 0)
            {
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\Browsers\Chromium Auto-Updater\chrome.exe"));
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\Browsers\Chromium Auto-Updater\Chromium-Updater.exe"));
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\My\Scheduling\Scheduling.exe"));
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\Utils\Kaxaml\Kaxaml.exe"));
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\MediaPlayers\AIMP2\AIMP2.exe"));
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\RapidShareManager\RapidShareManager.exe"));
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\VMware\VMware Workstation\vmware.exe"));
                //Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\Neptuo DEV\FileBackuper\FileBackuper.GUI.exe"));
                //Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\Neptuo DEV\LinkExtractor\LinkExtractor.exe"));
                //Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\Utils\MyUtilities\ImagePreviewer.GUI.exe"));
                //Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\Utils\MyUtilities\StarFinder.GUI.exe"));
            }
            if (Manager.TextNotes.Count == 0)
            {
                Manager.TextNotes.Add(new TextNote("Hello World!", "Hello World from WindowsDock TextNotes!!", DateTime.Now.Add(new TimeSpan(-5, 0, 0))));
                Manager.TextNotes.Add(new TextNote("Lorem ipsum", "Lorem ipsum sit amer dolor.", DateTime.Now.Add(new TimeSpan(-4, 0, 0))));
                Manager.TextNotes.Add(new TextNote("X33EJA", "Do 4.3. udelat vse potrebne pro checkpoint 2!", DateTime.Now.Add(new TimeSpan(-3, 0, 0))));
                Manager.TextNotes.Add(new TextNote("Moje bakalarska prace", "Dokoncit bakalarskou praci.", DateTime.Now.Add(new TimeSpan(-2, 0, 0))));
            }
            if (Manager.Scripts.Count == 0)
            {
                Manager.Scripts.Add(new Script("TestScript", @"D:\Temp\WindowsDock\Scripts\dir.cmd", @"D:\Temp", DateTime.Now.Add(new TimeSpan(-4, 0, 0))));
            }
#endif
            if (Manager.Commands.Count == 0)
            {
                Manager.Commands.Add(new Command("Explorer", "explorer", "{0}"));
                Manager.Commands.Add(new Command("cmd", "cmd.exe", @"/a /k Scripts\opencmd.cmd ""{0}"""));
#if DEBUG
                Manager.Commands.Add(new Command("ttcmd", @"C:\Program Files (x86)\totalcmd\totalcmd.exe", @"{0}"));
#endif
                Manager.Commands.Default = Manager.Commands[0];
            }
        }

        /// <summary>
        /// Setups position of the window
        /// </summary>
        private void DoPosition()
        {
            Top = NormalTop;
            Left = (SystemParameters.PrimaryScreenWidth - ActualWidth) / 2;
            grdIconsPanel.Height = NormalHeight;
        }

        /// <summary>
        /// Opens text note detail window
        /// </summary>
        /// <param name="textNote"></param>
        private void OpenTextNoteDetail(TextNote textNote)
        {
            EditTextNoteWindow edit = new EditTextNoteWindow(textNote);
            edit.Closed += delegate
            {
                if (isNewTextNote && (!String.IsNullOrEmpty(edit.TextNote.Header.Trim()) || !String.IsNullOrEmpty(edit.TextNote.Content.Trim())))
                    Manager.TextNotes.Add(edit.TextNote);
                edit = null;
                isNewTextNote = false;
            };
            edit.ShowDialog();
        }

        /// <summary>
        /// Opens text note detail window
        /// </summary>
        /// <param name="textNote"></param>
        private void OpenScriptDetail(Script script)
        {
            EditScriptWindow edit = new EditScriptWindow(script);
            edit.Closed += delegate
            {
                if (isNewScript && !String.IsNullOrEmpty(edit.Script.Header) && !String.IsNullOrEmpty(edit.Script.Path))
                    Manager.Scripts.Add(edit.Script);
                edit = null;
                isNewScript = false;
            };
            edit.ShowDialog();
        }

        private void RunDispatcher()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Dispatcher_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private void FilterTextNotes()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Manager.TextNotes);
            view.Filter = (o) => { return (o as TextNote).Header.Contains(tbxTextNotesFilter.Text); };
        }

        private void FilterDesktop()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Manager.DesktopItems);
            view.Filter = (o) => { return (o as DesktopItem).Name.Contains(tbxDesktopFilter.Text); };
        }

        private void FindHotkeyIgnorables()
        {
            hotkeyIgnorables.Add(tbxBrowsePath);
            hotkeyIgnorables.Add(tbxDesktopFilter);
            hotkeyIgnorables.Add(tbxNewTextNoteHeader);
            hotkeyIgnorables.Add(tbxTextNotesFilter);
        }

        private void RunShortcut(Shortcut shortcut)
        {
            string path = shortcut.Path;
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.WorkingDirectory = shortcut.WorkingDirectory;
            p.Start();

            HideMainPanel();
        }

        private void Dispatcher_Tick(object sender, EventArgs e)
        {
            //TODO: Check Notes!!
            foreach (TextNote item in Manager.TextNotes)
            {
                if (item.Alarm != null && item.Alarm.Value.Year == DateTime.Now.Year
                    && item.Alarm.Value.Month == DateTime.Now.Month && item.Alarm.Value.Day == DateTime.Now.Day
                    && item.Alarm.Value.Hour == DateTime.Now.Hour && item.Alarm.Value.Minute == DateTime.Now.Minute)
                {
                    SoundPlayer player = new SoundPlayer(Manager.AlarmSound);
                    player.Play();
                    item.IsAlarming = true;
                }
                else
                    item.IsAlarming = false;
            }

        }
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowHelper.HideFromWindowList(this);

            HotkeyHelper.RegisterHotKey(this, Key.W, HotkeyHelper.Win, delegate { ToggleMainPanel(); });
        }

        private void btnShortcut_Click(object sender, RoutedEventArgs e)
        {
            RunShortcut((sender as Button).Tag as Shortcut);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            HideExtension();
            currentAutoHiding = false;
            EditWindow edit = new EditWindow(Manager);
            edit.Closed += delegate { edit = null; currentAutoHiding = true; };
            edit.ShowDialog();
        }

        private void window_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowMainPanel();
        }

        private void window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Manager.AutoHiding && currentAutoHiding && currentExtension == ExtensionType.None)
                HideMainPanel();
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DoPosition();
        }

        private void btnTextNotes_Click(object sender, RoutedEventArgs e)
        {
            ToggleExtension(ExtensionType.TextNotes);
        }

        private void btnNewTextNote_Click(object sender, RoutedEventArgs e)
        {
            Manager.TextNotes.Add(new TextNote(tbxNewTextNoteHeader.Text));
            tbxNewTextNoteHeader.Text = "";
            tbxNewTextNoteHeader.Focus();
        }

        private void btnRemoveTextNote_Click(object sender, RoutedEventArgs e)
        {
            Manager.TextNotes.Remove((sender as Button).Tag as TextNote);
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
                OpenTextNoteDetail(lvwTextNotes.SelectedItem as TextNote);
                lvwTextNotes.SelectedItem = null;
            }
        }

        private void btnNewTextNoteFull_Click(object sender, RoutedEventArgs e)
        {
            isNewTextNote = true;
            OpenTextNoteDetail(new TextNote());
        }

        private void btnScripts_Click(object sender, RoutedEventArgs e)
        {
            ToggleExtension(ExtensionType.Scripts);
        }

        private void btnRemoveScript_Click(object sender, RoutedEventArgs e)
        {
            Manager.Scripts.Remove((sender as Button).Tag as Script);
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
            isNewScript = true;
            OpenScriptDetail(new Script());
        }

        private void btnEditScript_Click(object sender, RoutedEventArgs e)
        {
            OpenScriptDetail((sender as Button).Tag as Script);
        }

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            ToggleExtension(ExtensionType.Browser);
        }

        private void btnBrowseOpen_Click(object sender, RoutedEventArgs e)
        {
            BrowseHelper.SetPath(tbxBrowsePath, ((sender as Button).Tag as BrowseItem).Path);
            lvwCommands.IsEnabled = BrowseHelper.OpenFolder(tbxBrowsePath.Text, Manager);
        }

        private void tbxBrowsePath_KeyUp(object sender, KeyEventArgs e)
        {
            RunResult result = BrowseHelper.TryToRun(e.Key, lvwBrowse, tbxBrowsePath, Manager);
            if (result.Handled)
            {
                if (result.Process != null)
                    ToggleExtension(ExtensionType.Browser);

                lvwCommands.IsEnabled = result.Runnable;
                return;
            }
            lvwCommands.IsEnabled = BrowseHelper.OpenFolder(tbxBrowsePath.Text, Manager);
        }

        private void tbxBrowsePath_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                if (BrowseHelper.HandleBack(tbxBrowsePath, Manager))
                    e.Handled = true;
        }

        private void btnCommandOpen_Click(object sender, RoutedEventArgs e)
        {
            Command c = (sender as Button).Tag as Command;

            Process p = new Process();
            p.StartInfo.FileName = c.Path;
            p.StartInfo.Arguments = String.Format(c.Args, tbxBrowsePath.Text);
            p.Start();
            ToggleExtension(ExtensionType.Browser);
        }

        private void btnClearBrowse_Click(object sender, RoutedEventArgs e)
        {
            tbxBrowsePath.Text = "";
            Manager.BrowseItems.Clear();
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
                    Manager.Shortcuts.Add(new Shortcut(item));
                }
            }
        }

        private void btnDesktop_Click(object sender, RoutedEventArgs e)
        {
            ToggleExtension(ExtensionType.Desktop);
        }

        private void btnDesktopOpen_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + ((sender as Button).Tag as DesktopItem).Name);
            ToggleExtension(ExtensionType.Desktop, true);
        }

        private void btnTextNotesFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterTextNotes();
        }

        private void tbxTextNotesFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                FilterTextNotes();
        }

        private void btnDesktopFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterDesktop();
        }

        private void tbxDesktopFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                FilterDesktop();
        }

        private void window_Closed(object sender, EventArgs e)
        {
            Manager.SaveTo(Manager.DefaultLocation);
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (FocusManager.GetFocusedElement(this) is TextBox)
                return;

            switch (e.Key)
            {
                case Key.B: ToggleExtension(ExtensionType.Browser); break;
                case Key.D: ToggleExtension(ExtensionType.Desktop); break;
                case Key.S: ToggleExtension(ExtensionType.Scripts); break;
                case Key.T: ToggleExtension(ExtensionType.TextNotes); break;
                case Key.X: Close(); break;
                case Key.Z: btnConfig_Click(sender, e); break;
            }

            foreach (Shortcut item in Manager.Shortcuts)
            {
                if (item.Key != Key.None && item.Key == e.Key)
                    RunShortcut(item);
            }

            e.Handled = true;
        }

        private void btnDesktopGo_Click(object sender, RoutedEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "explorer";
            p.StartInfo.Arguments = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            p.Start();
            ToggleExtension(ExtensionType.Desktop, true);
        }
    }
}
