using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using LedShowEditor.Config;

namespace LedShowEditor.ViewModels
{
    public class LedViewModel : Screen
    {
        private double _locationX;
        private double _locationY;
        private LedShape _shape;
        private double _angle;
        private Geometry _ledGeometry;
        private double _size;
        public uint Id { get; set; }
        public string HardwareAddress { get; set; } // Possible support for locating led on physical hardware
        public string Name { get; set; }
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
        public double Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                NotifyOfPropertyChange(() => Size);
            }
        }

        public LedViewModel()
        {
            // Default a bunch of stuff
            Name = "New Led";
            LocationX = 0;
            LocationY = 0;
        }

        public LedViewModel(LedConfig ledConfig)
        {
            Id = ledConfig.Id;
            HardwareAddress = ledConfig.HardwareAddress;
            Name = ledConfig.Name;
            LocationX = ledConfig.LocationX;
            LocationY = ledConfig.LocationY;
            Shape = ledConfig.Shape;
            Angle = ledConfig.Angle;
            Size = ledConfig.Size;
        }

        private void UpdateGeometry(LedShape shape)
        {
            switch (shape)
            {
                case LedShape.Square:
                {
                    LedGeometry = new RectangleGeometry(new Rect(new Size(30, 30)));
                    break;
                }
                case LedShape.Rectangle:
                {
                    LedGeometry = new RectangleGeometry(new Rect(new Size(60, 20)));
                    break;
                }
                case LedShape.Circle:
                {
                    LedGeometry = new EllipseGeometry(new Rect(new Size(30,30)));
                    break;
                }
                default:
                {
                    LedGeometry = new RectangleGeometry(new Rect(new Size(60,20)));
                    break;
                }
            }
        }
    }
}
