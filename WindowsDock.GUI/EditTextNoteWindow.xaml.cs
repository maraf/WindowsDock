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

namespace WindowsDock.GUI
{
    /// <summary>
    /// Interaction logic for EditTextNoteWindow.xaml
    /// </summary>
    public partial class EditTextNoteWindow : Window
    {
        private TextNote textNote;

        public TextNote TextNote { get { return textNote; } protected set { textNote = value; } }

        public EditTextNoteWindow(TextNote textNote)
        {
            InitializeComponent();

            TextNote = textNote;

            if (TextNote.Alarm != null)
                cbxAlarm.IsChecked = true;

            DataContext = TextNote;

            tbxHeader.Focus();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        private void cbxAlarm_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxAlarm.IsChecked.Value)
            {
                if(TextNote.Alarm == null)
                    TextNote.Alarm = DateTime.Now.AddHours(1);
                tbxAlarm.SelectionStart = 0;
                tbxAlarm.SelectionLength = tbxAlarm.Text.Length;
                tbxAlarm.Focus();
            }
            else
            {
                TextNote.Alarm = null;
                TextNote.IsAlarming = false;
            }
        }
    }
}
