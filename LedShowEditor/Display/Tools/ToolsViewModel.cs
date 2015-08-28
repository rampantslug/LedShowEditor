using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace LedShowEditor.Display.Tools
{
    [Export(typeof(ITools))]
    public class ToolsViewModel : Screen, ITools
    {
    }
}
