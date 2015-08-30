﻿using System.ComponentModel.Composition;
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
    }
}
