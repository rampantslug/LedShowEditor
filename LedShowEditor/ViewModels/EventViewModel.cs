using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using LedShowEditor.Helpers;

namespace LedShowEditor.ViewModels
{
    public class EventViewModel: Screen
    {
        public uint StartFrame
        {
            get
            {
                return _startFrame;
            }
            set
            {
                _startFrame = value;
                NotifyOfPropertyChange(() => StartFrame);
                NotifyOfPropertyChange(() => EventLength);
            }
        }

        public uint EndFrame
        {
            get
            {
                return _endFrame;
            }
            set
            {
                _endFrame = value;
                NotifyOfPropertyChange(() => EndFrame);
                NotifyOfPropertyChange(() => EventLength);
            }
        }

        public uint EventLength
        {
            get
            {
                return _endFrame - _startFrame;
            }
        }

        public Color StartColor
        {
            get
            {
                return _startColor;
            }
            set
            {
                _startColor = value;
                NotifyOfPropertyChange(() => StartColor);
                UpdateBrush();
            }
        }

        public Color EndColor
        {
            get
            {
                return _endColor;
            }
            set
            {
                _endColor = value;
                NotifyOfPropertyChange(() => EndColor);
                UpdateBrush();
            }
        }

        public Brush EventBrush
        {
            get
            {
                return _eventBrush;
            }
            set
            {
                _eventBrush = value;
                NotifyOfPropertyChange(() => EventBrush);
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public EventViewModel(uint startFrame, uint endFrame, Color startColor, Color endColor)
        {
            _startFrame = startFrame;
            _endFrame = endFrame;
            _startColor = startColor;
            _endColor = endColor;

            UpdateBrush();
        }

        public bool Contains(uint frame)
        {
            return frame >= StartFrame && frame  <= EndFrame;
        }

        public Brush GetColor(uint frame)
        {
            // Single colour event
            var solidBrush = EventBrush as SolidColorBrush;
            if (solidBrush != null)
            {
                return EventBrush;
            }
            else // Gradient Brush
            {
                var linearGradBrush = EventBrush as LinearGradientBrush;
                if (linearGradBrush != null)
                {
                    // Normalise frame position to length event
                    var positionInEvent = frame - StartFrame;
                    var offset = (double)positionInEvent/EventLength;

                    var offsetColor = linearGradBrush.GradientStops.GetRelativeColor(offset);
                    return new SolidColorBrush(offsetColor);
                }
            }
            return EventBrush;
        }

        private void UpdateBrush()
        {
            if (StartColor == EndColor)
            {
                EventBrush = new SolidColorBrush(StartColor);
            }
            else
            {
                EventBrush = new LinearGradientBrush(StartColor, EndColor, 0);
            }
        }

        public bool ContainsFrame(int frameNo)
        {
            return frameNo >= StartFrame && frameNo < EndFrame;
        }

        public void ExecuteLedRowCommand(Key key)
        {
            var breakhere = true;
            if (key == Key.D1)
            {
                // Insert a new event at current frame of 4 frame sizes

            }
            else if (key == Key.D2)
            {
                // Insert a new event at current frame of 4 frame sizes
            }
            else if (key == Key.D3)
            {
                // Insert a new event at current frame of 4 frame sizes
            }
            else if (key == Key.D4)
            {
                // Insert a new event at current frame of 4 frame sizes
            }
        }




        private uint _startFrame;
        private uint _endFrame;
        private Brush _eventBrush;
        private bool _isSelected;
        private Color _startColor;
        private Color _endColor;
    }
}
