using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using LedShowEditor.ViewModels;

namespace LedShowEditor.Display.LedTree
{
    [Export(typeof(ILedTree))]
    class LedTreeViewModel: Screen, ILedTree, IDropTarget
    {

        private readonly IEventAggregator _eventAggregator;
        public ILeds LedsVm { get; set; }

         /// <summary>
        /// 
        /// </summary>
        [ImportingConstructor]
        public LedTreeViewModel(IEventAggregator eventAggregator, ILeds ledsViewModel)
        {
            _eventAggregator = eventAggregator;
            LedsVm = ledsViewModel;

             DisplayName = "Leds";
        }

        public void AddLed()
        {
            LedsVm.AddLed();
        }
     
        public void AddGroup()
        {
            LedsVm.AddGroup();
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // Allow moving leds or groups
            var sourceLed = dropInfo.Data as LedViewModel;
            var sourceGroup = dropInfo.Data as GroupViewModel;
            var targetItem = dropInfo.TargetItem as GroupViewModel;

            if (targetItem != null)
            {
                if (sourceLed != null)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                }
                else if (sourceGroup != null)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                }
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var sourceLed = dropInfo.Data as LedViewModel;
            var sourceGroup = dropInfo.Data as GroupViewModel;
            var targetItem = dropInfo.TargetItem as GroupViewModel;

            if (targetItem != null && sourceLed != null)
            {
                // Remove led from original group
                foreach (var group in LedsVm.Groups)
                {
                    if (group.Leds.Contains(sourceLed))
                    {
                        group.Leds.Remove(sourceLed);
                        break;
                    }
                }
                targetItem.Leds.Add(sourceLed);
            }
            else if (targetItem != null && sourceGroup != null)
            {
                // Perform default drop behaviour of inserting above??
                var targetIndex = LedsVm.Groups.IndexOf(targetItem);
                LedsVm.Groups.Remove(sourceGroup);
                LedsVm.Groups.Insert(targetIndex, sourceGroup);
            }
        }


    }
}
