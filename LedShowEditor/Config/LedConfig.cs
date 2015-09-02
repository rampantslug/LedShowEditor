using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LedShowEditor.Config
{
    /// <summary>
    /// This class gets serialised to json
    /// </summary>
    public class LedConfig
    {
        public uint Id { get; set; }
        public string HardwareAddress { get; set; } // Possible support for locating led on physical hardware
        public string Name { get; set; }
        public bool IsSingleColor { get; set; }
        public Color SingleColor { get; set; }

        public double LocationX { get; set; }
        public double LocationY { get; set; }

        public double Angle { get; set; }
        public double Scale { get; set; }

        public LedShape Shape { get; set; }
    }
}
