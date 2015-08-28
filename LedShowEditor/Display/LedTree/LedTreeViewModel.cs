using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace LedShowEditor.Display.LedTree
{
    [Export(typeof(ILedTree))]
    class LedTreeViewModel: Screen, ILedTree
    {
    }
}
