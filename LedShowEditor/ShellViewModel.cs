using System.ComponentModel.Composition;
using Caliburn.Micro;
using LedShowEditor.LedTree;
using LedShowEditor.Playfield;
using LedShowEditor.Timeline;
using LedShowEditor.Tools;

namespace LedShowEditor
{

    /// <summary>
    /// 
    /// </summary>
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShell
    {

        private readonly IEventAggregator _eventAggregator;

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
            ILedTree ledTree,
            IPlayfield playfield,
            ITools tools,
            ITimeline timeline
            )
        {
            _eventAggregator = eventAggregator;
            LedTree = ledTree;
            Playfield = playfield;
            Timeline = timeline;
            Tools = tools;

        }



        protected override void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);

            _eventAggregator.Subscribe(this);

        }





    }
}