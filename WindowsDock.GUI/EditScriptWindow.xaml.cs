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
    /// Interaction logic for EditTextNoteWindow.xaml
    /// </summary>
    public partial class EditScriptWindow : Window
    {
        private Script script;

        public Script Script { get { return script; } protected set { script = value; } }

        public EditScriptWindow(Script script)
        {
            InitializeComponent();

            Script = script;

            DataContext = Script;

            tbxHeader.Focus();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
            file.FileName = System.IO.Path.GetFileName(Script.Path);
            file.InitialDirectory = System.IO.Path.GetDirectoryName(script.Path);
            file.CheckFileExists = true;
            file.Multiselect = false;
            file.Title = Resource.Get("Script.SelectFile");
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Script.Path = file.FileName;
                if (String.IsNullOrEmpty(Script.WorkingDirectory))
                    Script.WorkingDirectory = System.IO.Path.GetDirectoryName(file.FileName);
            }
        }
    }
}
