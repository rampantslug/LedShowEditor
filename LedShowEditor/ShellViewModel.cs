using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace LedShowEditor
{

    /// <summary>
    /// 
    /// </summary>
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.AllActive, IShell
    {

        private readonly IEventAggregator _eventAggregator;


        /// <summary>
        /// Constructor for the ShellViewModel. Main container for Client application elements.
        /// Imports required UI elements via DI.
        /// </summary>
        /// <param name="eventAggregator"></param>    
        [ImportingConstructor]
        public ShellViewModel(
            IEventAggregator eventAggregator
            )
        {
            _eventAggregator = eventAggregator;

        }



        protected override void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);

            _eventAggregator.Subscribe(this);

        }





    }
}