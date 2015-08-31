using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.Display.ShowsList;

namespace LedShowEditor.ViewModels
{
    [Export(typeof(ILeds))]
    public class LedsViewModel: Screen, ILeds, IHandle<LedSelectedEvent>
    {
        

        #region Properties

        public IObservableCollection<LedViewModel> AllLeds
        {
            get
            {
                return _allLeds;
            }
            private set
            {
                _allLeds = value;
                NotifyOfPropertyChange(() => AllLeds);
            }
        }
        public IObservableCollection<GroupViewModel> Groups
        {
            get
            {
                return _groups;
            }
            private set
            {
                _groups = value;
                NotifyOfPropertyChange(() => Groups);
            }
        }
        public IObservableCollection<ShowViewModel> Shows
        {
            get
            {
                return _shows;
            }
            private set
            {
                _shows = value;
                NotifyOfPropertyChange(() => Shows);
            }
        }

        public LedViewModel SelectedLed
        {
            get
            {
                return _selectedLed;
            }
            set
            {
                _selectedLed = value;
                NotifyOfPropertyChange(() => SelectedLed);
            }
        }

        public ShowViewModel SelectedShow
        {
            get
            {
                return _selectedShow;
            }
            set
            {
                _selectedShow = value;
                NotifyOfPropertyChange(() => SelectedShow);
            }
        }

        public uint CurrentFrame
        {
            get
            {
                return _currentFrame;
            }
            set
            {
                _currentFrame = value;
                UpdateColorsOfLeds(_currentFrame);
                NotifyOfPropertyChange(() => CurrentFrame);
            }
        }


        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                _isPlaying = value;
                if (_isPlaying)
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
                NotifyOfPropertyChange(() => IsPlaying);
            }
        }

        public uint NewEventStartFrame
        {
            get
            {
                return _newEventStartFrame;
            }
            set
            {
                _newEventStartFrame = value;
                NotifyOfPropertyChange(() => NewEventStartFrame);
            }
        }

        public uint NewEventEndFrame
        {
            get
            {
                return _newEventEndFrame;
            }
            set
            {
                _newEventEndFrame = value;
                NotifyOfPropertyChange(() => NewEventEndFrame);
            }
        }

        public Color NewEventStartColor
        {
            get
            {
                return _newEventStartColor;
            }
            set
            {
                _newEventStartColor = value;
                NotifyOfPropertyChange(() => NewEventStartColor);
            }
        }

        #endregion

        [ImportingConstructor]
        public LedsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            AllLeds = new BindableCollection<LedViewModel>();
            Groups = new BindableCollection<GroupViewModel>();
            Shows = new BindableCollection<ShowViewModel>();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05)
            };
            _timer.Tick += TimerOnTick;
        }

        public void Handle(LedSelectedEvent message)
        {
            SelectedLed = message.SelectedLed;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (SelectedShow != null)
            {
                if (CurrentFrame < SelectedShow.Frames)
                {
                    CurrentFrame++;
                }
                else
                {
                    CurrentFrame = 0;
                    _isPlaying = false;
                }
            }
        }

        private void UpdateColorsOfLeds(uint currentFrame)
        {
            if (SelectedShow == null)
            {
                return;
            }

            // Find each led in the show and set its colour based on the current frame
            foreach (var ledInShow in SelectedShow.Leds)
            {
                // Default to transparent if there is no event during this frame
                var resolvedColor = (Brush)Brushes.Transparent;
                foreach (var ledEvent in ledInShow.Events)
                {
                    if (ledEvent.Contains(currentFrame))
                    {
                        // TODO: Replace brush with color
                        resolvedColor = ledEvent.GetColor(currentFrame);
                        
                        break;
                    }
                }
                ledInShow.LinkedLed.CurrentColor = resolvedColor;
            }
        }

        public void AddLed()
        {
            // Get highest Led.Id
            // This should be fine as most machines have no more than 200 leds
            // It could eventually fail due to stupid user
            uint highestId = 1;
            foreach (var led in AllLeds)
            {
                if (led.Id > highestId)
                {
                    highestId = led.Id;
                }
            }
            AllLeds.Add(new LedViewModel(_eventAggregator, highestId + 1));
        }

        public void DeleteLed()
        {
            if (SelectedLed != null)
            {
                AllLeds.Remove(SelectedLed);
            }
        }

        public void AddGroup()
        {
            Groups.Add(new GroupViewModel());
        }

        public void DeleteGroup()
        {
            // Todo: Need to figure out how to select a group
            //Groups.Remove()
        }

        public void AddShow()
        {

        }

        public void DeleteShow()
        {

        }

        public void AddEvent()
        {
            if (SelectedShow != null && SelectedLed != null)
            {
                var existingLed = SelectedShow.Leds.FirstOrDefault(led => led.LinkedLed == SelectedLed);
                if (existingLed == null)
                {
                    existingLed = new LedInShowViewModel(SelectedLed);
                    SelectedShow.Leds.Add(existingLed);
                }

                // Overwrite any existing event that starts at the same frame (This may or may not be desired behaviour).
                var eventToReplace = existingLed.Events.FirstOrDefault(showEvent => showEvent.StartFrame == NewEventStartFrame);
                if (eventToReplace != null)
                {
                    existingLed.Events.Remove(eventToReplace);
                }

                existingLed.Events.Add(new EventViewModel(NewEventStartFrame, 
                    NewEventEndFrame, new SolidColorBrush(NewEventStartColor)));
            }
        }

        #region Load / Save

        public void LoadLedsFromConfig(IList<LedConfig> leds)
        {
            AllLeds.Clear();
            if (leds != null)
            {
                foreach (var ledConfig in leds)
                {
                    var led = new LedViewModel(_eventAggregator, ledConfig);
                    AllLeds.Add(led);
                }
            }
        }

        public IList<LedConfig> GetLedsAsConfigs()
        {
            return AllLeds.Select(led => new LedConfig()
            {
                Id = led.Id, HardwareAddress = led.HardwareAddress, Name = led.Name, LocationX = led.LocationX, LocationY = led.LocationY, Shape = led.Shape, Angle = led.Angle, Scale = led.Scale
            }).ToList();
        }

        public void LoadGroupsFromConfig(IList<GroupConfig> groups)
        {
            Groups.Clear();
            if (groups != null)
            {
                foreach (var groupConfig in groups)
                {
                    var group = new GroupViewModel(groupConfig);
                    if (AllLeds.Any())
                    {
                        foreach (var ledId in group.LedIds)
                        {
                            var matchingLed = AllLeds.FirstOrDefault(led => led.Id == ledId);
                            if (matchingLed != null)
                            {
                                group.Leds.Add(matchingLed);
                            }
                        }
                    }

                    Groups.Add(group);
                }
            }
        }

        public IList<GroupConfig> GetGroupsAsConfigs()
        {
            return Groups.Select(group => new GroupConfig()
            {
                Id = group.Id,
                Name = group.Name,
                Leds = group.LedIds
            }).ToList();
        }

        public ShowConfig GetShowAsConfig(ShowViewModel show)
        {
            var showConfig = new ShowConfig {Frames = show.Frames};
            foreach (var ledInShow in show.Leds)
            {
                var ledInShowConfig = new LedInShowConfig {LinkedLed = ledInShow.LinkedLed.Id};
                foreach (var showEvent in ledInShow.Events)
                {
                    SolidColorBrush tempBrush = (SolidColorBrush)showEvent.EventColor;
                    var showEventConfig = new EventConfig
                    {
                        StartFrame = showEvent.StartFrame,
                        EndFrame = showEvent.EndFrame,
                        EventColor = tempBrush.Color
                    };
                    ledInShowConfig.Events.Add(showEventConfig);
                }
                showConfig.Leds.Add(ledInShowConfig);               
            }
            return showConfig;
        }

        public void LoadShowFromConfig(ShowConfig showConfig, string name)
        {
            if (showConfig != null)
            {
                var show = new ShowViewModel()
                {
                    Name = name,
                    Frames = showConfig.Frames
                };
                foreach (var ledInShowConfig in showConfig.Leds)
                {
                     var matchingLed = AllLeds.FirstOrDefault(led => led.Id == ledInShowConfig.LinkedLed);
                     if (matchingLed != null)
                     {
                         var ledInShow = new LedInShowViewModel(matchingLed);
                         foreach (var eventConfig in ledInShowConfig.Events)
                         {
                             var eventViewModel = new EventViewModel(eventConfig.StartFrame, eventConfig.EndFrame,
                                 new SolidColorBrush(eventConfig.EventColor));
                             ledInShow.Events.Add(eventViewModel);
                         }
                         show.Leds.Add(ledInShow);
                     }                   
                }
                Shows.Add(show);
            }
        }

        #endregion


        private LedViewModel _selectedLed;
        private IEventAggregator _eventAggregator;
        private uint _currentFrame;
        private ShowViewModel _selectedShow;
        private DispatcherTimer _timer;
        private bool _isPlaying;
        private IObservableCollection<LedViewModel> _allLeds;
        private IObservableCollection<GroupViewModel> _groups;
        private IObservableCollection<ShowViewModel> _shows;
        
        private uint _newEventStartFrame;
        private uint _newEventEndFrame;
        private Color _newEventStartColor;
    }
}


