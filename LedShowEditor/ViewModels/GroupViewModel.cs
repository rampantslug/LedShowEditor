using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LedShowEditor.Config;
using LedShowEditor.ViewModels.Events;

namespace LedShowEditor.ViewModels
{
    public class GroupViewModel : Screen
    {
        public string Name { get; set; }

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

        public void DeleteGroup()
        {
            _eventAggregator.PublishOnUIThread(new DeleteGroupEvent{Group = this});
        }

        private IObservableCollection<LedViewModel> _leds;
        private readonly IEventAggregator _eventAggregator;
    }
}

