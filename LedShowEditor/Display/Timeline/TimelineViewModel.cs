using System.ComponentModel.Composition;
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
    }
}