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


        #region Import / Export

        public void Import(IList<LedConfig> leds)
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

        public IList<LedConfig> ExportLeds()
        {
            return AllLeds.Select(led => new LedConfig()
            {
                Id = led.Id, HardwareAddress = led.HardwareAddress, Name = led.Name, LocationX = led.LocationX, LocationY = led.LocationY, Shape = led.Shape, Angle = led.Angle, Scale = led.Scale
            }).ToList();
        }

        public void Import(IList<GroupConfig> groups)
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

        public IList<GroupConfig> ExportGroups()
        {
            return Groups.Select(group => new GroupConfig()
            {
                Id = group.Id,
                Name = group.Name,
                Leds = group.LedIds
            }).ToList();
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
    }
}


