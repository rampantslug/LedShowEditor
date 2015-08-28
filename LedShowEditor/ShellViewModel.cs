﻿using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.Display.LedTree;
using LedShowEditor.Display.Playfield;
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

        // Display Modules
        public ILedTree LedTree { get; private set; }    
        public IPlayfield Playfield { get; private set; }
        public ITimeline Timeline { get; private set; }
        public ITools Tools { get; private set; }


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
            ITools tools,
            ITimeline timeline
            )
        {
            _eventAggregator = eventAggregator;
            _ledsViewModel = ledsViewModel;
            LedTree = ledTree;
            Playfield = playfield;
            Timeline = timeline;
            Tools = tools;

        }



        protected override void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);

            LoadConfig();

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

            foreach (var ledConfig in gameConfiguration.Leds)
            {
                var led = new LedViewModel(ledConfig);
            }
        }


    }
}