using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using LedShowEditor.ViewModels;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.Display.Timeline
{
    [Export(typeof (ITimeline))]
    public class TimelineViewModel : Screen, ITimeline
    {
        private IEventAggregator _eventAggregator;

        public ILeds LedsVm { get; set; }


        public LedInShowViewModel SelectedLed
        {
            get
            {
                if (LedsVm.SelectedShow != null)
                {
                    var matchingLedInShow = LedsVm.SelectedShow.Leds.FirstOrDefault(led => led.LinkedLed == LedsVm.SelectedLed);
                    return matchingLedInShow;
                }

                return null;
            }
            set
            {
                // Update linked led...
                if (value != null) // Somehow value can be null?!?
                {
                    LedsVm.SelectedLed = value.LinkedLed;
                    NotifyOfPropertyChange(() => SelectedLed);
                }
            }
        }

        [ImportingConstructor]
        public TimelineViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;
        }

        #region Playback Control

        public void FirstFrame()
        {
            LedsVm.IsPlaying = false;
            LedsVm.CurrentFrame = 0;
        }

        public void Pause()
        {
            LedsVm.IsPlaying = false;
        }

        public void PlayPause()
        {
            LedsVm.IsPlaying = !LedsVm.IsPlaying;
        }

        public void StepBack()
        {
            LedsVm.IsPlaying = false;
            LedsVm.CurrentFrame--;
        }

        public void StepForward()
        {
            LedsVm.IsPlaying = false;
            LedsVm.CurrentFrame++;
        }

        public void LastFrame()
        {
            LedsVm.IsPlaying = false;

            if (LedsVm.SelectedShow != null)
            {
                LedsVm.CurrentFrame = LedsVm.SelectedShow.Frames - 1;
            }
        }

        #endregion

        public void ExecuteLedRowCommand(Key key)
        {
            if (key == Key.D1)
            {
                LedsVm.AddEvent(LedsVm.CurrentFrame, LedsVm.CurrentFrame + 4);
            }
            else if (key == Key.D2)
            {
                LedsVm.AddEvent(LedsVm.CurrentFrame, LedsVm.CurrentFrame + 8);
            }
            else if (key == Key.D3)
            {
                LedsVm.AddEvent(LedsVm.CurrentFrame, LedsVm.CurrentFrame + 16);
            }
            else if (key == Key.D4)
            {
                LedsVm.AddEvent(LedsVm.CurrentFrame, LedsVm.CurrentFrame + 32);
            }
        }

        #region Handle Mouse movement / Dragging of Event

        public void MouseEnter(object source)
        {
            var eventContainer = source as Grid;
            if (eventContainer != null)
            {
            }
        }

        public void MouseLeave(object source)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private Point StartingPoint { get; set; }
        private bool _isDraggingStartFrame;

        private uint _scaleFactor = 5;

        private EventViewModel _activeEvent = null;
        private LedInShowViewModel _selectedLed;
        private bool _hoverEdgeActive;

        public void PreviewMouseLeftButtonDown(object source)
        {
            var listView = source as ListView;
            if (listView != null && SelectedLed != null)
            {
                var position = Mouse.GetPosition(listView);

                // Get the LedInShow row that mouse is over...
                var ledIndex = LedsVm.SelectedShow.Leds.IndexOf(SelectedLed);
                var rowHeight = 25; // NOTE: If this is changed in xaml then this needs to be updated. Possibly bind xaml to this value.

                var selectedRowY1 = ledIndex*rowHeight;
                var selectedRowY2 = ledIndex*rowHeight + rowHeight;

                if (position.Y >= selectedRowY1 && position.Y <= selectedRowY2)
                {                  
                    var variance = 5;
                    foreach (var eventViewModel in SelectedLed.Events)
                    {
                        var startIndex = eventViewModel.StartFrame * _scaleFactor;
                        var endIndex = eventViewModel.EndFrame * _scaleFactor;

                        if (position.X > startIndex - variance && position.X < startIndex + variance)
                        {
                            StartingPoint = Mouse.GetPosition(listView);
                            _activeEvent = eventViewModel;
                            break;
                        }
                        else if (position.X > endIndex - variance && position.X < endIndex + variance)
                        {
                            StartingPoint = Mouse.GetPosition(listView);
                            _activeEvent = eventViewModel;
                            break;
                        }
                    }
                    if (!_hoverEdgeActive)
                    {
                        LedsVm.CurrentFrame = (uint)position.X / _scaleFactor;
                    }
                }        
            }
        }

        public void PreviewMouseLeftButtonUp(object source)
        {
            _activeEvent = null;
        }

        public void MouseMove(object source)
        {
            

            var listView = source as ListView;
            if (listView != null && SelectedLed != null)
            {
                var position = Mouse.GetPosition(listView);

                // Get the LedInShow row that mouse is over...
                var ledIndex = LedsVm.SelectedShow.Leds.IndexOf(SelectedLed);
                var rowHeight = 25; // NOTE: If this is changed in xaml then this needs to be updated. Possibly bind xaml to this value.

                var selectedRowY1 = ledIndex*rowHeight;
                var selectedRowY2 = ledIndex*rowHeight + rowHeight;

                if (position.Y >= selectedRowY1 && position.Y <= selectedRowY2)
                {
                    var variance = 5;
                    foreach (var eventViewModel in SelectedLed.Events)
                    {
                        var startIndex = eventViewModel.StartFrame * _scaleFactor;
                        var endIndex = eventViewModel.EndFrame * _scaleFactor;

                        if (position.X > startIndex - variance && position.X < startIndex + variance)
                        {
                            Mouse.OverrideCursor = Cursors.SizeWE;
                            _isDraggingStartFrame = true;
                            _activeEvent = eventViewModel;
                            _hoverEdgeActive = true;
                            break;
                        }
                        else if (position.X > endIndex - variance && position.X < endIndex + variance)
                        {
                            //Mouse.SetCursor(Cursors.SizeWE);
                            Mouse.OverrideCursor = Cursors.SizeWE;
                            _isDraggingStartFrame = false;
                            _activeEvent = eventViewModel;
                            _hoverEdgeActive = true;
                            break;
                        }
                    }

                    if (_activeEvent != null)
                    {
                        if (Mouse.LeftButton == MouseButtonState.Pressed)
                        {
                            var xDelta = position.X - StartingPoint.X;
                            if (xDelta >= _scaleFactor || xDelta <= -_scaleFactor)
                            {
                                var amount = (int) xDelta/_scaleFactor;
                                if (_isDraggingStartFrame)
                                {
                                    _activeEvent.StartFrame = (uint)(_activeEvent.StartFrame + amount);
                                }
                                else
                                {
                                    _activeEvent.EndFrame = (uint)(_activeEvent.EndFrame + amount);
                                }

                                // Reset the starting point
                                StartingPoint = position;
                            }
                            _hoverEdgeActive = true;
                        }
                    }
                    if(!_hoverEdgeActive)
                    {
                        Mouse.OverrideCursor = Cursors.Arrow;
                        _activeEvent = null;
                    }                   
                }
                _hoverEdgeActive = false;
            }        
        }

        #endregion

   
    }
}