using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedShowEditor.Config
{
    /// <summary>
    /// This class gets serialised to json
    /// </summary>
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
