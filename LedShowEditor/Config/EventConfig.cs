using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LedShowEditor.Config
{
    public class EventConfig
    {
        public uint StartFrame{ get; set; }

        public uint EndFrame { get; set; }

        // TODO: this will get more complex as what events can entail icnrease
        public Color EventColor { get; set; }
    }
}
