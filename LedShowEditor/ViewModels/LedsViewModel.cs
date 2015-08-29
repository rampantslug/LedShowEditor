using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using LedShowEditor.Config;

namespace LedShowEditor.ViewModels
{
    [Export(typeof(ILeds))]
    public class LedsViewModel: Screen, ILeds
    {
        public IObservableCollection<LedViewModel> AllLeds { get; set; }
        public IObservableCollection<GroupViewModel> Groups { get; set; } 



        public LedsViewModel()
        {
            AllLeds = new BindableCollection<LedViewModel>();
            Groups = new BindableCollection<GroupViewModel>();
        }






        public void Import(IList<LedConfig> leds)
        {
            AllLeds.Clear();
            if (leds != null)
            {
                foreach (var ledConfig in leds)
                {
                    var led = new LedViewModel(ledConfig);
                    AllLeds.Add(led);
                }
            }
            NotifyOfPropertyChange(()=> AllLeds);
        }

        public IList<LedConfig> ExportLeds()
        {
            return AllLeds.Select(led => new LedConfig()
            {
                Id = led.Id, HardwareAddress = led.HardwareAddress, Name = led.Name, LocationX = led.LocationX, LocationY = led.LocationY, Shape = led.Shape, Angle = led.Angle, Size = led.Size
            }).ToList();
        }

        public void Import(IList<GroupConfig> groups)
        {
            Groups.Clear();
            if (groups != null)
            {
                foreach (var groupConfig in groups)
                {
                    var group = new GroupViewModel(groupConfig);
                    if (AllLeds.Any())
                    {
                        foreach (var ledId in group.LedIds)
                        {
                            var matchingLed = AllLeds.FirstOrDefault(led => led.Id == ledId);
                            if (matchingLed != null)
                            {
                                group.Leds.Add(matchingLed);
                            }
                        }
                    }

                    Groups.Add(group);
                }
            }
            NotifyOfPropertyChange(()=> Groups);
        }

        public IList<GroupConfig> ExportGroups()
        {
            return Groups.Select(group => new GroupConfig()
            {
                Id = group.Id,
                Name = group.Name,
                Leds = group.LedIds
            }).ToList();
        }
    }
}


