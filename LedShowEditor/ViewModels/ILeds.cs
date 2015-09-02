using System.Collections.Generic;
using System.Windows.Media;
using Caliburn.Micro;
using LedShowEditor.Config;

namespace LedShowEditor.ViewModels
{
    public interface ILeds
    {
        IObservableCollection<LedViewModel> AllLeds { get; }
        IObservableCollection<GroupViewModel> Groups { get; }
        IObservableCollection<ShowViewModel> Shows { get; }

        uint CurrentFrame { get; set; }
        bool IsPlaying { get; set; }

        uint NewEventStartFrame { get; set; }
        uint NewEventEndFrame { get; set; }
        Color NewEventStartColor { get; set; }

        void AddLed();
        void DeleteLed(LedViewModel led);
        void DuplicateLed(LedViewModel led);

        void AddGroup();
        void DeleteGroup(GroupViewModel group);

        void AddShow();
        void DeleteShow();

        void AddEvent();

        void LoadLedsFromConfig(IList<LedConfig> leds);
        IList<LedConfig> GetLedsAsConfigs();
        void LoadGroupsFromConfig(IList<GroupConfig> groups);
        IList<GroupConfig> GetGroupsAsConfigs();

        ShowConfig GetShowAsConfig(ShowViewModel show);
        void LoadShowFromConfig(ShowConfig showConfig, string name);

        
    }
}
