using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Reflection;
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

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            DisplayName = "Led Show Editor";
        }



        protected override void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);
            LeftTabs.Add(LedTree);
            LeftTabs.Add(ShowList);

            _lastConfigFile = ConfigurationManager.AppSettings.Get("LastConfig");

            // If we can load the last config then also load any associated shows
            if (LoadConfig(_lastConfigFile))
            {
                LoadShows(_configDirectory);               
            }
            else
            {
                // Create blank option
                //var path = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
               // _lastConfigFile = path + @"\playfieldConfig"
            }
            
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            SaveConfig();
        }

        public void LoadExistingConfig()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON File (*.json)|*.json",              
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadConfig(openFileDialog.FileName);
            }
        }



        public bool LoadConfig(string fullConfigFilename) 
        {
            if (string.IsNullOrEmpty(fullConfigFilename) || !File.Exists(fullConfigFilename))
            {
                return false;
            }
            var gameConfiguration = Configuration.FromFile(fullConfigFilename);
            _configDirectory = Path.GetDirectoryName(fullConfigFilename);

            // Update local information from configuration
            Playfield.PlayfieldImagePath = _configDirectory + gameConfiguration.PlayfieldImage;

            _ledsViewModel.LoadLedsFromConfig(gameConfiguration.Leds);
            _ledsViewModel.LoadGroupsFromConfig(gameConfiguration.Groups);

            // If loaded ok then set default load file to be this...
            _lastConfigFile = fullConfigFilename;
            ConfigurationManager.AppSettings.Set("LastConfig", _lastConfigFile);
            ConfigName = Path.GetFileNameWithoutExtension(_lastConfigFile);

            return true;
        }

        public void SaveConfig()
        {
            if (string.IsNullOrEmpty(_lastConfigFile) || !File.Exists(_lastConfigFile))
            {
                // Prompt user for a filename save location
            }

            var playfieldImage = Path.GetFileName(Playfield.PlayfieldImagePath);
            var config = new Configuration
            {
                Leds = _ledsViewModel.GetLedsAsConfigs(),
                Groups = _ledsViewModel.GetGroupsAsConfigs(),
                PlayfieldImage = playfieldImage
            };
            config.ToFile(_lastConfigFile);

            SaveLedShows();
        }

        private void SaveLedShows()
        {
            foreach (var showViewModel in _ledsViewModel.Shows)
            {
                var showConfig = _ledsViewModel.GetShowAsConfig(showViewModel);
                var path = _configDirectory + @"\LedShows\";
                showConfig.ToFile(path + showViewModel.Name + ".json");
            }
        }

        private void LoadShows(string configLocation)
        {
            var path = configLocation + @"\LedShows\";

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


        private readonly IEventAggregator _eventAggregator;
        private readonly ILeds _ledsViewModel;
        private BindableCollection<IScreen> _leftTabs;
        private string _lastConfigFile;
        private string _configDirectory;
        private string _configName;

    }
}