using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace LedShowEditor.ViewModels
{
    [Export(typeof(ILeds))]
    public class LedsViewModel: Screen, ILeds
    {
    }
}
