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
using System.Windows.Shapes;
using WindowsDock.Core;
using DesktopCore;

namespace WindowsDock.GUI
{
    /// <summary>
    /// Interaction logic for EditShortcutWindow.xaml
    /// </summary>
    public partial class EditShortcutWindow : Window
    {
        private Shortcut shortcut;

        public Shortcut Shortcut { get { return shortcut; } protected set { shortcut = value; } }

        public EditShortcutWindow(Shortcut shortcut)
        {
            InitializeComponent();

            Shortcut = shortcut;

            DataContext = shortcut;

            btnBrowse.Focus();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Title = Resource.Get("Shortcut.SelectFile");
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Shortcut.Path = file.FileName;
                if (String.IsNullOrEmpty(Shortcut.WorkingDirectory))
                    Shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(file.FileName);
            }
        }
    }
}
