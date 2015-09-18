using System.ComponentModel.Composition;
using System.Linq;
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

        public void DeleteGroup(GroupViewModel dataContext)
        {
            LedsVm.DeleteGroup(dataContext);
        }

        public void DeleteLed(LedViewModel dataContext)
        {
            LedsVm.DeleteLed(dataContext);
        }

        public void DuplicateLed(LedViewModel dataContext)
        {
            LedsVm.DuplicateLed(dataContext);
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // Allow moving leds or groups
            var sourceLed = dropInfo.Data as LedViewModel;
            var sourceGroup = dropInfo.Data as GroupViewModel;
            var targetGroup = dropInfo.TargetItem as GroupViewModel;
            var targetLed = dropInfo.TargetItem as LedViewModel;

            if (targetGroup != null)
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
            else if (targetLed != null)
            {
                if (sourceLed != null)
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
            var targetGroup = dropInfo.TargetItem as GroupViewModel;
            var targetLed = dropInfo.TargetItem as LedViewModel;

            // Move led from 1 group to a new group
            if (targetGroup != null && sourceLed != null)
            {
                // Remove led from original group
                RemoveLedFromGroup(sourceLed);
                targetGroup.Leds.Add(sourceLed);
            }
            // Reorder groups
            else if (targetGroup != null && sourceGroup != null)
            {
                // Perform default drop behaviour of inserting above??
                var targetIndex = LedsVm.Groups.IndexOf(targetGroup);
                LedsVm.Groups.Remove(sourceGroup);
                LedsVm.Groups.Insert(targetIndex, sourceGroup);
            }
            // Reorder leds
            else if (targetLed != null && sourceLed != null)
            {
                RemoveLedFromGroup(sourceLed);

                var newLedsGroup = GetParentGroup(targetLed);
                var targetIndex = newLedsGroup.Leds.IndexOf(targetLed);
                newLedsGroup.Leds.Insert(targetIndex, sourceLed);
            }
        }

        public void RemoveLedFromGroup(LedViewModel led)
        {
            var parentGroup = GetParentGroup(led);
            if (parentGroup != null)
            {
                parentGroup.Leds.Remove(led);
            }
        }

        public GroupViewModel GetParentGroup(LedViewModel led)
        {
            return LedsVm.Groups.FirstOrDefault(group => group.Leds.Contains(led));
        }
    }
}
