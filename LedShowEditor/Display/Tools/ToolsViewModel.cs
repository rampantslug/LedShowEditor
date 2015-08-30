using System.ComponentModel.Composition;
using Caliburn.Micro;
using LedShowEditor.ViewModels;

namespace LedShowEditor.Display.Tools
{
    [Export(typeof(ITools))]
    public class ToolsViewModel : Screen, ITools
    {
        private IEventAggregator _eventAggregator;
        public ILeds LedsVm { get; set; }


        [ImportingConstructor]
        public ToolsViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

        }
    }
}
