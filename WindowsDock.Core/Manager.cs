using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Collections;
using System.Windows.Media;
using System.Xml;
using System.IO.IsolatedStorage;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace WindowsDock.Core
{
    public class UnableToLoadConfigurationException : Exception {
        public UnableToLoadConfigurationException(Exception e) : base("Unable to load configuration file!", e) { }
    }

    public interface IManager
    {
        Shortcuts Shortcuts { get; set; }

        TextNotes TextNotes { get; set; }

        Scripts Scripts { get; set; }

        BrowseItems BrowseItems { get; set; }

        Commands Commands { get; set; }

        DesktopItems DesktopItems { get; set; }

        TimeSpan HideDuration { get; set; }

        TimeSpan HideDelay { get; set; }

        double Opacity { get; set; }

        string Background { get; set; }

        bool AutoHiding { get; set; }

        int HiddenOffset { get; set; }

        bool TextNotesEnabled { get; set; }

        bool ScriptsEnabled { get; set; }

        bool BrowserEnabled { get; set; }

        bool DesktopEnabled { get; set; }

        string AlarmSound { get; set; }

        bool DesktopIconsEnabled { get; set; }


        string DefaultLocation { get; }

        void SaveTo(string path);

        void LoadFrom(string path);

        void Replace(string path);

        string GetFullFilePath();

        void Restore(bool shortcuts, bool textNotes, bool scripts, bool settings);
    }

    public class ManagerFacory
    {
        public static IManager Create() { return new DefaultManager(); }
    }

    public class DefaultManager : NotifyPropertyChanged, IManager
    {
        private Shortcuts shortcuts = new Shortcuts();
        private TextNotes textNotes = new TextNotes();
        private BrowseItems browseItems = new BrowseItems();
        private Commands commands = new Commands();
        private Scripts scripts = new Scripts();
        private DesktopItems desktopItems = new DesktopItems();
        private TimeSpan hideDuration = TimeSpan.Parse("0:0:0.125");
        private TimeSpan hideDelay = TimeSpan.Parse("0:0:0.25");
        private double opacity = 0.5;
        private string background = "#FFF0FFFF";
        private bool autoHiding = true;
        private int hiddenOffset = 2;
        private bool textNotesEnabled = true;
        private bool scriptsEnabled = true;
        private bool browserEnabled = true;
        private bool desktopEnabled = true;
        private string alarmSound = "Sounds/alarm.wav";
        private bool desktopIconsEnabled = true;

        public Shortcuts Shortcuts { get { return shortcuts; } set { shortcuts = value; } }

        public TextNotes TextNotes { get { return textNotes; } set { textNotes = value; } }

        public Scripts Scripts { get { return scripts; } set { scripts = value; } }

        public BrowseItems BrowseItems { get { return browseItems; } set { browseItems = value; } }

        public Commands Commands { get { return commands; } set { commands = value; } }

        public DesktopItems DesktopItems { get { return desktopItems; } set { desktopItems = value; } }

        public TimeSpan HideDuration
        {
            get { return hideDuration; }
            set
            {
                hideDuration = value;
                FirePropertyChanged("HideDuration");
            }
        }

        public TimeSpan HideDelay
        {
            get { return hideDelay; }
            set
            {
                hideDelay = value;
                FirePropertyChanged("HideDelay");
            }
        }

        public double Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                FirePropertyChanged("Opacity");
            }
        }

        public string Background
        {
            get { return background; }
            set
            {
                background = value;
                FirePropertyChanged("Background");
            }
        }

        public bool AutoHiding
        {
            get { return autoHiding; }
            set
            {
                autoHiding = value;
                FirePropertyChanged("AutoHiding");
            }
        }

        public int HiddenOffset
        {
            get { return hiddenOffset; }
            set
            {
                hiddenOffset = value;
                FirePropertyChanged("HiddenOffset");
            }
        }

        public bool TextNotesEnabled
        {
            get { return textNotesEnabled; }
            set
            {
                textNotesEnabled = value;
                FirePropertyChanged("TextNotesEnabled");
            }
        }

        public bool ScriptsEnabled
        {
            get { return scriptsEnabled; }
            set
            {
                scriptsEnabled = value;
                FirePropertyChanged("ScriptsEnabled");
            }
        }

        public bool BrowserEnabled
        {
            get { return browserEnabled; }
            set
            {
                browserEnabled = value;
                FirePropertyChanged("BrowserEnabled");
            }
        }

        public bool DesktopEnabled
        {
            get { return desktopEnabled; }
            set
            {
                desktopEnabled = value;
                FirePropertyChanged("DesktopEnabled");
            }
        }

        public string AlarmSound
        {
            get { return alarmSound; }
            set
            {
                alarmSound = value;
                FirePropertyChanged("AlarmSound");
            }
        }

        public bool DesktopIconsEnabled
        {
            get { return desktopIconsEnabled; }
            set
            {
                desktopIconsEnabled = value;
                FirePropertyChanged("DesktopIconsEnabled");
            }
        }


        public virtual string DefaultLocation { get { return "WindowsDock.Settings.resx"; } }

        public DefaultManager() { }

        public virtual void SaveTo(string path)
        {
            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(DefaultLocation, FileMode.Create, f))
            using (IResourceWriter rw = new ResXResourceWriter(stream))
            {
                int i = 0;
                foreach (Shortcut item in Shortcuts)
                {
                    rw.AddResource(String.Format("Shortcut[{0}]", i), new SimpleShortcut() { Args = item.Args, Path = item.Path, Key = item.Key, WorkingDirectory = item.WorkingDirectory });
                    i++;
                }
                i = 0;
                foreach (TextNote item in TextNotes)
                {
                    rw.AddResource(String.Format("TextNote[{0}]", i), new SimpleTextNote() { Header = item.Header, Content = item.Content, Modified = item.Modified, Alarm = item.Alarm });
                    i++;
                }
                i = 0;
                foreach (Script item in Scripts)
                {
                    rw.AddResource(String.Format("Script[{0}]", i), new SimpleScript() { Header = item.Header, Path = item.Path, WorkingDirectory = item.WorkingDirectory });
                    i++;
                }
                i = 0;
                foreach (Command item in Commands)
                {
                    rw.AddResource(String.Format("Command[{0}]", i), new SimpleCommand() { Name = item.Name, Path = item.Path, Args = item.Args });
                    i++;
                }
                rw.AddResource("CommandDefaultIndex", Commands.DefaultIndex);
                rw.AddResource("HideDuration", HideDuration);
                rw.AddResource("Opacity", Opacity);
                rw.AddResource("Background", Background);
                rw.AddResource("AutoHiding", AutoHiding);
                rw.AddResource("HiddenOffset", HiddenOffset);
                rw.AddResource("TextNotesEnabled", TextNotesEnabled);
                rw.AddResource("ScriptsEnabled", ScriptsEnabled);
                rw.AddResource("BrowserEnabled", BrowserEnabled);
                rw.AddResource("DesktopEnabled", DesktopEnabled);
                rw.AddResource("AlarmSound", AlarmSound);
                rw.AddResource("DesktopIconsEnabled", DesktopIconsEnabled);
            }
        }

        public virtual void LoadFrom(string path)
        {
            Shortcuts.Clear();
            TextNotes.Clear();
            Commands.Clear();
            Commands.Clear();
            Scripts.Clear();

            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            if (f.FileExists(DefaultLocation))
            {
                try
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(DefaultLocation, FileMode.OpenOrCreate, f))
                    using (IResourceReader rr = new ResXResourceReader(stream))
                    {
                        IDictionaryEnumerator en = rr.GetEnumerator();
                        while (en.MoveNext())
                        {
                            string key = (string)en.Key;
                            if (key.Contains("Shortcut["))
                            {
                                SimpleShortcut ss = (SimpleShortcut)en.Value;
                                if (File.Exists(ss.Path))
                                    Shortcuts.Add(new Shortcut(ss.Path) { Args = ss.Args, Key = ss.Key, WorkingDirectory = ss.WorkingDirectory });
                            }
                            else if (key.Contains("TextNote["))
                            {
                                SimpleTextNote stn = (SimpleTextNote)en.Value;
                                TextNotes.Add(new TextNote(stn.Header, stn.Content, stn.Modified, stn.Alarm));
                            }
                            else if (key.Contains("Script["))
                            {
                                SimpleScript ss = (SimpleScript)en.Value;
                                Scripts.Add(new Script(ss.Header, ss.Path, ss.WorkingDirectory));
                            }
                            else if (key.Contains("Command["))
                            {
                                SimpleCommand sc = (SimpleCommand)en.Value;
                                Commands.Add(new Command(sc.Name, sc.Path, sc.Args));
                            }
                            else if (key.Contains("CommandDefaultIndex"))
                            {
                                Commands.DefaultIndex = (int)en.Value;
                            }
                            else if (key.Equals("HideDuration"))
                            {
                                HideDuration = (TimeSpan)en.Value;
                            }
                            else if (key.Equals("Opacity"))
                            {
                                Opacity = (double)en.Value;
                            }
                            else if (key.Equals("Background"))
                            {
                                Background = (string)en.Value;
                            }
                            else if (key.Equals("AutoHiding"))
                            {
                                AutoHiding = (bool)en.Value;
                            }
                            else if (key.Equals("HiddenOffset"))
                            {
                                HiddenOffset = (int)en.Value;
                            }
                            else if (key.Equals("TextNotesEnabled"))
                            {
                                TextNotesEnabled = (bool)en.Value;
                            }
                            else if (key.Equals("ScriptsEnabled"))
                            {
                                ScriptsEnabled = (bool)en.Value;
                            }
                            else if (key.Equals("BrowserEnabled"))
                            {
                                BrowserEnabled = (bool)en.Value;
                            }
                            else if (key.Equals("DesktopEnabled"))
                            {
                                DesktopEnabled = (bool)en.Value;
                            }
                            else if (key.Equals("AlarmSound"))
                            {
                                AlarmSound = (string)en.Value;
                            }
                            else if (key.Equals("DesktopIconsEnabled"))
                            {
                                DesktopIconsEnabled = (bool)en.Value;
                            }
                        }
                    }
                }
                catch (XmlException e)
                {
                    throw new UnableToLoadConfigurationException(e);
                }
            }
        }

        public void Replace(string path)
        {
            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            
            using (StreamReader source = new StreamReader(File.OpenRead(path)))
            using (StreamWriter dest = new StreamWriter(new IsolatedStorageFileStream(DefaultLocation, FileMode.Create, f)))
            {
                dest.Write(source.ReadToEnd());
            }
            LoadFrom(DefaultLocation);
        }

        public string GetFullFilePath()
        {
            string filePath = DefaultLocation;
            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            using (IsolatedStorageFileStream oStream = new IsolatedStorageFileStream(DefaultLocation, FileMode.Open, f))
            {
                filePath = oStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(oStream).ToString();
            }
            return filePath;
        }

        public void Restore(bool shortcuts, bool textNotes, bool scripts, bool settings)
        {
            IManager newManager = ManagerFacory.Create();

            if (shortcuts)
            {
                Shortcuts = newManager.Shortcuts;
            }
            if (textNotes)
            {
                TextNotes = newManager.TextNotes;
            }
            if (scripts)
            {
                Scripts = newManager.Scripts;
            }
            if (settings)
            {
                HideDuration = newManager.HideDuration;
                HideDelay = newManager.HideDelay;
                Opacity = newManager.Opacity;
                Background = newManager.Background;
                AutoHiding = newManager.AutoHiding;
                TextNotesEnabled = newManager.TextNotesEnabled;
                ScriptsEnabled = newManager.ScriptsEnabled;
                BrowserEnabled = newManager.BrowserEnabled;
                DesktopEnabled = newManager.DesktopEnabled;
                AlarmSound = newManager.AlarmSound;
            }
        }
    }

    [Serializable]
    struct SimpleShortcut
    {
        public string Path;
        public string Args;
        public string WorkingDirectory;
        public Key Key;
    }

    [Serializable]
    struct SimpleTextNote
    {
        public string Header;
        public string Content;
        public DateTime Modified;
        public DateTime? Alarm;
    }

    [Serializable]
    struct SimpleScript
    {
        public string Header;
        public string Path;
        public string WorkingDirectory;
    }

    [Serializable]
    struct SimpleCommand
    {
        public string Name;
        public string Path;
        public string Args;
    }
}
