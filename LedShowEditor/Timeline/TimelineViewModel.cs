using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace LedShowEditor.Timeline
{
    [Export(typeof(ITimeline))]
    public class TimelineViewModel :Screen, ITimeline
    {
    }
}
