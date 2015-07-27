using System;
using System.ComponentModel.Composition;
using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Applications
{
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : IModuleController
    {
        [ImportingConstructor]
        public ModuleController(Lazy<IShellViewModel> shellViewModel)
        {
            this.shellViewModel = shellViewModel;
        }

        private readonly Lazy<IShellViewModel> shellViewModel;

        public void Initialize()
        {
            // Upgrade the settings from a previous version when the new version starts the first time.
            //if (Settings.Default.IsUpgradeNeeded)
            //{
            //    Settings.Default.Upgrade();
            //    Settings.Default.IsUpgradeNeeded = false;
            //}

            //DocumentController.Initialize();
        }

        public void Run()
        {
            shellViewModel.Value.Show();
        }

        public void Shutdown()
        {
            //DocumentController.Shutdown();

            try
            {
                //Settings.Default.Save();
            }
            catch (Exception)
            {
                // When more application instances are closed at the same time then an exception occurs.
            }
        }
    }
}