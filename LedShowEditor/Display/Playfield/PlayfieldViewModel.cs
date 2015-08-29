using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Caliburn.Micro;
using LedShowEditor.ViewModels;
using MahApps.Metro.Controls;

namespace LedShowEditor.Display.Playfield
{
    [Export(typeof(IPlayfield))]
    class PlayfieldViewModel: Screen, IPlayfield
    {
        private string _playfieldImagePath;
        private ImageSource _playfieldImage;
        private double _playfieldWidth;
        private double _playfieldHeight;
        private double _scaleFactorX;
        private double _scaleFactorY;
        private readonly IEventAggregator _eventAggregator;

        #region Properties

        public string PlayfieldImagePath
        {
            get
            {
                return _playfieldImagePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _playfieldImagePath = value;

                    if (System.IO.File.Exists(_playfieldImagePath))
                    {
                        PlayfieldImage = new BitmapImage(new Uri(_playfieldImagePath));
                    }
                }
            }
        }

        public ImageSource PlayfieldImage
        {
            get
            {
                return _playfieldImage;
            }
            set
            {
                _playfieldImage = value;
                NotifyOfPropertyChange(() => PlayfieldImage);
            }
        }

        public double PlayfieldWidth
        {
            get
            {
                return _playfieldWidth;
            }
            set
            {
                _playfieldWidth = value;
                NotifyOfPropertyChange(() => PlayfieldWidth);

                // Update ScaleFactor
                ScaleFactorX = PlayfieldWidth / 100;
            }
        }

        public double PlayfieldHeight
        {
            get
            {
                return _playfieldHeight;
            }
            set
            {
                _playfieldHeight = value;
                NotifyOfPropertyChange(() => PlayfieldHeight);

                // Update ScaleFactor
                ScaleFactorY = PlayfieldHeight / 100;

            }
        }

        public double ScaleFactorX
        {
            get
            {
                return _scaleFactorX;
            }
            set
            {
                _scaleFactorX = value;
                NotifyOfPropertyChange(() => ScaleFactorX);
            }
        }

        public double ScaleFactorY
        {
            get
            {
                return _scaleFactorY;
            }
            set
            {
                _scaleFactorY = value;
                NotifyOfPropertyChange(() => ScaleFactorY);
            }
        }


        public ILeds LedsVm { get; set; }

        public LedViewModel SelectedLed { get; set; }
   

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        [ImportingConstructor]
        public PlayfieldViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

            PlayfieldWidth = 400;
            PlayfieldHeight = 800;

        }

        

        #endregion

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);

        }

        #region Handle Mouse movement / Dragging of Device

        public void MouseEnter(object source)
        {
            // change mouse cursor
            Mouse.OverrideCursor = Cursors.Hand;
        }

        public void MouseLeave(object source)
        {
            // change mouse cursor
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public Point StartingPoint { get; set; }

        public void MouseDown(object source)
        {
            var ledGeom = source as Path;
            if (ledGeom != null)
            {
                //var parentGrid = myGrid.GetParentObject() as Grid;
                StartingPoint = Mouse.GetPosition(ledGeom);

                

                // Find the Led we are moving and set it to selected device
                var dataContext = ledGeom.DataContext as LedViewModel;
                if (dataContext != null)
                {
                    SelectedLed = dataContext;
                }
            }
            else
            {
                SelectedLed = null;                
            }
        }

        public void MouseMove(object source)
        {
            var ledGeom = source as Path;
            if (Mouse.LeftButton == MouseButtonState.Pressed && ledGeom != null && SelectedLed != null)
            {
                var currentPoint = Mouse.GetPosition(ledGeom);
                var xDelta = currentPoint.X - StartingPoint.X;
                var yDelta = currentPoint.Y - StartingPoint.Y;

                SelectedLed.LocationX = SelectedLed.LocationX + (double)(xDelta / ScaleFactorX);
                SelectedLed.LocationY = SelectedLed.LocationY + (double)(yDelta / ScaleFactorY);
            }
        }

        #endregion

    }
}
