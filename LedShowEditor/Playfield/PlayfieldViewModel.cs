using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace LedShowEditor.Playfield
{
    [Export(typeof(IPlayfield))]
    class PlayfieldViewModel: Screen, IPlayfield
    {
    }
}
