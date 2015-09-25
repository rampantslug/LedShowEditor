using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.Display.ShowsList;
using LedShowEditor.Helpers;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    [Export(typeof(ILeds))]
    public class LedsViewModel: Screen, ILeds, 
        IHandle<SelectLedEvent>

    {
        
        #region Properties

        public string WorkingDirectory
        {
            get
            {
                return _workingDirectory;
            }
            set 
            {
                _workingDirectory = Directory.Exists(value) ? value + @"\" : Directory.GetCurrentDirectory() + @"\";
            }
        }

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

                _eventAggregator.PublishOnUIThread(new ShowSelectedEvent());
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

            double framesPerSecond = 32;
            double frameFreq = 1 / framesPerSecond;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(frameFreq)
            };
            _timer.Tick += TimerOnTick;
        }

        #region Event Handlers

        public void Handle(SelectLedEvent message)
        {
            SelectedLed = message.Led;
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
                var additionalpath = WorkingDirectory + @"LedShows\" + show.Name + ".json";
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

        /// <summary>
        /// Export LedShow in lamp show format. i.e. Dots and Spaces
        /// </summary>
        /// <param name="show"></param>
        public void ExportLampShow(ShowViewModel show)
        {
            var filePath = WorkingDirectory + @"LedShows\" + show.Name + ".lampshow";
            var stringBuilder = new StringBuilder();

            const int lineWidthToPipe = 44;
            const int lineWidthToFirstFrame = 46;


            //
            // Header Template
            //
            var hashLine = "";
            hashLine = hashLine.PadRight(lineWidthToFirstFrame + 64, '#');
            stringBuilder.AppendLine(hashLine);
            stringBuilder.AppendLine("# Lightshow: " + show.Name);
            stringBuilder.AppendLine("# Type: simple");
            stringBuilder.AppendLine("# Length: " + show.Frames + " frames - Approx " + show.Frames/32 + " seconds");
            stringBuilder.AppendLine(@"# Created using LedShowEditor - Get it at https://bitbucket.org/rampantslug/ledshoweditor");
            stringBuilder.AppendLine(hashLine);
            
            var markers = "# Markers:";
            var frames = "# Frames:";
            markers = markers.PadRight(lineWidthToPipe);
            frames = frames.PadRight(lineWidthToPipe);
            markers = markers + "|        8      16      24      32      40      48      56      64";
            frames = frames +   "| 1234567812345678123456781234567812345678123456781234567812345678";
            stringBuilder.AppendLine(markers);
            stringBuilder.AppendLine(frames);

            // Add a single line for each led
            foreach (var ledInShowViewModel in show.Leds)
            {
                var ledSimpleName = ledInShowViewModel.LinkedLed.Name.Replace(" ", ""); // Remove spaces
                var line = "lamp:" + ledSimpleName;
                line = line.PadRight(lineWidthToPipe);
                line = line + "| ";

                for (int frameNo = 0; frameNo <= (int)show.Frames; frameNo++)
                {
                    var frameInEvents = false;
                    foreach (var eventViewModel in ledInShowViewModel.Events)
                    {
                        frameInEvents = eventViewModel.ContainsFrame(frameNo);
                        if (frameInEvents)
                            break;
                    }
                    if (frameInEvents)
                    {
                        line = line + "."; // ON
                    }
                    else
                    {
                        line = line + " "; // OFF
                    }
                } 
                stringBuilder.AppendLine(line);
            }

            File.WriteAllText(filePath,stringBuilder.ToString());
        }

        public void DeleteLedFromShowEvent(ShowViewModel show, LedInShowViewModel led)
        {
            show.Leds.Remove(led);
        }

        public void AddEvent()
        {
            AddEvent(NewEventStartFrame, NewEventEndFrame);
        }

        public void AddEvent(uint startFrame, uint endFrame)
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
                    if (existingEvent.StartFrame >= startFrame && existingEvent.EndFrame <= endFrame)
                    {
                        removeEvents.Add(existingEvent);
                        continue;
                    }

                    // Does new event start before existing finishes -> update existing finish time
                    if (existingEvent.EndFrame > startFrame && existingEvent.EndFrame < endFrame)
                    {
                        existingEvent.EndFrame = startFrame;
                        continue;
                    }

                    // Does new event finish after existing starts -> update existing start time
                    if (existingEvent.StartFrame > startFrame && existingEvent.EndFrame > endFrame)
                    {
                        existingEvent.StartFrame = endFrame;
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
                    var eventToAdd = new EventViewModel(startFrame, endFrame, new SolidColorBrush(SelectedLed.SingleColor));
                    existingLed.Events.Add(eventToAdd);
                }
                else
                {
                    var eventToAdd = new EventViewModel(startFrame, endFrame, new LinearGradientBrush(NewEventStartColor, NewEventEndColor, 0));

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

            // Add leds to groups
            if (groups != null)
            {
                foreach (var groupConfig in groups)
                {
                    foreach (var ledId in groupConfig.Leds)
                    {
                        var ledViewModel = AllLeds.FirstOrDefault(match => match.Id == ledId);
                        if (ledViewModel != null)
                        {
                            var groupVm = Groups.FirstOrDefault(match => match.Name == groupConfig.Name);
                            if (groupVm != null)
                            {
                                groupVm.Leds.Add(ledViewModel);
                            }
                        }
                    }
                }
            }
            else
            {
                // Add all leds to unassigned as we currently have no groups
                foreach (var ledViewModel in AllLeds)
                {
                    _unassignedGroup.Leds.Add(ledViewModel);
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
                    Color startColor = Colors.Black;
                    Color endColor = Colors.Black;

                    // Single colour event
                    var solidBrush = showEvent.EventColor as SolidColorBrush;
                    if (solidBrush != null)
                    {
                        startColor = solidBrush.Color;
                        endColor = solidBrush.Color;
                    }
                    else // Gradient Brush
                    {
                        var linearGradBrush = showEvent.EventColor as LinearGradientBrush;
                        if (linearGradBrush != null)
                        {
                            startColor = linearGradBrush.GradientStops.GetRelativeColor(0);
                            endColor = linearGradBrush.GradientStops.GetRelativeColor(1);
                        }
                    }

                    var showEventConfig = new EventConfig
                    {
                        StartFrame = showEvent.StartFrame,
                        EndFrame = showEvent.EndFrame,
                        StartColor = startColor,
                        EndColor = endColor,
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
                             Brush eventBrush;

                             if (eventConfig.StartColor == eventConfig.EndColor)
                             {
                                 eventBrush = new SolidColorBrush(eventConfig.StartColor);
                             }
                             else
                             {
                                 eventBrush = new LinearGradientBrush(eventConfig.StartColor, eventConfig.EndColor, 0);
                             }

                             var eventViewModel = new EventViewModel(eventConfig.StartFrame, eventConfig.EndFrame,
                                 eventBrush);
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
        private string _workingDirectory;
    }
}


