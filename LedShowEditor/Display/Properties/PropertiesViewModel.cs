using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.Display.Playfield;
using LedShowEditor.ViewModels;
using LedShowEditor.ViewModels.Events;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace LedShowEditor.Display.Properties
{
    [Export(typeof(IProperties))]
    public class PropertiesViewModel : Screen, IProperties, IHandle<SelectLedEvent>, IHandle<SingleColorLedColorModifiedEvent>
    {


        public IEnumerable<LedShape> AllShapes
        {
            get { return Enum.GetValues(typeof(LedShape)).Cast<LedShape>(); }
        }

        public ObservableCollection<ColorItem> ColorList {
            get { return _colorList; }
        }


        [ImportingConstructor]
        public PropertiesViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel, IPlayfield playfieldViewModel)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            LedsVm = ledsViewModel;
            PlayfieldVm = playfieldViewModel;

            DisplayName = "Properties";

            _colorList = new BindableCollection<ColorItem>()
            {
                new ColorItem(Colors.White, "White"),
                new ColorItem(Colors.Transparent, "Transparent")
            };
        }      

        public void AddEvent()
        {
            LedsVm.AddEvent();
        }

        public void BrowsePlayfieldImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.bmp,*.jpg,*.png)|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|" +
                "BMP (*.bmp)|*.bmp|JPG (*.jpg,*.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|TIF (*.tif,*.tiff)|*.tif;*.tiff",                
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PlayfieldVm.UpdateImage(openFileDialog.FileName);               
            }
        }

        public void ProcessChange()
        {
            if (LedsVm.SelectedLed != null)
            {
                NotifyOfPropertyChange(() => LedsVm.SelectedLed.Name);
            }
        }

        public void Handle(SingleColorLedColorModifiedEvent message)
        {
            UpdateColorList(message.NewColor);
        }

        public void Handle(SelectLedEvent message)
        {
            UpdateColorList(message.Led.SingleColor);
        }

        private void UpdateColorList(Color solidColor) 
        {
            ColorList.Clear();
            ColorList.Add(new ColorItem(solidColor, "Solid Color"));
            ColorList.Add(new ColorItem(Colors.Transparent, "Transparent"));
        }

        private IEventAggregator _eventAggregator;
        private string _selectedSingleLedEventOption;
        private BindableCollection<ColorItem> _colorList;
        public ILeds LedsVm { get; set; }
        public IPlayfield PlayfieldVm { get; set; }

        
    }
}
