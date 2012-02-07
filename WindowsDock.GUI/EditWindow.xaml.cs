using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsDock.Core;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using DesktopCore;

namespace WindowsDock.GUI
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private bool isNew = false;
        private bool isNewCommand = false;
        private IManager manager;
        private MainWindow mainWindow;

        public IManager Manager { get { return manager; } protected set { manager = value; } }
        public MainWindow MainWindow { get { return mainWindow; } protected set { mainWindow = value; } }

        public EditWindow(MainWindow mainWindow, IManager manager)
        {
            InitializeComponent();

            Manager = manager;
            MainWindow = mainWindow;

            DataContext = Manager;

#if DEBUG
            btnDev.Visibility = Visibility.Visible;
#else
            btnDev.Visibility = Visibility.Collapsed;
#endif

            expShortcuts.IsExpanded = true;
            tblVersion.Text = String.Format("build {0} v{1}", Version.BuildDate.ToShortDateString(), Version.Current);

            coxPosition.ItemsSource = ResourceHelper.GetEnum<WindowPosition>(Resource.Resources);
            coxAlign.ItemsSource = ResourceHelper.GetEnum<WindowAlign>(Resource.Resources);
            coxLanguages.ItemsSource = DesktopCore.Resources.GetSupportedLocales("Resources/Resources", "en-US");
            coxShortcutBubbleFontSize.ItemsSource = new List<int> { 6, 7, 8, 9, 10, 11, 12 };
            coxShortcutIconSize.ItemsSource = new List<int> { 24, 32 };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        public static Key[] GetPermittedKeys()
        {
            return Shortcuts.PermitedKeys;
        }

        protected void OpenDetail(Shortcut shortcut)
        {
            EditShortcutWindow edit = new EditShortcutWindow(MainWindow.Helper, shortcut);
            edit.Closed += delegate {
                if(isNew && edit.Shortcut.Path != null) {
                    Manager.Shortcuts.Add(edit.Shortcut);
                }
                edit = null;
            };
            edit.ShowDialog();
        }

        protected void OpenCommanDetail(Command command)
        {
            EditCommandWindow edit = new EditCommandWindow(command);
            edit.Closed += delegate {
                if (isNewCommand)
                {
                    if (!String.IsNullOrEmpty(edit.Command.Name.Trim()) && !String.IsNullOrEmpty(edit.Command.Path.Trim()))
                        Manager.Commands.Add(edit.Command);
                } 
                edit = null;
                isNewCommand = false;
            };
            edit.ShowDialog();
        }

        protected void OpenCurrentShortcut()
        {
            if (lvwShortcuts.SelectedItem != null)
            {
                isNew = false;
                OpenDetail(lvwShortcuts.SelectedItem as Shortcut);
            }
        }

        protected void DeleteCurrentShortcut()
        {
            if (lvwShortcuts.SelectedItem != null)
            {
                Manager.Shortcuts.Remove(lvwShortcuts.SelectedItem as Shortcut);
            }
        }

        protected void MoveUpCurrentShortcut()
        {
            if (lvwShortcuts.SelectedItem != null)
            {
                if (lvwShortcuts.SelectedIndex != 0)
                    lvwShortcuts.SelectedItem = Manager.Shortcuts.Swap(lvwShortcuts.SelectedItem as Shortcut, lvwShortcuts.Items[lvwShortcuts.SelectedIndex - 1] as Shortcut);
                lvwShortcuts.ScrollIntoView(lvwShortcuts.SelectedItem);
                lvwShortcuts.Focus();
            }
        }

        protected void MoveDownCurrentShortcut()
        {
            if (lvwShortcuts.SelectedItem != null)
            {
                if (lvwShortcuts.SelectedIndex != lvwShortcuts.Items.Count - 1)
                    lvwShortcuts.SelectedItem = Manager.Shortcuts.Swap(lvwShortcuts.SelectedItem as Shortcut, lvwShortcuts.Items[lvwShortcuts.SelectedIndex + 1] as Shortcut);
                lvwShortcuts.ScrollIntoView(lvwShortcuts.SelectedItem);
                lvwShortcuts.Focus();
            }
        }

        protected string PickColor()
        {
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return String.Format(dialog.Color.IsKnownColor ? "{0}" : "#{0}", dialog.Color.Name);

            return null;
        }

        private void lvwShortcuts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stpEditButtons.IsEnabled = lvwShortcuts.SelectedItem != null;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            OpenCurrentShortcut();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrentShortcut();
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            MoveUpCurrentShortcut();
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            MoveDownCurrentShortcut();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            OpenDetail(new Shortcut());
        }

        private void lvwCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stpEditCommandButtons.IsEnabled = lvwCommands.SelectedItem != null;
        }

        private void btnEditCommand_Click(object sender, RoutedEventArgs e)
        {
            OpenCommanDetail(lvwCommands.SelectedItem as Command);
        }

        private void btnRemoveCommand_Click(object sender, RoutedEventArgs e)
        {
            Manager.Commands.Remove(lvwCommands.SelectedItem as Command);
        }

        private void btnNewCommand_Click(object sender, RoutedEventArgs e)
        {
            isNewCommand = true;
            OpenCommanDetail(new Command());
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander ex = sender as Expander;

            if (ex != expShortcuts)
                expShortcuts.IsExpanded = false;
            if (ex != expExtensions)
                expExtensions.IsExpanded = false;
            if (ex != expComands)
                expComands.IsExpanded = false;
            if (ex != expMiscelaneous)
                expMiscelaneous.IsExpanded = false;
            if (ex != expVisuals)
                expVisuals.IsExpanded = false;
            if (ex != expRestore)
                expRestore.IsExpanded = false;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Filter = "WAV file (*.wav)|*.wav";
            file.Title = Resource.Get("Edit.AlarmFile");
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Manager.AlarmSound = file.FileName;
            }
        }

        private void btnRestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            Manager.Restore(cbxShotcuts.IsChecked.Value, cbxTextNotes.IsChecked.Value, cbxScripts.IsChecked.Value, cbxSettings.IsChecked.Value);

            MainWindow.Helper.RegisterActivationHotkey(Manager.ActivationKey);

            tblRestoredInfo.Visibility = Visibility.Visible;
        }

        private void btnToggleDesktopIcons_Click(object sender, RoutedEventArgs e)
        {
            bool newVal = !Manager.DesktopIconsEnabled;
            DesktopHelper.ShowIcons(newVal);
            Manager.DesktopIconsEnabled = newVal;
        }

        private void btnCreateStartup_Click(object sender, RoutedEventArgs e)
        {
            ShortcutHelper.CreateStartupShortcut();
        }

        private void btnDeleteStartup_Click(object sender, RoutedEventArgs e)
        {
            ShortcutHelper.DeleteStartupShortcut();
        }

        private void btnSaveConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Manager.SaveTo(Manager.DefaultLocation);

            watch.Stop();
            tbcConfigurationMessage.Text = String.Format(Resource.Get("Edit.SavingTimeFormat"), watch.ElapsedMilliseconds);
        }

        private void btnLoadConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Manager.LoadFrom(Manager.DefaultLocation);

            watch.Stop();
            tbcConfigurationMessage.Text = String.Format(Resource.Get("Edit.LoadingTimeFormat"), watch.ElapsedMilliseconds);
        }

        private void btnCopyConfiguration_Click(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.CopyFiles(Manager.GetFullFilePath());
            tbcConfigurationMessage.Text = Resource.Get("Edit.Copied");
        }

        private void btnReplaceConfiguration_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Filter = "WindowsDock Resource file (*.resx)|WindowsDock.Settings.resx";
            file.Title = Resource.Get("Edit.ResourceFile");
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Manager.Replace(file.FileName);
            }
        }

        private void lvwShortcuts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenCurrentShortcut();
        }

        private void lvwShortcuts_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    OpenCurrentShortcut();
                    break;
                case Key.Delete:
                    DeleteCurrentShortcut();
                    break;
                case Key.Up:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        MoveUpCurrentShortcut();
                        e.Handled = true;
                    }
                    break;
                case Key.Down:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        MoveDownCurrentShortcut();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void tbxAlignOffset_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Up)
                return;

            TextBox source = sender as TextBox;
            if (source != null)
            {
                int add = e.Key == Key.LeftShift ? 10 : 1;

                if (e.Key == Key.Up)
                    Manager.AlignOffset += add;
                else if (e.Key == Key.Down)
                    Manager.AlignOffset -= add;
            }
        }

        private void btnPickBackground_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Manager.Background = String.Format(dialog.Color.IsKnownColor ? "{0}" : "#{0}", dialog.Color.Name);
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = Shortcuts.PermitedKeys.Contains((Key)e.Item);
        }

        private void btnPickBorder_Click(object sender, RoutedEventArgs e)
        {
            string color = PickColor();
            if (color != null)
                Manager.BorderColor = color;
        }

        private void btnApplyActivation_Click(object sender, RoutedEventArgs e)
        {
            Key newKey = (Key)coxActivationKey.SelectedItem;
            if (MainWindow.Helper.RegisterActivationHotkey(newKey))
                Manager.ActivationKey = newKey;
        }

        private void btnDev_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnPickAppButtonColor_Click(object sender, RoutedEventArgs e)
        {
            string color = PickColor();
            if (color != null)
                Manager.BorderColor = color;
        }
    }
}
