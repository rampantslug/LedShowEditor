using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.Display.ShowsList;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    [Export(typeof(ILeds))]
    public class LedsViewModel: Screen, ILeds, 
        IHandle<SelectLedEvent>, IHandle<DeleteLedEvent>, IHandle<DuplicateLedEvent>, 
        IHandle<DeleteGroupEvent>,
        IHandle<DeleteShowEvent>, IHandle<DuplicateShowEvent>, IHandle<DeleteLedFromShowEvent>

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

        public Color NewEventEndColor
        {
            get
            {
                return _newEventEndColor;
            }
            set
            {
                _newEventEndColor = value;
                NotifyOfPropertyChange(() => NewEventEndColor);
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

        #region Event Handlers

        public void Handle(SelectLedEvent message)
        {
            SelectedLed = message.Led;
        }

        public void Handle(DeleteLedEvent message)
        {
            DeleteLed(message.Led);
        }

        public void Handle(DuplicateLedEvent message)
        {
            DuplicateLed(message.Led);
        }

        public void Handle(DeleteGroupEvent message)
        {
            DeleteGroup(message.Group);
        }

        public void Handle(DeleteShowEvent message)
        {
            DeleteShow(message.Show);
        }

        public void Handle(DuplicateShowEvent message)
        {
            DuplicateShow(message.Show);
        }

        public void Handle(DeleteLedFromShowEvent message)
        {
            DeleteLedFromShowEvent(SelectedShow, message.Led);
        }

        #endregion

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
            var ledVm = new LedViewModel(_eventAggregator, highestId + 1);
            AllLeds.Add(ledVm);
            _unassignedGroup.Leds.Add(ledVm);
        }

        public void DeleteLed(LedViewModel led)
        {
            if (led != null)
            {
                AllLeds.Remove(led);
                foreach (var groupViewModel in Groups)
                {
                    groupViewModel.Leds.Remove(led);
                }
            }
        }

        public void DuplicateLed(LedViewModel led)
        {
            if (led != null)
            {
                AddLed();
                var duplicate = AllLeds.Last();

                duplicate.IsSingleColor = led.IsSingleColor;
                duplicate.SingleColor = led.SingleColor;
                duplicate.Shape = led.Shape;
                duplicate.Angle = led.Angle;
                duplicate.Scale = led.Scale;
            }
        }

        public void AddGroup()
        {
            Groups.Add(new GroupViewModel(_eventAggregator));
        }

        public void DeleteGroup(GroupViewModel group)
        {
            if (group != null && group != _unassignedGroup)
            {               
                foreach (var led in group.Leds)
                {
                    _unassignedGroup.Leds.Add(led);
                }
                Groups.Remove(group);
            }
        }

        public void AddShow()
        {
            Shows.Add(new ShowViewModel(_eventAggregator));
        }

        
        public void DeleteShow(ShowViewModel show)
        {
            if (show != null)
            {
                Shows.Remove(show);
                // Also need to remove physical file
                var path = Directory.GetCurrentDirectory();
                var additionalpath = path + @"\LedShows\" + show.Name + ".json";
                if (File.Exists(additionalpath))
                {
                    File.Delete(additionalpath);
                }
            }
        }

        public void DuplicateShow(ShowViewModel show)
        {
            if (show != null)
            {
                AddShow();
                var duplicate = Shows.Last();

                duplicate.Frames = show.Frames;
                // Need to perform a deep copy for leds
                foreach (var led in show.Leds)
                {
                    var duplicateLed = new LedInShowViewModel(_eventAggregator, led.LinkedLed);
                    foreach (var eventVm in led.Events)
                    {
                        var duplicateEvent = new EventViewModel(eventVm.StartFrame, eventVm.EndFrame, eventVm.EventColor);
                        duplicateLed.Events.Add(duplicateEvent);
                    }
                    duplicate.Leds.Add(duplicateLed);
                }                
            }
        }

        public void DeleteLedFromShowEvent(ShowViewModel show, LedInShowViewModel led)
        {
            show.Leds.Remove(led);
        }

        public void AddEvent()
        {
            if (SelectedShow != null && SelectedLed != null)
            {
                var existingLed = SelectedShow.Leds.FirstOrDefault(led => led.LinkedLed == SelectedLed);
                
                // Add led to show if required...
                if (existingLed == null)
                {
                    existingLed = new LedInShowViewModel(_eventAggregator, SelectedLed);
                    SelectedShow.Leds.Add(existingLed);
                }

                // Check for overlap of existing events
                var removeEvents = new List<EventViewModel>();
                foreach (var existingEvent in existingLed.Events)
                {
                    // Does new event completely overlap existing -> delete existing
                    if (existingEvent.StartFrame >= NewEventStartFrame && existingEvent.EndFrame <= NewEventEndFrame)
                    {
                        removeEvents.Add(existingEvent);                       
                        continue;
                    }

                    // Does new event start before existing finishes -> update existing finish time
                    if (existingEvent.EndFrame > NewEventStartFrame && existingEvent.EndFrame < NewEventEndFrame)
                    {
                        existingEvent.EndFrame = NewEventStartFrame;
                        continue;
                    }

                    // Does new event finish after existing starts -> update existing start time
                    if (existingEvent.StartFrame > NewEventStartFrame && existingEvent.EndFrame > NewEventEndFrame)
                    {
                        existingEvent.StartFrame = NewEventEndFrame;
                        continue;
                    }

                    // Does new event sit completely inside existing event -> Chop existing event into 2 events
                    //TODO: add code here...
                }
                // Perform removal in different loop as cannot do it in above loop
                foreach (var removeEvent in removeEvents)
                {
                    existingLed.Events.Remove(removeEvent);
                }


                // TODO: If blink type of event then need to add multiple events
                if (SelectedLed.IsSingleColor)
                {
                    var eventToAdd = new EventViewModel(NewEventStartFrame, NewEventEndFrame, new SolidColorBrush(SelectedLed.SingleColor));
                    existingLed.Events.Add(eventToAdd);
                }
                else
                {
                    var eventToAdd = new EventViewModel(NewEventStartFrame, NewEventEndFrame, new LinearGradientBrush(NewEventStartColor, NewEventEndColor, 0));

                    existingLed.Events.Add(eventToAdd);
                }

                
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
                Id = led.Id, HardwareAddress = led.HardwareAddress, Name = led.Name, IsSingleColor = led.IsSingleColor, SingleColor = led.SingleColor, 
                LocationX = led.LocationX, LocationY = led.LocationY, Shape = led.Shape, Angle = led.Angle, Scale = led.Scale
            }).ToList();
        }

        public void LoadGroupsFromConfig(IList<GroupConfig> groups)
        {
            Groups.Clear();

            _unassignedGroup = new GroupViewModel(_eventAggregator) { Name = "Unassigned Leds" };
            Groups.Add(_unassignedGroup);

            if (groups != null)
            {
                foreach (var groupConfig in groups)
                {
                    if (groupConfig.Name != _unassignedGroup.Name)
                    {
                        var group = new GroupViewModel(_eventAggregator, groupConfig);
                        Groups.Add(group);
                    }
                }
            }

            // Go through all the leds and find the first group that it has been assigned to
            if (AllLeds.Any())
            {
                foreach (var ledViewModel in AllLeds)
                {
                    var matchFound = false;
                    if (groups != null)
                    {                        
                        foreach (var groupConfig in groups)
                        {
                            if (groupConfig.Leds.Contains(ledViewModel.Id))
                            {
                                matchFound = true;
                                var groupVm = Groups.FirstOrDefault(match => match.Name == groupConfig.Name);
                                if (groupVm != null)
                                {
                                    groupVm.Leds.Add(ledViewModel);
                                }
                                break;
                            }
                        }
                    }
                    if(!matchFound)
                    {
                        _unassignedGroup.Leds.Add(ledViewModel);
                    }
                }

            }

        }

        public IList<GroupConfig> GetGroupsAsConfigs()
        {
            return Groups.Select(group => new GroupConfig()
            {
                //Id = group.Id,
                Name = group.Name,
                Leds = group.Leds.Select(led => led.Id).ToList()
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
                        StartColor = tempBrush.Color,
                        EndColor = tempBrush.Color,
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
                var show = new ShowViewModel(_eventAggregator)
                {
                    Name = name,
                    Frames = showConfig.Frames
                };
                foreach (var ledInShowConfig in showConfig.Leds)
                {
                     var matchingLed = AllLeds.FirstOrDefault(led => led.Id == ledInShowConfig.LinkedLed);
                     if (matchingLed != null)
                     {
                         var ledInShow = new LedInShowViewModel(_eventAggregator, matchingLed);
                         foreach (var eventConfig in ledInShowConfig.Events)
                         {
                             var eventViewModel = new EventViewModel(eventConfig.StartFrame, eventConfig.EndFrame,
                                 new SolidColorBrush(eventConfig.StartColor));
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
        private GroupViewModel _unassignedGroup;
        private Color _newEventEndColor;
    }
}


