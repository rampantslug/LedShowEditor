using Caliburn.Micro;
using LedShowEditor.Config;

namespace LedShowEditor.ViewModels
{
    public class LedViewModel : Screen
    {
        public uint Id { get; set; }
        public string HardwareAddress { get; set; } // Possible support for locating led on physical hardware
        public string Name { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }


        public LedViewModel()
        {
            // Default a bunch of stuff
            Name = "New Led";
            LocationX = 0;
            LocationY = 0;
        }

        public LedViewModel(LedConfig ledConfig)
        {
            Id = ledConfig.Id;
            HardwareAddress = ledConfig.HardwareAddress;
            Name = ledConfig.Name;
            LocationX = ledConfig.LocationX;
            LocationY = ledConfig.LocationY;
        }
    }
}
