using System.Security.Policy;
using Caliburn.Micro;

namespace LedShowEditor.Display.Playfield
{
    public interface IPlayfield : IScreen
    {
        void ClearImage();
        void UpdateImageLocation(string imageLocation);

        void UpdateImage(string imageFilename);
        string PlayfieldImageName { get; }
        double PlayfieldToLedsScale { get; set; }
    }
}
