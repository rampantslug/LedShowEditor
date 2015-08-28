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
    public class GroupConfig
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public IList<uint> Leds { get; set; }
    }
}
