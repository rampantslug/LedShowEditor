using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    public class LedInShowViewModel: Screen, IHandle<SingleColorLedColorModifiedEvent>
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
            _eventAggregator.Subscribe(this);
            LinkedLed = linkedLed;
            _events = new BindableCollection<EventViewModel>();
        }


        public void DeleteEvent(EventViewModel dataContext)
        {
            Events.Remove(dataContext);
        }

        public void Handle(SingleColorLedColorModifiedEvent message)
        {
            if (message.Led == LinkedLed)
            {
                foreach (var eventViewModel in Events)
                {
                    // Dont modify transparent values as we want to keep fades
                    if (eventViewModel.StartColor != Colors.Transparent)
                    {
                        eventViewModel.StartColor = message.NewColor;
                    }
                    if (eventViewModel.EndColor != Colors.Transparent)
                    {
                        eventViewModel.EndColor = message.NewColor;
                    }
                }
            }
        }

        public EventViewModel GetLastEvent()
        {
            var lastEvent = Events.First();
            foreach (var eventViewModel in Events)
            {
                if (eventViewModel.EndFrame >= lastEvent.EndFrame)
                {
                    lastEvent = eventViewModel;
                }
            }
            return lastEvent;
        }


        private IObservableCollection<EventViewModel> _events;
        private IEventAggregator _eventAggregator;

        
    }
}