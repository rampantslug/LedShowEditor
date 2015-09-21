using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.Display.LedTree;
using LedShowEditor.Display.Playfield;
using LedShowEditor.Display.Properties;
using LedShowEditor.Display.ShowsList;
using LedShowEditor.Display.Timeline;
using LedShowEditor.Display.Tools;
using LedShowEditor.ViewModels;
using Microsoft.Win32;
using Configuration = LedShowEditor.Config.Configuration;

namespace LedShowEditor
{

    /// <summary>
    /// 
    /// </summary>
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShell
    {

        public string ConfigName
        {
            get
            {
                return _configName;
            }
            set
            {
                _configName = value;
                NotifyOfPropertyChange(() => ConfigName);
            }
        }

        public bool IsConfigLoaded
        {
            get
            {
                return _isConfigLoaded;
            }
            set
            {
                _isConfigLoaded = value;
                NotifyOfPropertyChange(() => IsConfigLoaded);
            }
        }

        // Display Modules
        public ILedTree LedTree { get; private set; }    
        public IPlayfield Playfield { get; private set; }
        public IProperties Properties { get; private set; }
        public IShowsList ShowList { get; private set; }
        public ITimeline Timeline { get; private set; }
        public ITools Tools { get; private set; }

        public BindableCollection<IScreen> LeftTabs
        {
            get
            {
                return _leftTabs;
            }
            set
            {
                _leftTabs = value;
                NotifyOfPropertyChange(() => LeftTabs);
            }
        }

        public BindableCollection<IScreen> RightTabs
        {
            get
            {
                return _rightTabs;
            }
            set
            {
                _rightTabs = value;
                NotifyOfPropertyChange(() => RightTabs);
            }
        }

        /// <summary>
        /// Constructor for the ShellViewModel. Main container for Client application elements.
        /// Imports required UI elements via DI.
        /// </summary> 
        [ImportingConstructor]
        public ShellViewModel(
            IEventAggregator eventAggregator,
            ILeds ledsViewModel,
            ILedTree ledTree,
            IPlayfield playfield,
            IProperties properties,
            IShowsList showList,
            ITools tools,
            ITimeline timeline
            )
        {
            _eventAggregator = eventAggregator;
            _ledsViewModel = ledsViewModel;
            LedTree = ledTree;
            Playfield = playfield;
            Properties = properties;
            ShowList = showList;
            Timeline = timeline;
            Tools = tools;

            LeftTabs = new BindableCollection<IScreen>();
            RightTabs = new BindableCollection<IScreen>();

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            DisplayName = "Led Show Editor";

            IsConfigLoaded = false;
        }

        


        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);

            LeftTabs.Add(LedTree);
            LeftTabs.Add(ShowList);

            RightTabs.Add(Properties);
            RightTabs.Add(Tools);

            var firstRun = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("FirstRun"));
            if (firstRun)
            {
                var exampleConfig = Directory.GetCurrentDirectory() + @"\IndyExample\IndyConfig.json";
                LoadConfig(exampleConfig);  
                UpdateSetting("FirstRun", "False");
            }
            else
            {
                _lastConfigFile = ConfigurationManager.AppSettings.Get("LastConfig");

                // If we can load the last config then also load any associated shows
                LoadConfig(_lastConfigFile);
            }  
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            SaveConfig();
        }

        public void NewConfig()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "JSON File (*.json)|*.json",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _lastConfigFile = saveFileDialog.FileName;

                _ledsViewModel.WorkingDirectory = Path.GetDirectoryName(_lastConfigFile) + @"\";

                Directory.CreateDirectory(_ledsViewModel.WorkingDirectory + @"LedShows\");

                var config = new Configuration();
                config.ToFile(_lastConfigFile);
                
                LoadConfig(_lastConfigFile);
            }
        }

        public void LoadExistingConfig()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON File (*.json)|*.json",                
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadConfig(openFileDialog.FileName);
            }
        }

        public void SaveCurrentConfig()
        {
            SaveConfig();
        }

        public void LoadConfig(string fullConfigFilename) 
        {
            // Clear Playfield image
            Playfield.ClearImage();

            if (string.IsNullOrEmpty(fullConfigFilename) || !File.Exists(fullConfigFilename))
            {
                return; // Need to put error message about file not found?
            }
            var gameConfiguration = Configuration.FromFile(fullConfigFilename);          
            if (gameConfiguration == null)
            {
                return; // Need some kind of error message that data is corrupt?
            }
            _ledsViewModel.WorkingDirectory = Path.GetDirectoryName(fullConfigFilename);

            // Update local information from configuration
            Playfield.UpdateImageLocation(_ledsViewModel.WorkingDirectory);
            Playfield.UpdateImage(_ledsViewModel.WorkingDirectory + gameConfiguration.PlayfieldImage);
 
            _ledsViewModel.LoadLedsFromConfig(gameConfiguration.Leds);
            _ledsViewModel.LoadGroupsFromConfig(gameConfiguration.Groups);

            // If loaded ok then set default load file to be this...
            _lastConfigFile = fullConfigFilename;
            UpdateSetting("LastConfig", _lastConfigFile);
            ConfigName = Path.GetFileNameWithoutExtension(_lastConfigFile);

            LoadShows(_ledsViewModel.WorkingDirectory);

            IsConfigLoaded = true;
        }

        public void SaveConfig()
        {
            if (string.IsNullOrEmpty(_lastConfigFile) || !File.Exists(_lastConfigFile))
            {
                return;
            }

            var config = new Configuration
            {
                Leds = _ledsViewModel.GetLedsAsConfigs(),
                Groups = _ledsViewModel.GetGroupsAsConfigs(),
                PlayfieldImage = Playfield.PlayfieldImageName
            };
            config.ToFile(_lastConfigFile);

            SaveLedShows();
        }

        private void SaveLedShows()
        {
            foreach (var showViewModel in _ledsViewModel.Shows)
            {
                var showConfig = _ledsViewModel.GetShowAsConfig(showViewModel);
                var path = _ledsViewModel.WorkingDirectory + @"LedShows\";
                showConfig.ToFile(path + showViewModel.Name + ".json");
            }
        }

        private void LoadShows(string configLocation)
        {
            var path = configLocation + @"LedShows\";

            if(!Directory.Exists(path))
            {
                return;
            }

            _ledsViewModel.Shows.Clear();
            var ledShows = Directory.GetFiles(path, "*.json");
            foreach (var file in ledShows)
            {
                if (File.Exists(file))
                {
                    var showConfig = ShowConfig.FromFile(file);
                    var showName = Path.GetFileNameWithoutExtension(file);
                    
                    _ledsViewModel.LoadShowFromConfig(showConfig, showName);
                }
            }
        }

        
    

    private static void UpdateSetting(string key, string value)
    {
        var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        configuration.AppSettings.Settings[key].Value = value;
        configuration.Save();

        ConfigurationManager.RefreshSection("appSettings");
    }


    public void ExecuteGlobalCommand(Key key)
    {
        if (key == Key.Space)
        {
            Timeline.PlayPause();
        }
        else if (key == Key.NumPad0)
        {
            Timeline.FirstFrame();
        }
        else if (key == Key.NumPad1)
        {
            Timeline.LastFrame();
        }
    }



        private readonly IEventAggregator _eventAggregator;
        private readonly ILeds _ledsViewModel;
        private BindableCollection<IScreen> _leftTabs;
        private string _lastConfigFile;        
        private string _configName;
        private bool _isConfigLoaded;
        private BindableCollection<IScreen> _rightTabs;
    }



}