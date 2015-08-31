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
        public uint Id { get; set; }

        public string Name { get; set; }

        public IList<uint> LedIds { get; set; } 

        public IObservableCollection<LedViewModel> Leds { get; set; }

        public GroupViewModel(GroupConfig groupConfig)
        {
            Id = groupConfig.Id;
            Name = groupConfig.Name;
            LedIds = groupConfig.Leds;

            Leds = new BindableCollection<LedViewModel>();
            LedIds = new BindableCollection<uint>();
        }

        public GroupViewModel()
        {
            Name = "Some new group";
        }
    }
}

