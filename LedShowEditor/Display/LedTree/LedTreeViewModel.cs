using System.ComponentModel.Composition;
using Caliburn.Micro;
using LedShowEditor.ViewModels;

namespace LedShowEditor.Display.LedTree
{
    [Export(typeof(ILedTree))]
    class LedTreeViewModel: Screen, ILedTree
    {

        private readonly IEventAggregator _eventAggregator;
        public ILeds LedsVm { get; set; }

         /// <summary>
        /// 
        /// </summary>
        [ImportingConstructor]
        public LedTreeViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

             DisplayName = "Leds";
        }

        public void AddLed()
        {
            LedsVm.AddLed();
        }

        public void DeleteLed()
        {
            LedsVm.DeleteLed();
        }

        public void AddGroup()
        {
            LedsVm.AddGroup();
        }

        public void DeleteGroup()
        {
            LedsVm.DeleteGroup();
        }
    }
}
