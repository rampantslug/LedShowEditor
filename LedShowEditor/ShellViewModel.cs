using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
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
        }



        protected override void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);
            LeftTabs.Add(LedTree);
            LeftTabs.Add(ShowList);

            LoadConfig();

        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            SaveConfig();
        }

        // TODO: Link this to a button so user can select config file
        // Attempts to load last loaded config file
        private void LoadConfig()
        {
            // Retrieve saved configuration information
            var filePath = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
            var gameConfiguration = Configuration.FromFile(filePath + @"\Config\machine.json");

            // Update local information from configuration
            Playfield.PlayfieldImagePath = filePath + @"\Config\" + gameConfiguration.PlayfieldImage;

            _ledsViewModel.Import(gameConfiguration.Leds);
            _ledsViewModel.Import(gameConfiguration.Groups);

        }

        private void SaveConfig()
        {
            var config = new Configuration
            {
                Leds = _ledsViewModel.ExportLeds(),
                Groups = _ledsViewModel.ExportGroups()
            };
            config.ToFile("output.json");
        }


    }
}