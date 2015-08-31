using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LedShowEditor.ViewModels;

namespace LedShowEditor.Display.Properties
{
    [Export(typeof(IProperties))]
    public class PropertiesViewModel : Screen, IProperties
    {
        private IEventAggregator _eventAggregator;
        public ILeds LedsVm { get; set; }

        [ImportingConstructor]
        public PropertiesViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

        }

        public void AddEvent()
        {
            LedsVm.AddEvent();
        }
    }
}
