using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LedShowEditor.ViewModels.Events
{
    public class SingleColorLedColorModifiedEvent
    {
        public LedViewModel Led { get; private set; }

        public Color NewColor { get; private set; }

        public SingleColorLedColorModifiedEvent(LedViewModel modifiedLed, Color newColor)
        {
            Led = modifiedLed;
            NewColor = newColor;
        }
    }
}
