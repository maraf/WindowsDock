using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesktopCore;

namespace WindowsDock.Core
{
    [Serializable]
    public class TextNotes : ObservableCollection<TextNote>
    {
    }

    [Serializable]
    public class TextNote : NotifyPropertyChanged
    {
        private string header;
        private string content;
        private DateTime modified;
        private DateTime? alarm = null;
        private bool isAlarming = false;

        public TextNote()
        {
            Modified = DateTime.Now;
        }

        public TextNote(string header)
            : this()
        {
            Header = header;
        }

        public TextNote(string header, string content)
            : this(header)
        {
            Content = content;
        }

        public TextNote(string header, string content, DateTime modified)
            : this(header, content)
        {
            Modified = modified;
        }

        public TextNote(string header, string content, DateTime modified, DateTime? alarm)
            : this(header, content, modified)
        {
            Alarm = alarm;
        }

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                Modified = DateTime.Now;
                FirePropertyChanged("Header");
            }
        }

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                Modified = DateTime.Now;
                FirePropertyChanged("Content");
            }
        }

        public DateTime Modified
        {
            get { return modified; }
            set
            {
                modified = value;
                FirePropertyChanged("Modified");
            }
        }

        public DateTime? Alarm
        {
            get { return alarm; }
            set
            {
                alarm = value;
                Modified = DateTime.Now;
                IsAlarming = false;
                FirePropertyChanged("Alarm");
            }
        }

        public bool IsAlarming
        {
            get { return isAlarming; }
            set
            {
                isAlarming = value;
                FirePropertyChanged("IsAlarming");
            }
        }

        public bool IsNotAlarming
        {
            get { return !isAlarming; }
        }
    }
}
