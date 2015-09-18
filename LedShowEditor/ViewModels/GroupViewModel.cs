using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using LedShowEditor.Config;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    public class GroupViewModel : Screen
    {
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

        public GroupViewModel(IEventAggregator eventAggregator, GroupConfig groupConfig)
            : this(eventAggregator)
        {
            Name = groupConfig.Name;
        }

        public GroupViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Name = "New group";
            Leds = new BindableCollection<LedViewModel>();
        }

     
        private IObservableCollection<LedViewModel> _leds;
        private readonly IEventAggregator _eventAggregator;
        private string _name;
    }
}

