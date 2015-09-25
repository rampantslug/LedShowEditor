using System.IO;
using Caliburn.Micro;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    public class ShowViewModel: Screen
    {
        public uint Frames
        {
            get
            {
                return _frames;
            }
            set
            {
                _frames = value;
                NotifyOfPropertyChange(() => Frames);

                _eventAggregator.PublishOnUIThread(new MaxFramesUpdatedEvent());
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                // Need to rename physical filename as well
                var path = Directory.GetCurrentDirectory();
                var initialName = path + @"\LedShows\" + _name + ".json";

                _name = value;

                var updatedName = path + @"\LedShows\" + _name + ".json";
                if (File.Exists(initialName))
                {
                    File.Move(initialName, updatedName);
                }
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
            Name = "New Show";
            Frames = 64;
            Leds = new BindableCollection<LedInShowViewModel>();
        }

        public void DeleteLedFromShow(LedInShowViewModel dataContext)
        {
            Leds.Remove(dataContext);
        }

        public void DuplicateLedEvents(LedInShowViewModel dataContext)
        {

        }



        private IObservableCollection<LedInShowViewModel> _leds;
        private string _name;
        private readonly IEventAggregator _eventAggregator;
        private uint _frames;
        private LedInShowViewModel _selectedLed;
    }

    public class MaxFramesUpdatedEvent
    {
    }
}