using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedShowEditor.Config
{
    public class LedInShowConfig
    {
        public uint LinkedLed { get; set; }
        public IList<EventConfig> Events { get; set; }

        public LedInShowConfig()
        {
            Events = new List<EventConfig>();
        }
    }
}
