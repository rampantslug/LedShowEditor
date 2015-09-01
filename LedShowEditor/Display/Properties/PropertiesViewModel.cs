using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.Display.Playfield;
using LedShowEditor.ViewModels;

namespace LedShowEditor.Display.Properties
{
    [Export(typeof(IProperties))]
    public class PropertiesViewModel : Screen, IProperties
    {
        private IEventAggregator _eventAggregator;
        public ILeds LedsVm { get; set; }
        public IPlayfield PlayfieldVm { get; set; }

        public IEnumerable<LedShape> AllShapes
        {
            get { return Enum.GetValues(typeof(LedShape)).Cast<LedShape>(); }
        }
 
        [ImportingConstructor]
        public PropertiesViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel, IPlayfield playfieldViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;
            PlayfieldVm = playfieldViewModel;
        }      

        public void AddEvent()
        {
            LedsVm.AddEvent();
        }
    }
}
