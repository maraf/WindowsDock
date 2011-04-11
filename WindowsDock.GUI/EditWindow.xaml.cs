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

        public IManager Manager { get { return manager; } protected set { manager = value; } }

        public EditWindow(IManager manager)
        {
            InitializeComponent();

            Manager = manager;

            DataContext = Manager;

            expShortcuts.IsExpanded = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        protected void OpenDetail(Shortcut shortcut)
        {
            EditShortcutWindow edit = new EditShortcutWindow(shortcut);
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

        private void lvwShortcuts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stpEditButtons.IsEnabled = lvwShortcuts.SelectedItem != null;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lvwShortcuts.SelectedItem != null)
            {
                isNew = false;
                OpenDetail(lvwShortcuts.SelectedItem as Shortcut);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Manager.Shortcuts.Remove(lvwShortcuts.SelectedItem as Shortcut);
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (lvwShortcuts.SelectedIndex != 0)
                lvwShortcuts.SelectedItem = Manager.Shortcuts.Swap(lvwShortcuts.SelectedItem as Shortcut, lvwShortcuts.Items[lvwShortcuts.SelectedIndex - 1] as Shortcut);
            lvwShortcuts.ScrollIntoView(lvwShortcuts.SelectedItem);
            lvwShortcuts.Focus();
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (lvwShortcuts.SelectedIndex != lvwShortcuts.Items.Count - 1)
                lvwShortcuts.SelectedItem = Manager.Shortcuts.Swap(lvwShortcuts.SelectedItem as Shortcut, lvwShortcuts.Items[lvwShortcuts.SelectedIndex + 1] as Shortcut);
            lvwShortcuts.ScrollIntoView(lvwShortcuts.SelectedItem);
            lvwShortcuts.Focus();
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
            if (sender as Expander != expShortcuts)
                expShortcuts.IsExpanded = false;
            if (sender as Expander != expExtensions)
                expExtensions.IsExpanded = false;
            if (sender as Expander != expComands)
                expComands.IsExpanded = false;
            if (sender as Expander != expMiscelaneous)
                expMiscelaneous.IsExpanded = false;
            if (sender as Expander != expRestore)
                expRestore.IsExpanded = false;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Filter = "WAV file (*.wav)|*.wav";
            file.Title = "Select alarm sound file";
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Manager.AlarmSound = file.FileName;
            }
        }

        private void btnRestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            Manager.Restore(cbxShotcuts.IsChecked.Value, cbxTextNotes.IsChecked.Value, cbxScripts.IsChecked.Value, cbxSettings.IsChecked.Value);
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
            tbcConfigurationMessage.Text = String.Format("Saving took {0}ms", watch.ElapsedMilliseconds);
        }

        private void btnLoadConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Manager.LoadFrom(Manager.DefaultLocation);

            watch.Stop();
            tbcConfigurationMessage.Text = String.Format("Loading took {0}ms", watch.ElapsedMilliseconds);
        }

        private void btnCopyConfiguration_Click(object sender, RoutedEventArgs e)
        {
            StringCollection collection = new StringCollection();
            collection.Add(Manager.GetFullFilePath());
            Clipboard.SetFileDropList(collection);
            tbcConfigurationMessage.Text = "File copied.";
        }

        private void btnReplaceConfiguration_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Filter = "WindowsDock Resource file (*.resx)|WindowsDock.Settings.resx";
            file.Title = "Select existing WindowsDock Resource file";
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Manager.Replace(file.FileName);
            }
        }
    }
}
