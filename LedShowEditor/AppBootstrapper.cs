using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;

namespace LedShowEditor
{
    public class AppBootstrapper : BootstrapperBase
    {
        protected CompositionContainer Container;

        public AppBootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// Add various services that we require to the container
        /// </summary>
        protected override void Configure()
        {
            Container = new CompositionContainer(new AggregateCatalog(
                AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));

            var batch = new CompositionBatch();

            var window = new WindowManager();
            batch.AddExportedValue<IWindowManager>(window);

            var eventAggregator = new EventAggregator();
            batch.AddExportedValue<IEventAggregator>(eventAggregator);

            batch.AddExportedValue(Container);

            Container.Compose(batch);

            MessageBinder.SpecialValues.Add("$pressedkey", (context) =>
            {
                // NOTE: IMPORTANT - you MUST add the dictionary key as lowercase as CM
                // does a ToLower on the param string you add in the action message, in fact ideally
                // all your param messages should be lowercase just in case. I don't really like this
                // behaviour but that's how it is!
                var keyArgs = context.EventArgs as KeyEventArgs;

                if (keyArgs != null)
                    return keyArgs.Key;

                return null;
            });
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            Container.SatisfyImportsOnce(instance);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                Assembly.GetExecutingAssembly()
            };
        }

        /// <summary>
        /// Set the View to use Shell on startup
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">startup event args</param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}