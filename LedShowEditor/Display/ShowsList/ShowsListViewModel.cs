using System.ComponentModel.Composition;
using Caliburn.Micro;
using LedShowEditor.ViewModels;
using LedShowEditor.ViewModels.Events;

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

        public void AddShow()
        {
            LedsVm.AddShow();
        }

        public void ImportExistingShow()
        {
            // TODO: open file browser window
        }

        public void DeleteShow(ShowViewModel dataContext)
        {
            LedsVm.DeleteShow(dataContext);
        }

        public void DuplicateShow(ShowViewModel dataContext)
        {
            LedsVm.DuplicateShow(dataContext);
        }

        public void ExportLampShow(ShowViewModel dataContext)
        {
            LedsVm.ExportLampShow(dataContext);
        }

    }
}
