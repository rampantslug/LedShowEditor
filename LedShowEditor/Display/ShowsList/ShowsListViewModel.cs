using System.ComponentModel.Composition;
using Caliburn.Micro;
using LedShowEditor.ViewModels;

namespace LedShowEditor.Display.ShowsList
{
    [Export(typeof(IShowsList))]
    public class ShowsListViewModel : Screen, IShowsList
    {
        private IEventAggregator _eventAggregator;
        public ILeds LedsVm { get; set; }

        [ImportingConstructor]
        public ShowsListViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

            DisplayName = "Shows";
        }
    }
}
