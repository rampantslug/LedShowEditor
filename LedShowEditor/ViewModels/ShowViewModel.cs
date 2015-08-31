using Caliburn.Micro;

namespace LedShowEditor.ViewModels
{
    public class ShowViewModel: Screen
    {

        private IObservableCollection<LedInShowViewModel> _leds;
        private string _name;


        public uint Frames { get; set; }

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

        public IObservableCollection<LedInShowViewModel> Leds
        {
            get
            {
                return _leds;
            }
            set
            {
                _leds = value;
                NotifyOfPropertyChange(() => Leds);
            }
        }

        public ShowViewModel()
        {
            Leds = new BindableCollection<LedInShowViewModel>();
        }
    }
}