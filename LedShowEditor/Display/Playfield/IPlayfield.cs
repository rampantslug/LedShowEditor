using System.Security.Policy;
using Caliburn.Micro;

namespace LedShowEditor.Display.Playfield
{
    public interface IPlayfield : IScreen
    {
        string PlayfieldImagePath { get; set; }
    }
}
