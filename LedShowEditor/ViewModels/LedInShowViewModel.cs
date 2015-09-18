using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
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


        public void DeleteEvent(EventViewModel dataContext)
        {
            Events.Remove(dataContext);
        }

        #region Handle Mouse movement / Dragging of Event

        public void MouseEnter(object source)
        {
            var eventContainer = source as Grid;
            if (eventContainer != null)
            {
                var position = Mouse.GetPosition(eventContainer);
                var variance = 3;
                foreach (var eventViewModel in Events)
                {
                    var startIndex = eventViewModel.StartFrame * _scaleFactor;
                    var endIndex = eventViewModel.EndFrame * _scaleFactor;

                    if (position.X > startIndex - variance && position.X < startIndex + variance)
                    {
                        Mouse.OverrideCursor = Cursors.SizeWE;
                        return;
                    }
                    else if (position.X > endIndex - variance && position.X < endIndex + variance)
                    {
                        //Mouse.SetCursor(Cursors.SizeWE);
                        Mouse.OverrideCursor = Cursors.SizeWE;
                        return;
                    }
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        public void MouseLeave(object source)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            /*var ledGeom = source as Path;
            if (ledGeom != null)
            {
                var activeLed = ledGeom.DataContext as LedViewModel;
                if (activeLed != null)
                {
                    activeLed.IsMouseOver = false;
                }
            }*/
        }

        private Point StartingPoint { get; set; }
        private bool _isDraggingStartFrame;

        private uint _scaleFactor = 5;

        private EventViewModel _activeEvent = null;

        public void MouseDown(object source)
        {
            var eventContainer = source as Grid;
            if (eventContainer != null)
            {
                var position = Mouse.GetPosition(eventContainer);
                var variance = 3;
                foreach (var eventViewModel in Events)
                {
                    var startIndex = eventViewModel.StartFrame * _scaleFactor;
                    var endIndex = eventViewModel.EndFrame * _scaleFactor;

                    if (position.X > startIndex - variance && position.X < startIndex + variance)
                    {
                        StartingPoint = Mouse.GetPosition(Application.Current.MainWindow);
                        _isDraggingStartFrame = true;
                        _activeEvent = eventViewModel;
                        return;
                    }
                    else if (position.X > endIndex - variance && position.X < endIndex + variance)
                    {
                        StartingPoint = Mouse.GetPosition(Application.Current.MainWindow);
                        _isDraggingStartFrame = false;
                        _activeEvent = eventViewModel;
                        return;
                    }
                }
            }
        }

        public void MouseUp(object source)
        {
            _activeEvent = null;
        }

        public void MouseMove(object source)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && _activeEvent != null)
            {
                var currentPoint = Mouse.GetPosition(Application.Current.MainWindow);
                var xDelta = currentPoint.X - StartingPoint.X;

                if (xDelta >= _scaleFactor)
                {
                    var amount = (uint) xDelta/_scaleFactor;
                    if (_isDraggingStartFrame)
                    {
                        _activeEvent.StartFrame = _activeEvent.StartFrame + amount;
                    }
                    else
                    {
                        _activeEvent.EndFrame = _activeEvent.EndFrame + amount;
                    }

                    // Reset the starting point
                    StartingPoint = currentPoint;
                }
            }
        }

        #endregion


        private IObservableCollection<EventViewModel> _events;
        private IEventAggregator _eventAggregator;
    }
}