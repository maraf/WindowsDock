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
using System.Globalization;
using System.Threading;
using DesktopCore;

namespace WindowsDock.Core
{
    public class UnableToLoadConfigurationException : Exception {
        public UnableToLoadConfigurationException(Exception e) : base(Resource.Get("Error.UnableToLoadConfig"), e) { }
    }

    public interface IManager
    {
        Shortcuts Shortcuts { get; set; }

        TextNotes TextNotes { get; set; }

        Scripts Scripts { get; set; }

        BrowseItems BrowseItems { get; set; }

        Commands Commands { get; set; }

        DesktopItems DesktopItems { get; set; }

        WindowPosition Position { get; set; }

        bool DockWindow { get; set; }

        WindowAlign Align { get; set; }

        int AlignOffset { get; set; }

        int CornerRadius { get; set; }

        TimeSpan HideDuration { get; set; }

        TimeSpan HideDelay { get; set; }

        double Opacity { get; set; }

        string Background { get; set; }

        int BorderThickness { get; set; }

        string BorderColor { get; set; }

        string AppButtonColor { get; set; }

        bool AutoHiding { get; set; }

        bool StartHidden { get; set; }

        int TaskbarHeight { get; set; }

        bool UseTaskBarHeightWhenBottom { get; set; }

        int HiddenOffset { get; set; }

        bool TextNotesEnabled { get; set; }

        bool ScriptsEnabled { get; set; }

        bool BrowserEnabled { get; set; }

        bool DesktopEnabled { get; set; }

        string AlarmSound { get; set; }

        bool DesktopIconsEnabled { get; set; }

        CultureInfo Locale { get; set; }

        bool ShowConfigButton { get; set; }

        bool ShowCloseButton { get; set; }

        bool ShowShortcutBubble { get; set; }

        int ShortcutBubbleFontSize { get; set; }

        int ShortcutIconSize { get; set; }

        Key ActivationKey { get; set; }
        Key ConfigKey { get; set; }
        Key CloseKey { get; set; }
        Key TextNotesKey { get; set; }
        Key ScriptsKey { get; set; }
        Key FolderBrowserKey { get; set; }
        Key DesktopBrowserKey { get; set; }
        Key DesktopExplorerKey { get; set; }


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
        private WindowPosition position = WindowPosition.Top;
        private bool dockWindow = false;
        private WindowAlign align = WindowAlign.Center;
        private int alignOffset = 0;
        private int cornerRadius = 10;
        private TimeSpan hideDuration = TimeSpan.Parse("0:0:0.125");
        private TimeSpan hideDelay = TimeSpan.Parse("0:0:0.25");
        private double opacity = 0.5;
        private string background = "#FFF0FFFF";
        private int borderThickness = 0;
        private string borderColor = "#FFFFFFFF";
        private string appButtonColor = "#80000000";
        private bool autoHiding = true;
        private bool startHidden = false;
        private int taskbarHeight = 40;
        private bool useTaskBarHeightWhenBottom = false;
        private int hiddenOffset = 2;
        private bool textNotesEnabled = true;
        private bool scriptsEnabled = true;
        private bool browserEnabled = true;
        private bool desktopEnabled = true;
        private string alarmSound = "Sounds/alarm.wav";
        private bool desktopIconsEnabled = true;
        private CultureInfo locale = Thread.CurrentThread.CurrentCulture;
        private bool showConfigButton = true;
        private bool showCloseButton = true;
        private bool showShortcutBubble = true;
        private int shortcutBubbleFontSize = 8;
        private int shortcutIconSize = 32;

        private Key activationKey = Key.W;
        private Key configKey = Key.Z;
        private Key closeKey = Key.X;
        private Key textNotesKey = Key.T;
        private Key scriptsKey = Key.S;
        private Key folderBrowserKey = Key.B;
        private Key desktopBrowserKey = Key.D;
        private Key desktopExplorerKey = Key.None;

        public Shortcuts Shortcuts { get { return shortcuts; } set { shortcuts = value; } }

        public TextNotes TextNotes { get { return textNotes; } set { textNotes = value; } }

        public Scripts Scripts { get { return scripts; } set { scripts = value; } }

        public BrowseItems BrowseItems { get { return browseItems; } set { browseItems = value; } }

        public Commands Commands { get { return commands; } set { commands = value; } }

        public DesktopItems DesktopItems { get { return desktopItems; } set { desktopItems = value; } }

        [Config("Position")]
        public WindowPosition Position
        {
            get { return position; }
            set
            {
                position = value;
                FirePropertyChanged("Position");
            }
        }

        [Config("DockWindow")]
        public bool DockWindow
        {
            get { return dockWindow; }
            set
            {
                dockWindow = value;
                FirePropertyChanged("DockWindow");
            }
        }

        [Config("Align")]
        public WindowAlign Align
        {
            get { return align; }
            set
            {
                align = value;
                FirePropertyChanged("Align");
            }
        }

        [Config("AlignOffset")]
        public int AlignOffset
        {
            get { return alignOffset; }
            set
            {
                alignOffset = value;
                FirePropertyChanged("AlignOffset");
            }
        }

        [Config("CornerRadius")]
        public int CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                FirePropertyChanged("CornerRadius");
            }
        }

        [Config("HideDuration")]
        public TimeSpan HideDuration
        {
            get { return hideDuration; }
            set
            {
                hideDuration = value;
                FirePropertyChanged("HideDuration");
            }
        }

        [Config("HideDelay")]
        public TimeSpan HideDelay
        {
            get { return hideDelay; }
            set
            {
                hideDelay = value;
                FirePropertyChanged("HideDelay");
            }
        }

        [Config("Opacity")]
        public double Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                FirePropertyChanged("Opacity");
            }
        }

        [Config("Background")]
        public string Background
        {
            get { return background; }
            set
            {
                background = value;
                FirePropertyChanged("Background");
            }
        }

        [Config("BorderThickness")]
        public int BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                FirePropertyChanged("BorderThickness");
            }
        }

        [Config("BorderColor")]
        public string BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                FirePropertyChanged("BorderColor");
            }
        }

        [Config("AppButtonColor")]
        public string AppButtonColor
        {
            get { return appButtonColor; }
            set
            {
                appButtonColor = value;
                FirePropertyChanged("AppButtonColor");
            }
        }

        [Config("AutoHiding")]
        public bool AutoHiding
        {
            get { return autoHiding; }
            set
            {
                autoHiding = value;
                FirePropertyChanged("AutoHiding");
            }
        }

        [Config("StartHidden")]
        public bool StartHidden
        {
            get { return startHidden; }
            set
            {
                startHidden = value;
                FirePropertyChanged("StartHidden");
            }
        }

        [Config("TaskbarHeight")]
        public int TaskbarHeight
        {
            get { return taskbarHeight; }
            set
            {
                taskbarHeight = value;
                FirePropertyChanged("TaskbarHeight");
            }
        }

        [Config("UseTaskBarHeightWhenBottom")]
        public bool UseTaskBarHeightWhenBottom
        {
            get { return useTaskBarHeightWhenBottom; }
            set
            {
                useTaskBarHeightWhenBottom = value;
                FirePropertyChanged("UseTaskBarHeightWhenBottom");
            }
        }

        [Config("HiddenOffset")]
        public int HiddenOffset
        {
            get { return hiddenOffset; }
            set
            {
                hiddenOffset = value;
                FirePropertyChanged("HiddenOffset");
            }
        }

        [Config("TextNotesEnabled")]
        public bool TextNotesEnabled
        {
            get { return textNotesEnabled; }
            set
            {
                textNotesEnabled = value;
                FirePropertyChanged("TextNotesEnabled");
            }
        }

        [Config("ScriptsEnabled")]
        public bool ScriptsEnabled
        {
            get { return scriptsEnabled; }
            set
            {
                scriptsEnabled = value;
                FirePropertyChanged("ScriptsEnabled");
            }
        }

        [Config("BrowserEnabled")]
        public bool BrowserEnabled
        {
            get { return browserEnabled; }
            set
            {
                browserEnabled = value;
                FirePropertyChanged("BrowserEnabled");
            }
        }

        [Config("DesktopEnabled")]
        public bool DesktopEnabled
        {
            get { return desktopEnabled; }
            set
            {
                desktopEnabled = value;
                FirePropertyChanged("DesktopEnabled");
            }
        }

        [Config("AlarmSound")]
        public string AlarmSound
        {
            get { return alarmSound; }
            set
            {
                alarmSound = value;
                FirePropertyChanged("AlarmSound");
            }
        }

        [Config("DesktopIconsEnabled")]
        public bool DesktopIconsEnabled
        {
            get { return desktopIconsEnabled; }
            set
            {
                desktopIconsEnabled = value;
                FirePropertyChanged("DesktopIconsEnabled");
            }
        }

        [Config("Locale")]
        public CultureInfo Locale
        {
            get { return locale; }
            set
            {
                locale = value;
                FirePropertyChanged("Locale");
            }
        }

        [Config("ShowConfigButton")]
        public bool ShowConfigButton
        {
            get { return showConfigButton; }
            set
            {
                showConfigButton = value;
                FirePropertyChanged("ShowConfigButton");
            }
        }

        [Config("ShowCloseButton")]
        public bool ShowCloseButton
        {
            get { return showCloseButton; }
            set
            {
                showCloseButton = value;
                FirePropertyChanged("ShowCloseButton");
            }
        }

        [Config("ShowShortcutBubble")]
        public bool ShowShortcutBubble
        {
            get { return showShortcutBubble; }
            set
            {
                showShortcutBubble = value;
                FirePropertyChanged("ShowShortcutBubble");
            }
        }

        [Config("ShortcutBubbleFontSize")]
        public int ShortcutBubbleFontSize
        {
            get { return shortcutBubbleFontSize; }
            set
            {
                shortcutBubbleFontSize = value;
                FirePropertyChanged("ShortcutBubbleFontSize");
            }
        }

        [Config("ShortcutIconSize")]
        public int ShortcutIconSize
        {
            get { return shortcutIconSize; }
            set
            {
                shortcutIconSize = value;
                FirePropertyChanged("ShortcutIconSize");
            }
        }

        #region Keys

        [Config("ActivationKey")]
        public Key ActivationKey
        {
            get { return activationKey; }
            set
            {
                activationKey = value;
                FirePropertyChanged("ActivationKey");
            }
        }

        [Config("ConfigKey")]
        public Key ConfigKey
        {
            get { return configKey; }
            set
            {
                configKey = value;
                FirePropertyChanged("ConfigKey");
            }
        }

        [Config("CloseKey")]
        public Key CloseKey
        {
            get { return closeKey; }
            set
            {
                closeKey = value;
                FirePropertyChanged("CloseKey");
            }
        }

        [Config("TextNotesKey")]
        public Key TextNotesKey
        {
            get { return textNotesKey; }
            set
            {
                textNotesKey = value;
                FirePropertyChanged("TextNotesKey");
            }
        }

        [Config("ScriptsKey")]
        public Key ScriptsKey
        {
            get { return scriptsKey; }
            set
            {
                scriptsKey = value;
                FirePropertyChanged("ScriptsKey");
            }
        }

        [Config("FolderBrowserKey")]
        public Key FolderBrowserKey
        {
            get { return folderBrowserKey; }
            set
            {
                folderBrowserKey = value;
                FirePropertyChanged("FolderBrowserKey");
            }
        }

        [Config("DesktopBrowserKey")]
        public Key DesktopBrowserKey
        {
            get { return desktopBrowserKey; }
            set
            {
                desktopBrowserKey = value;
                FirePropertyChanged("DesktopBrowserKey");
            }
        }

        [Config("DesktopExplorerKey")]
        public Key DesktopExplorerKey
        {
            get { return desktopExplorerKey; }
            set
            {
                desktopExplorerKey = value;
                FirePropertyChanged("DesktopExplorerKey");
            }
        }

        #endregion

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

                IEnumerable<ConfigInfo> properties = ConfigAttribute.GetProperties(GetType());
                foreach (ConfigInfo info in properties)
                {
                    rw.AddResource(info.Attribute.Name, info.Property.GetValue(this, null));
                }
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
                        IEnumerable<ConfigInfo> properties = ConfigAttribute.GetProperties(GetType());
                        IDictionaryEnumerator en = rr.GetEnumerator();
                        while (en.MoveNext())
                        {
                            bool set = false;
                            foreach (ConfigInfo info in properties)
                            {
                                if (info.Attribute.Name == (en.Key as string))
                                {
                                    info.Property.SetValue(this, en.Value, null);
                                    set = true;
                                    break;
                                }
                            }
                            if (set)
                                continue;

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
                            else if (key.Equals("Locale"))
                            {
                                Thread.CurrentThread.CurrentCulture = Locale;
                            }
                        }
                    }
                }
                catch (XmlException e)
                {
                    throw new UnableToLoadConfigurationException(e);
                }
            }

            Resource.Load("Resources/Resources");
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
                StartHidden = newManager.StartHidden;
                HiddenOffset = newManager.HiddenOffset;
                CornerRadius = newManager.CornerRadius;
                TextNotesEnabled = newManager.TextNotesEnabled;
                ScriptsEnabled = newManager.ScriptsEnabled;
                BrowserEnabled = newManager.BrowserEnabled;
                DesktopEnabled = newManager.DesktopEnabled;
                AlarmSound = newManager.AlarmSound;
                DesktopIconsEnabled = newManager.DesktopIconsEnabled;
                Locale = newManager.Locale;

                ActivationKey = newManager.ActivationKey;
                ConfigKey = newManager.ConfigKey;
                CloseKey = newManager.CloseKey;
                TextNotesKey = newManager.TextNotesKey;
                ScriptsKey = newManager.ScriptsKey;
                FolderBrowserKey = newManager.FolderBrowserKey;
                DesktopBrowserKey = newManager.DesktopBrowserKey;
            }
        }
    }

    #region IO

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

    #endregion
}
