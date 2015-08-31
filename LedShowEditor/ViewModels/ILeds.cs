using System.Collections.Generic;
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


        void AddLed();
        void DeleteLed();
        void AddGroup();
        void DeleteGroup();
        void AddShow();
        void DeleteShow();


        void LoadLedsFromConfig(IList<LedConfig> leds);
        IList<LedConfig> GetLedsAsConfigs();
        void LoadGroupsFromConfig(IList<GroupConfig> groups);
        IList<GroupConfig> GetGroupsAsConfigs();

        ShowConfig GetShowAsConfig(ShowViewModel show);
        void LoadShowFromConfig(ShowConfig showConfig, string name);
    }
}
