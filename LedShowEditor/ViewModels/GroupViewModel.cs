using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LedShowEditor.Config;

namespace LedShowEditor.ViewModels
{
    public class GroupViewModel : Screen
    {
        private IObservableCollection<LedViewModel> _leds;
       // public uint Id { get; set; }

        public string Name { get; set; }

        //public IList<uint> LedIds { get; set; }

        public IObservableCollection<LedViewModel> Leds
        {
            get
            {
                return _leds;
            }
            private set
            {
                _leds = value;
                NotifyOfPropertyChange(() => Leds);
            }
        }

        public GroupViewModel(GroupConfig groupConfig) : this()
        {
            //Id = groupConfig.Id;
            Name = groupConfig.Name;
            //LedIds = groupConfig.Leds;
        }

        public GroupViewModel()
        {
            Name = "Some new group";
            Leds = new BindableCollection<LedViewModel>();
            //LedIds = new BindableCollection<uint>();
        }

        public void DeleteGroup()
        {
        }
    }
}

