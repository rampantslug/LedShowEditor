using Caliburn.Micro;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    public class ShowViewModel: Screen
    {
        public uint Frames { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public IObservableCollection<LedInShowViewModel> Leds
        {
            get
            {
                return _leds;
            }
            set
            {
                _leds = value;
                NotifyOfPropertyChange(() => Leds);
            }
        }

        public ShowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Leds = new BindableCollection<LedInShowViewModel>();
        }

        public void DeleteShow()
        {
            _eventAggregator.PublishOnUIThread(new DeleteShowEvent(){Show = this});
        }

        public void DuplicateShow()
        {
            _eventAggregator.PublishOnUIThread(new DuplicateShowEvent() { Show = this });
        }

        public void ExportLampShow()
        {
            // TODO: Add export lampshow code here...
        }


        private IObservableCollection<LedInShowViewModel> _leds;
        private string _name;
        private readonly IEventAggregator _eventAggregator;
    }
}