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

        LedViewModel SelectedLed { get; }

        uint CurrentFrame { get; set; }
        bool IsPlaying { get; set; }

        uint NewEventStartFrame { get; set; }
        uint NewEventEndFrame { get; set; }
        Color NewEventStartColor { get; set; }
        string WorkingDirectory { get; set; }

        // Edit Leds
        void AddLed();
        void DeleteLed(LedViewModel led);
        void DuplicateLed(LedViewModel led);

        // Edit Groups
        void AddGroup();
        void DeleteGroup(GroupViewModel group);

        // Edit Shows
        void AddShow();
        void DeleteShow(ShowViewModel show);
        void DuplicateShow(ShowViewModel dataContext);
        void ExportLampShow(ShowViewModel dataContext);

        void AddEvent();

        // Import / Export
        void LoadLedsFromConfig(IList<LedConfig> leds);
        IList<LedConfig> GetLedsAsConfigs();
        void LoadGroupsFromConfig(IList<GroupConfig> groups);
        IList<GroupConfig> GetGroupsAsConfigs();

        ShowConfig GetShowAsConfig(ShowViewModel show);
        void LoadShowFromConfig(ShowConfig showConfig, string name);


        
        
    }
}
