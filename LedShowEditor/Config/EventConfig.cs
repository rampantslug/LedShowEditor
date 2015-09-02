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
    public class EventConfig
    {
        public uint StartFrame{ get; set; }

        public uint EndFrame { get; set; }

        public Color StartColor { get; set; }

        public Color EndColor { get; set; }
    }
}
