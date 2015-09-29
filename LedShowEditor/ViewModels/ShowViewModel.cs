using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using LedShowEditor.Display.Dialogs;
using LedShowEditor.ViewModels.Events;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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

        public EventViewModel SelectedEvent
        {
            get
            {
                return _selectedEvent;
            }
            set
            {
                _selectedEvent = value;
                NotifyOfPropertyChange(() => SelectedEvent);
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

        public async void DuplicateLedEvents(LedInShowViewModel dataContext)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var ledNames = Leds.Select(led => led.LinkedLed.Name).ToList();

            var dialog = new LedSelectorDialog(ledNames, metroWindow);
            await metroWindow.ShowMetroDialogAsync(dialog);

            var result = await dialog.WaitForButtonPressAsync();
            if (!string.IsNullOrEmpty(result))
            {
                var matchingLedInShow = Leds.FirstOrDefault(led => led.LinkedLed.Name == result);
                if (matchingLedInShow != null)
                {
                    foreach (var eventVm in dataContext.Events)
                    {
                        var duplicateEventViewModel = new EventViewModel(eventVm.StartFrame, eventVm.EndFrame,
                            eventVm.StartColor, eventVm.EndColor);
                        matchingLedInShow.Events.Add(duplicateEventViewModel);
                    }
                }
            }
            await metroWindow.HideMetroDialogAsync(dialog);        
        }

        public void AddLed(LedViewModel ledVm)
        {
            var matchingLed = Leds.FirstOrDefault(led => led.LinkedLed.Id == ledVm.Id);
            if (matchingLed == null) // Led does not already exist
            {
                Leds.Add(new LedInShowViewModel(_eventAggregator, ledVm));
            }
        }

        public void AddGroup(GroupViewModel groupVm)
        {
            foreach (var ledViewModel in groupVm.Leds)
            {
                AddLed(ledViewModel);
            }
        }


        private IObservableCollection<LedInShowViewModel> _leds;
        private string _name;
        private readonly IEventAggregator _eventAggregator;
        private uint _frames;
        private LedInShowViewModel _selectedLed;
        private EventViewModel _selectedEvent;

    }

    public class MaxFramesUpdatedEvent
    {
    }
}