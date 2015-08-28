using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedShowEditor.Config
{
    /// <summary>
    /// This class gets serialised to machine.json
    /// </summary>
    public class LedConfig
    {
        public uint Id { get; set; }
        public string HardwareAddress { get; set; } // Possible support for locating led on physical hardware
        public string Name { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
    }
}
