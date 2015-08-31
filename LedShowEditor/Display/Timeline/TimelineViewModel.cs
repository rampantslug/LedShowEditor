using System.ComponentModel.Composition;
using Caliburn.Micro;
using LedShowEditor.ViewModels;

namespace LedShowEditor.Display.Timeline
{
    [Export(typeof(ITimeline))]
    public class TimelineViewModel :Screen, ITimeline
    {
        private IEventAggregator _eventAggregator;

        public ILeds LedsVm { get; set; }

        [ImportingConstructor]
        public TimelineViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

        }

        public void FirstFrame()
        {
            LedsVm.IsPlaying = false;
            LedsVm.CurrentFrame = 0;
        }

        public void Pause()
        {
            LedsVm.IsPlaying = false;
        }

        public void Play()
        {
            LedsVm.IsPlaying = true;
        }

        public void Step()
        {
            LedsVm.IsPlaying = false;
            LedsVm.CurrentFrame++;
        }

        public void LastFrame()
        {
            LedsVm.IsPlaying = false;
            // TODO: Need to replace this with max frames
            LedsVm.CurrentFrame = 100;
        }
    }
}
