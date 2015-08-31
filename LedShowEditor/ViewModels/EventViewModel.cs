using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;

namespace LedShowEditor.ViewModels
{
    public class EventViewModel: Screen
    {
        private uint _startFrame;
        private uint _endFrame;
        private Brush _eventColor;

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
            // TODO: If single color throughout event then all good. Else if transition need to find the color at specific frame
            return EventColor;
        }
    }
}
