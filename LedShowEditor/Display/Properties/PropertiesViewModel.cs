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


        public IEnumerable<LedShape> AllShapes
        {
            get { return Enum.GetValues(typeof(LedShape)).Cast<LedShape>(); }
        }

        public IEnumerable<string> SingleLedEventOptions
        {
            get { return new BindableCollection<string> {"Solid", "Fade In", "Fade Out", "Blinking"}; }
        }

        public string SelectedSingleLedEventOption
        {
            get { return _selectedSingleLedEventOption; }
            set 
            {
                _selectedSingleLedEventOption = value ;
                NotifyOfPropertyChange(() => SelectedSingleLedEventOption);
            }
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
            if (LedsVm.SelectedLed.IsSingleColor)
            {
                // Check what type of event we want...
            }

            LedsVm.AddEvent();
        }

        public void BrowsePlayfieldImage()
        {
            
        }

        private IEventAggregator _eventAggregator;
        private string _selectedSingleLedEventOption;
        public ILeds LedsVm { get; set; }
        public IPlayfield PlayfieldVm { get; set; }
    }
}
