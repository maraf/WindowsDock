using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using WindowsDock.Core;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Threading;
using System.Media;
using DesktopCore;
using System.Threading;

namespace WindowsDock.GUI
{
    public class DockHelper
    {
        private const double NormalHeight = 44;
        private const double NormalTop = 0;
        private const double HiddenTop = -44;

        private IList<UIElement> hotkeyIgnorables = new List<UIElement>();

        public MainWindow Window { get; protected set; }

        public IManager Manager { get; protected set; }

        public bool IsShown
        {
            //get { return PositionHelper.GetEdgeValue(Window, Manager.Position) == PositionHelper.GetComputedEgdeValue(Manager.Position, NormalTop); }
            get { return PositionHelper.IsShown(Window, Manager.Position, NormalTop, Manager.HiddenOffset); }
        }
        
        public bool IsSubWindowOpened { get; protected set; }
        public bool CurrentAutoHiding { get; set; }
        public bool IsNewTextNote { get; set; }
        public bool IsNewScript { get; set; }
        public ExtensionType CurrentExtension { get; set; }

        public DockHelper(MainWindow window)
        {
            Manager = ManagerFacory.Create();
            Window = window;
            CurrentAutoHiding = true;
            IsNewTextNote = false;
            IsNewScript = false;
            CurrentExtension = ExtensionType.None;

            try
            {
                (Manager as NotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(Manager_PropertyChanged);
                Manager.LoadFrom(Manager.DefaultLocation);
            }
            catch (UnableToLoadConfigurationException e)
            {
                MessageBox.Show("WindowsDock", "Unable to load configuration file, propably because it is damaged. Go to configuration and load one from backup (if have any) or start with this plain one.");
            }
            DoPosition();

            if (Manager.StartHidden)
                HideMainPanel();

            DesktopHelper.ShowIcons(Manager.DesktopIconsEnabled);

            LoadDefaults();
            RunDispatcher();
            FindHotkeyIgnorables();
        }

        /// <summary>
        /// Called on window SourceInitialized
        /// </summary>
        /// <param name="e">EventArgs</param>
        public void OnSourceInitialized(EventArgs e)
        {
            DesktopCore.WindowHelper.HideFromWindowList(Window);
            RegisterActivationHotkey(Manager.ActivationKey);
        }

        /// <summary>
        /// Tries to register windows global hotkey
        /// </summary>
        /// <param name="key">Hotkey (+Win)</param>
        /// <returns>If succeed true, otherwise false</returns>
        public bool RegisterActivationHotkey(Key key)
        {
            DesktopCore.WindowHelper.HideFromWindowList(Window);
            try
            {
                HotkeyHelper.RegisterHotKey(Window, key, HotkeyHelper.Win, delegate { ToggleMainPanel(); });
                return true;
            }
            catch (Win32Exception)
            {
                MessageBox.Show(String.Format(Resource.Get("Error.UnableToBindGlobalHotkeyFormat"), key), "WindowsDock");
                return false;
            }
        }

        /// <summary>
        /// Handler for setuping right view based on value of Manager.Position
        /// </summary>
        /// <param name="sender">Manager</param>
        /// <param name="e">EventArgs</param>
        public void Manager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Position" || e.PropertyName == "CornerRadius" || e.PropertyName == "BorderThickness")
            {
                SetupView();
            }
            else if (e.PropertyName == "Align" || e.PropertyName == "AlignOffset")
            {
                DoCentralization();
            }
            else if (e.PropertyName == "Locale")
            {
                Thread.CurrentThread.CurrentCulture = Manager.Locale;
                Resource.Load("Resources/Resources");
                Resource.ReProvideAll();
            }
        }

        /// <summary>
        /// Loads default shortcuts to Manager
        /// </summary>
        public void LoadDefaults()
        {
#if DEBUG
            if (Manager.Shortcuts.Count == 0)
            {
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\MediaPlayers\AIMP3\AIMP3.exe"));
                Manager.Shortcuts.Add(new Shortcut(@"C:\Program Files (x86)\RapidShareManager\RapidShareManager.exe"));
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

        #region Window position, centralization and view setup

        /// <summary>
        /// Setups position of the window
        /// </summary>
        public void DoPosition()
        {
            double position;
            if (Manager.StartHidden && CurrentAutoHiding)
                position = PositionHelper.GetComputedEgdeValue(Manager.Position, HiddenTop, NormalTop);
            else
                position = PositionHelper.GetComputedEgdeValue(Manager.Position, NormalTop, HiddenTop);

            PositionHelper.SetPosition(position, NormalHeight, Manager.Position, Window, Window.stpMainPanelItems);
            DoCentralization();
        }

        /// <summary>
        /// Centers main window
        /// </summary>
        public void DoCentralization()
        {
            if (Manager.Align == WindowAlign.Center)
                PositionHelper.SetToCenter(Manager.Position, Window);
            else
                PositionHelper.SetAlign(Manager.Position, Manager.Align, Manager.AlignOffset, Window);
        }

        /// <summary>
        /// Setups view regardless of Manager configuration
        /// </summary>
        public void SetupView()
        {
            switch (Manager.Position)
            {
                case WindowPosition.Top:
                    SetupTopView();
                    break;
                case WindowPosition.Left:
                    SetupLeftView();
                    break;
                case WindowPosition.Right:
                    SetupRightView();
                    break;
                case WindowPosition.Bottom:
                    SetupBottomView();
                    break;
            }
            DoPosition();
        }

        /// <summary>
        /// Setups Top view
        /// </summary>
        public void SetupTopView()
        {
            Window.stpMainPanelItems.Width = Double.NaN;
            Window.stpMain.Orientation = Orientation.Vertical;

            Window.brdBackground.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
            Window.brdBackground.BorderThickness = new Thickness(Manager.BorderThickness, 0, Manager.BorderThickness, Manager.BorderThickness);
            Window.stpMainPanelItems.Margin = new Thickness(5, 0, 5, 0);
            Window.stpMainPanelItems.Orientation = Orientation.Horizontal;

            FrameworkElementFactory panelFactory = new FrameworkElementFactory(typeof(StackPanel));
            panelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            panelFactory.SetValue(StackPanel.ClipToBoundsProperty, false);
            Window.lvwShortcuts.ItemsPanel = new ItemsPanelTemplate(panelFactory);
            Window.lvwShortcuts.Margin = new Thickness(0, 0, 5, 0);

            Window.stpApplicationButtons.Margin = new Thickness(4, 8, 0, 0);
            Window.stpApplicationButtons.Orientation = Orientation.Vertical;

            Window.btnConfig.Margin = new Thickness(0, 2, 0, 0);

            Window.brdBrowse.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
            Window.brdTextNotes.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
            Window.brdScripts.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
            Window.brdDesktop.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
        }

        /// <summary>
        /// Setups Left view
        /// </summary>
        public void SetupLeftView()
        {
            Window.stpMainPanelItems.Height = Double.NaN;
            Window.stpMain.Orientation = Orientation.Horizontal;

            Window.brdBackground.CornerRadius = new CornerRadius(0, Manager.CornerRadius, Manager.CornerRadius, 0);
            Window.brdBackground.BorderThickness = new Thickness(0, Manager.BorderThickness, Manager.BorderThickness, Manager.BorderThickness);
            Window.stpMainPanelItems.Margin = new Thickness(0, 5, 0, 5);
            Window.stpMainPanelItems.Orientation = Orientation.Vertical;

            FrameworkElementFactory panelFactory = new FrameworkElementFactory(typeof(StackPanel));
            panelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            panelFactory.SetValue(StackPanel.ClipToBoundsProperty, false);
            Window.lvwShortcuts.ItemsPanel = new ItemsPanelTemplate(panelFactory);
            Window.lvwShortcuts.Margin = new Thickness(0, 0, 0, 5);

            Window.stpApplicationButtons.Margin = new Thickness(8, 4, 0, 0);
            Window.stpApplicationButtons.Orientation = Orientation.Horizontal;

            Window.btnConfig.Margin = new Thickness(2, 0, 0, 0);

            Window.brdBrowse.CornerRadius = new CornerRadius(Manager.CornerRadius);
            Window.brdTextNotes.CornerRadius = new CornerRadius(Manager.CornerRadius);
            Window.brdScripts.CornerRadius = new CornerRadius(Manager.CornerRadius);
            Window.brdDesktop.CornerRadius = new CornerRadius(Manager.CornerRadius);
        }

        /// <summary>
        /// Setups Right view
        /// </summary>
        public void SetupRightView()
        {
            Window.stpMainPanelItems.Height = Double.NaN;
            Window.stpMain.Orientation = Orientation.Horizontal;

            Window.brdBackground.CornerRadius = new CornerRadius(Manager.CornerRadius, 0, 0, Manager.CornerRadius);
            Window.brdBackground.BorderThickness = new Thickness(Manager.BorderThickness, Manager.BorderThickness, 0, Manager.BorderThickness);
            Window.stpMainPanelItems.Margin = new Thickness(0, 5, 0, 5);
            Window.stpMainPanelItems.Orientation = Orientation.Vertical;

            FrameworkElementFactory panelFactory = new FrameworkElementFactory(typeof(StackPanel));
            panelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            panelFactory.SetValue(StackPanel.ClipToBoundsProperty, false);
            Window.lvwShortcuts.ItemsPanel = new ItemsPanelTemplate(panelFactory);
            Window.lvwShortcuts.Margin = new Thickness(0, 0, 0, 5);

            Window.stpApplicationButtons.Margin = new Thickness(8, 4, 0, 0);
            Window.stpApplicationButtons.Orientation = Orientation.Horizontal;

            Window.btnConfig.Margin = new Thickness(0, 0, 2, 0);

            Window.brdBrowse.CornerRadius = new CornerRadius(Manager.CornerRadius);
            Window.brdTextNotes.CornerRadius = new CornerRadius(Manager.CornerRadius);
            Window.brdScripts.CornerRadius = new CornerRadius(Manager.CornerRadius);
            Window.brdDesktop.CornerRadius = new CornerRadius(Manager.CornerRadius);
        }

        /// <summary>
        /// Setups Top view
        /// </summary>
        public void SetupBottomView()
        {
            Window.stpMainPanelItems.Width = Double.NaN;
            Window.stpMain.Orientation = Orientation.Vertical;

            Window.brdBackground.CornerRadius = new CornerRadius(Manager.CornerRadius, Manager.CornerRadius, 0, 0);
            Window.brdBackground.BorderThickness = new Thickness(Manager.BorderThickness, Manager.BorderThickness, Manager.BorderThickness, 0);
            Window.stpMainPanelItems.Margin = new Thickness(5, 0, 5, 0);
            Window.stpMainPanelItems.Orientation = Orientation.Horizontal;

            FrameworkElementFactory panelFactory = new FrameworkElementFactory(typeof(StackPanel));
            panelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            panelFactory.SetValue(StackPanel.ClipToBoundsProperty, false);
            Window.lvwShortcuts.ItemsPanel = new ItemsPanelTemplate(panelFactory);
            Window.lvwShortcuts.Margin = new Thickness(0, 0, 5, 0);

            Window.stpApplicationButtons.Margin = new Thickness(4, 8, 0, 0);
            Window.stpApplicationButtons.Orientation = Orientation.Vertical;

            Window.btnConfig.Margin = new Thickness(0, 2, 0, 0);

            Window.brdBrowse.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
            Window.brdTextNotes.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
            Window.brdScripts.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
            Window.brdDesktop.CornerRadius = new CornerRadius(0, 0, Manager.CornerRadius, Manager.CornerRadius);
        }

        #endregion

        #region MainPanel and extension show/hide

        /// <summary>
        /// Toggle main panel
        /// </summary>
        public void ToggleMainPanel()
        {
            if (IsShown && Window.IsActive)
                HideMainPanel();
            else
                ShowMainPanel();
        }

        /// <summary>
        /// Shows main panel
        /// </summary>
        public void ShowMainPanel()
        {
            //TODO: Crashes SetupView !!!
            DoCentralization();
            DoubleAnimation d = new DoubleAnimation(PositionHelper.GetComputedEgdeValue(Manager.Position, NormalTop, HiddenTop), Manager.HideDuration);
            Window.BeginAnimation(PositionHelper.GetEdgeProperty(Manager.Position), d);

            Window.Focus();
            Window.Activate();
        }

        /// <summary>
        /// Hides main panel
        /// </summary>
        public void HideMainPanel()
        {
            if (!IsSubWindowOpened && CurrentExtension == ExtensionType.None)
            {
                double top = PositionHelper.GetComputedEgdeValue(Manager.Position, HiddenTop, NormalTop) + Manager.HiddenOffset;
                DoubleAnimation d = new DoubleAnimation(top, Manager.HideDuration);
                d.BeginTime = Manager.HideDelay;
                Window.BeginAnimation(PositionHelper.GetEdgeProperty(Manager.Position), d);
            }
        }

        /// <summary>
        /// Toggles current extension, if passed extension is the same as current one, it hides it
        /// </summary>
        /// <param name="extension">Extension to toggle</param>
        public void ToggleExtension(ExtensionType extension, bool hideMainPanel = false)
        {
            if (CurrentExtension != extension)
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
            if (CurrentExtension != ExtensionType.None)
                HideExtension();

            if (extension == ExtensionType.TextNotes)
            {
                Window.grdTextNotes.Visibility = Visibility.Visible;
                Window.tbxNewTextNoteHeader.Focus();
                CurrentExtension = ExtensionType.TextNotes;
            }
            else if (extension == ExtensionType.Scripts)
            {
                Window.grdScripts.Visibility = Visibility.Visible;
                CurrentExtension = ExtensionType.Scripts;
            }
            else if (extension == ExtensionType.Browser)
            {
                Window.grdBrowse.Visibility = Visibility.Visible;
                CurrentExtension = ExtensionType.Browser;
                Window.tbxBrowsePath.Focus();
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
                Window.grdDesktop.Visibility = Visibility.Visible;
                CurrentExtension = ExtensionType.Desktop;
                Window.tbxDesktopFilter.Focus();
            }

            //currentAutoHiding = false;
        }

        /// <summary>
        /// Closes currently opened extension
        /// </summary>
        public void HideExtension(bool hideMainPanel = false)
        {
            if (CurrentExtension == ExtensionType.TextNotes)
                Window.grdTextNotes.Visibility = Visibility.Collapsed;
            else if (CurrentExtension == ExtensionType.Scripts)
                Window.grdScripts.Visibility = Visibility.Collapsed;
            else if (CurrentExtension == ExtensionType.Browser)
                Window.grdBrowse.Visibility = Visibility.Collapsed;
            else if (CurrentExtension == ExtensionType.Desktop)
                Window.grdDesktop.Visibility = Visibility.Collapsed;

            if (hideMainPanel)
                HideMainPanel();

            //currentAutoHiding = true;
            FocusManager.SetFocusedElement(Window, Window);
            CurrentExtension = ExtensionType.None;
        }

        #endregion

        /// <summary>
        /// Opens text note detail window
        /// </summary>
        /// <param name="textNote"></param>
        public void OpenTextNoteDetail(TextNote textNote)
        {
            IsSubWindowOpened = true;
            EditTextNoteWindow edit = new EditTextNoteWindow(textNote);
            edit.Closed += delegate
            {
                if (IsNewTextNote && (!String.IsNullOrEmpty(edit.TextNote.Header.Trim()) || !String.IsNullOrEmpty(edit.TextNote.Content.Trim())))
                    Manager.TextNotes.Add(edit.TextNote);
                edit = null;
                IsNewTextNote = false;
                IsSubWindowOpened = false;
            };
            edit.ShowDialog();
        }

        /// <summary>
        /// Opens text note detail window
        /// </summary>
        /// <param name="textNote"></param>
        public void OpenScriptDetail(Script script)
        {
            IsSubWindowOpened = true;
            EditScriptWindow edit = new EditScriptWindow(script);
            edit.Closed += delegate
            {
                if (IsNewScript && !String.IsNullOrEmpty(edit.Script.Header) && !String.IsNullOrEmpty(edit.Script.Path))
                    Manager.Scripts.Add(edit.Script);
                edit = null;
                IsNewScript = false;
                IsSubWindowOpened = false;
            };
            edit.ShowDialog();
        }

        /// <summary>
        /// Opens confguration window
        /// </summary>
        public void OpenConfigWindow()
        {
            IsSubWindowOpened = true;
            HideExtension();
            CurrentAutoHiding = false;
            EditWindow edit = new EditWindow(Window, Manager);
            edit.Closed += delegate { 
                edit = null; 
                CurrentAutoHiding = true;
                IsSubWindowOpened = false;
            };
            edit.ShowDialog();
        }

        public void RunDispatcher()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Dispatcher_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        public void Dispatcher_Tick(object sender, EventArgs e)
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

        public void FilterTextNotes()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Manager.TextNotes);
            view.Filter = (o) => { return (o as TextNote).Header.Contains(Window.tbxTextNotesFilter.Text); };
        }

        public void FilterDesktop()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Manager.DesktopItems);
            view.Filter = (o) => { return (o as DesktopItem).Name.Contains(Window.tbxDesktopFilter.Text); };
        }

        public void FindHotkeyIgnorables()
        {
            hotkeyIgnorables.Add(Window.tbxBrowsePath);
            hotkeyIgnorables.Add(Window.tbxDesktopFilter);
            hotkeyIgnorables.Add(Window.tbxNewTextNoteHeader);
            hotkeyIgnorables.Add(Window.tbxTextNotesFilter);
        }

        public void RunShortcut(Shortcut shortcut)
        {
            string path = shortcut.Path;
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.WorkingDirectory = shortcut.WorkingDirectory;
            p.Start();

            HideMainPanel();
        }
    }
}
