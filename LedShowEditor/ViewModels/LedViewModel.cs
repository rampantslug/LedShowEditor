using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using LedShowEditor.Config;

namespace LedShowEditor.ViewModels
{
    public class LedViewModel : Screen
    {
        private IEventAggregator _eventAggregator;

        private double _locationX;
        private double _locationY;
        private LedShape _shape;
        private double _angle;
        private Geometry _ledGeometry;
        private double _scale;
        private string _name;
        private Brush _currentColor;
        private bool _isSelected;

        private bool _isMouseOver = false;
        private IObservableCollection<EventViewModel> _events;

        public uint Id { get; set; }
        public string HardwareAddress { get; set; } // Possible support for locating led on physical hardware

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public IObservableCollection<EventViewModel> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                NotifyOfPropertyChange(() => Events);
            }
        } 


        public Brush CurrentColor
        {
            get
            {
                return _currentColor;
            }
            set
            {
                _currentColor = value;
                NotifyOfPropertyChange(() => CurrentColor);
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
                NotifyOfPropertyChange(() => IsHighlighted);
                
                // Notify parent container that we are selected
                _eventAggregator.PublishOnUIThread(new LedSelectedEvent
                {
                    SelectedLed = this
                });
            }
        }

        public bool IsHighlighted
        {
            get
            {
                if (IsSelected || IsMouseOver)
                {
                    return true;
                }
                return false;

            }
        }

        public bool IsMouseOver
        {
            get
            {
                return _isMouseOver;
            }
            set
            {
                _isMouseOver = value;
                NotifyOfPropertyChange(() => IsMouseOver);
                NotifyOfPropertyChange(() => IsHighlighted);
            }
        }

        #region Size, Location and Shape Properties

        public double LocationX
        {
            get
            {
                return _locationX;
            }
            set
            {
                _locationX = value;
                NotifyOfPropertyChange(() => LocationX);
            }
        }
        public double LocationY
        {
            get
            {
                return _locationY;
            }
            set
            {
                _locationY = value;
                NotifyOfPropertyChange(() => LocationY);
            }
        }

        public LedShape Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                _shape = value;
                // Update the geometry to match new shape
                UpdateGeometry(_shape);
                NotifyOfPropertyChange(() => Shape);
            }
        }



        public Geometry LedGeometry
        {
            get
            {
                return _ledGeometry;
            }
            set
            {
                _ledGeometry = value;
                NotifyOfPropertyChange(() => LedGeometry);
            }
        }

        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                NotifyOfPropertyChange(() => Angle);
            }
        }
        public double Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                NotifyOfPropertyChange(() => Scale);
            }
        }

        

        #endregion

        public LedViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            // Default a bunch of stuff
            Name = "New Led";

            CurrentColor = Brushes.Transparent;

            LocationX = 0;
            LocationY = 0;
            Shape = LedShape.CircleMed;
            Angle = 0;
            Scale = 1.0;

            _events = new BindableCollection<EventViewModel>();
        }

        public LedViewModel(IEventAggregator eventAggregator, LedConfig ledConfig)
        {
            _eventAggregator = eventAggregator;
            Id = ledConfig.Id;
            HardwareAddress = ledConfig.HardwareAddress;
            Name = ledConfig.Name;

            CurrentColor = Brushes.Transparent;

            LocationX = ledConfig.LocationX;
            LocationY = ledConfig.LocationY;
            Shape = ledConfig.Shape;
            Angle = ledConfig.Angle;
            Scale = ledConfig.Scale;

            _events = new BindableCollection<EventViewModel>();
        }

        // Build what shapes we can from code. Irregular shapes are taken from AllShapes.xaml
        private void UpdateGeometry(LedShape shape)
        {
            switch (shape)
            {
                case LedShape.Square:
                {
                    var square = new RectangleGeometry(new Rect(new Size(19, 19)))
                    {
                        RadiusX = 2, 
                        RadiusY = 2
                    };
                    LedGeometry = square;
                    break;
                }
                case LedShape.Rectangle:
                {
                    var rectangle = new RectangleGeometry(new Rect(new Size(38, 19)))
                    {
                        RadiusX = 2,
                        RadiusY = 2
                    };
                    LedGeometry = rectangle;
                    break;
                }
                case LedShape.CircleLarge:
                {
                    LedGeometry = new EllipseGeometry(new Rect(new Size(43,43)));
                    break;
                }
                case LedShape.CircleMed:
                {
                    LedGeometry = new EllipseGeometry(new Rect(new Size(35, 35)));
                    break;
                }
                case LedShape.CircleSmall:
                {
                    LedGeometry = new EllipseGeometry(new Rect(new Size(30, 30)));
                    break;
                }
                case LedShape.Triangle:
                {
                    LedGeometry = Geometry.Parse("F1 M 185.413,17.47C 184.808,17.4908 184.211,18.0602 183.614,18.6297L 170.114,42.0672C 169.947,42.7547 169.781,43.4422 169.989,43.9526C 170.197,44.4631 170.781,44.7964 171.364,45.1297L 200.051,45.1297C 200.447,44.7547 200.843,44.3797 200.979,43.8901C 201.114,43.4006 200.989,42.7964 200.864,42.1922L 187.239,18.5047C 186.628,17.9769 186.017,17.4492 185.413,17.47 Z ");   
                    break;
                }
                case LedShape.ThinTriangleLarge:
                {
                    LedGeometry = Geometry.Parse("F1 M 129.834,19.3484C 128.912,19.3484 128.012,20.2214 127.501,20.8627C 126.99,21.5041 126.867,21.9139 126.745,22.3237L 117.401,62.9447C 116.934,65.0156 116.467,67.0865 117.058,68.1173C 117.649,69.1481 119.298,69.1389 120.946,69.1296L 138.674,69.1296C 140.349,69.1389 142.024,69.1481 142.618,68.1036C 143.212,67.059 142.724,64.9606 142.236,62.8622L 132.988,22.4692C 132.873,22.0109 132.758,21.5526 132.228,20.887C 131.699,20.2214 130.755,19.3484 129.834,19.3484 Z ");                 
                    break;
                }
                case LedShape.ThinTriangleSmall:
                {
                    LedGeometry = Geometry.Parse("F1 M 89.0975,3.47906C 88.5153,3.479 87.9468,4.14063 87.624,4.62671C 87.3011,5.11273 87.2237,5.4234 87.1464,5.73395L 81.2442,36.5204C 80.9493,38.0899 80.6544,39.6594 81.0277,40.4407C 81.401,41.222 82.4423,41.2149 83.4837,41.2079L 94.6816,41.2079C 95.7397,41.2149 96.7978,41.222 97.1728,40.4303C 97.5478,39.6386 97.2397,38.0482 96.9316,36.4578L 91.0902,5.84424C 91.0173,5.49689 90.9445,5.1496 90.6101,4.64508C 90.2757,4.14063 89.6797,3.47906 89.0975,3.47906 Z ");   
                    break;
                }
                case LedShape.Arrow:
                {
                    LedGeometry = Geometry.Parse("F1 M 229.95,10.7789C 231.153,10.7062 231.776,11.8137 231.776,11.8137L 241.112,32.4879C 241.389,33.1023 241.667,33.7167 241.522,34.223C 241.377,34.7292 240.81,35.1271 240.244,35.5251L 236.094,38.4387C 236.094,38.4387 236.166,38.7881 236.166,38.9733L 236.166,63.8279C 236.166,64.9326 235.27,65.8279 234.166,65.8279L 225.541,65.8279C 224.436,65.8279 223.541,64.9326 223.541,63.8279L 223.541,38.9733C 223.548,38.7062 223.613,38.4387 223.613,38.4387L 219.498,35.5894C 218.772,35.0867 218.045,34.5839 217.833,34.0248C 217.62,33.4658 217.921,32.8503 218.222,32.2349L 228.151,11.9387C 228.151,11.9387 228.861,10.6229 229.95,10.7789 Z ");   
                    break;
                }
                default:
                {
                    LedGeometry = new EllipseGeometry(new Rect(new Size(35, 35)));
                    break;
                }
            }
        }
    }
}
