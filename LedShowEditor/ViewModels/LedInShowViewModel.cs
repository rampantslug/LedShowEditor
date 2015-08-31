using Caliburn.Micro;

namespace LedShowEditor.ViewModels
{
    public class LedInShowViewModel: Screen
    {
        private IObservableCollection<EventViewModel> _events;

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

        public LedInShowViewModel(LedViewModel linkedLed)
        {
            LinkedLed = linkedLed;
            _events = new BindableCollection<EventViewModel>();
        }

    }
}