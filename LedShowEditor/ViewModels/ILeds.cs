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


        void Import(IList<LedConfig> leds);
        IList<LedConfig> ExportLeds();
        void Import(IList<GroupConfig> groups);
        IList<GroupConfig> ExportGroups();
    }
}
