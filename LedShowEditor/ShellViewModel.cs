using System.ComponentModel.Composition;
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

namespace LedShowEditor
{

    /// <summary>
    /// 
    /// </summary>
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShell
    {

        private readonly IEventAggregator _eventAggregator;
        private ILeds _ledsViewModel;
        private BindableCollection<IScreen> _leftTabs;

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
        /// <param name="eventAggregator"></param>
        /// <param name="ledTree"></param>
        /// <param name="playfield"></param>
        /// <param name="tools"></param>
        /// <param name="timeline"></param>    
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

            DisplayName = "Led Show Editor";
        }



        protected override void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);
            LeftTabs.Add(LedTree);
            LeftTabs.Add(ShowList);

            LoadConfig();
            LoadShows();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            SaveConfig();
            SaveLedShows();
        }

        // TODO: Link this to a button so user can select config file
        // Attempts to load last loaded config file
        private void LoadConfig()
        {
            // Retrieve saved configuration information
            var filePath = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
            var gameConfiguration = Configuration.FromFile(filePath + @"\playfieldConfig.json");

            // Update local information from configuration
            Playfield.PlayfieldImagePath = filePath + @"\" + gameConfiguration.PlayfieldImage;

            _ledsViewModel.LoadLedsFromConfig(gameConfiguration.Leds);
            _ledsViewModel.LoadGroupsFromConfig(gameConfiguration.Groups);

        }

        private void SaveConfig()
        {
            var playfieldImage = Path.GetFileName(Playfield.PlayfieldImagePath);
            var config = new Configuration
            {
                Leds = _ledsViewModel.GetLedsAsConfigs(),
                Groups = _ledsViewModel.GetGroupsAsConfigs(),
                PlayfieldImage = playfieldImage
            };
            config.ToFile("playfieldConfig.json");
        }

        private void SaveLedShows()
        {
            foreach (var showViewModel in _ledsViewModel.Shows)
            {
                var showConfig = _ledsViewModel.GetShowAsConfig(showViewModel);
                var path = Directory.GetCurrentDirectory() + @"\LedShows\";
                showConfig.ToFile(path + showViewModel.Name + ".json");
            }
        }

        private void LoadShows()
        {
            var path = Directory.GetCurrentDirectory();
            var additionalpath = path + @"\LedShows\";

            var ledShows = Directory.GetFiles(additionalpath, "*.json");
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
    }
}