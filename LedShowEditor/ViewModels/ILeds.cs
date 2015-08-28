using System.Collections.Generic;
using Caliburn.Micro;
using LedShowEditor.Config;

namespace LedShowEditor.ViewModels
{
    public interface ILeds
    {
         IObservableCollection<LedViewModel> AllLeds { get; set; }
        void Import(IList<LedConfig> leds);
        IList<LedConfig> ExportLeds();

        void Import(IList<GroupConfig> groups);
        IList<GroupConfig> ExportGroups();
    }
}
