using Caliburn.Micro;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    public class LedInShowViewModel: Screen
    {      
        public IObservableCollection<EventViewModel> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                NotifyOfPropertyChange(() => Events);
            }
        }

        public LedViewModel LinkedLed { get; set; }

        public LedInShowViewModel(IEventAggregator eventAggregator, LedViewModel linkedLed)
        {
            _eventAggregator = eventAggregator;
            LinkedLed = linkedLed;
            _events = new BindableCollection<EventViewModel>();
        }


        public void DeleteLedFromShow()
        {
            _eventAggregator.PublishOnUIThread(new DeleteLedFromShowEvent(this));
        }

        public void DuplicateLedEvents()
        {
            // TODO: Get led id from context
        }

        public void DeleteEvent(EventViewModel dataContext)
        {
            Events.Remove(dataContext);
        }


        private IObservableCollection<EventViewModel> _events;
        private IEventAggregator _eventAggregator;
    }
}