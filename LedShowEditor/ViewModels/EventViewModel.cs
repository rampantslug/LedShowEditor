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

        public Brush EventColor
        {
            get
            {
                return _eventColor;
            }
            set
            {
                _eventColor = value;
                NotifyOfPropertyChange(() => EventColor);
            }
        }

        public EventViewModel(uint startFrame, uint endFrame, Brush brush)
        {
            _startFrame = startFrame;
            _endFrame = endFrame;
            _eventColor = brush;
        }

        public bool Contains(uint frame)
        {
            return frame >= StartFrame && frame  <= EndFrame;
        }

        public Brush GetColor(uint frame)
        {
            // Single colour event
            var solidBrush = EventColor as SolidColorBrush;
            if (solidBrush != null)
            {
                return EventColor;
            }
            else // Gradient Brush
            {
                var linearGradBrush = EventColor as LinearGradientBrush;
                if (linearGradBrush != null)
                {
                    // Normalise frame position to length event
                    var positionInEvent = frame - StartFrame;
                    var offset = (double)positionInEvent/EventLength;

                    var offsetColor = linearGradBrush.GradientStops.GetRelativeColor(offset);
                    return new SolidColorBrush(offsetColor);
                }
            }
            return EventColor;
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
        private Brush _eventColor;

       
    }
}
