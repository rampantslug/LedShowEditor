using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace LedShowEditor.Display.Timeline
{
    [Export(typeof(ITimeline))]
    public class TimelineViewModel :Screen, ITimeline
    {
    }
}
