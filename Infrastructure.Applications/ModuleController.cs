using System;
using System.ComponentModel.Composition;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Applications
{
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : IModuleController
    {
        [ImportingConstructor]
        public ModuleController(Lazy<IShellViewModel> shellViewModel, IMenuViewModel menuViewModel, ITabsViewModel tabsViewModel, IShellService shellService)
        {
            this.shellViewModel = shellViewModel;
            this.menuViewModel = menuViewModel;
            this.tabsViewModel = tabsViewModel;
            this.shellService = shellService;
        }

        private readonly Lazy<IShellViewModel> shellViewModel;
        private readonly IMenuViewModel menuViewModel;
        private readonly ITabsViewModel tabsViewModel;
        private readonly IShellService shellService;

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
            shellService.TopView = menuViewModel.View;
            shellService.ContentView = tabsViewModel.View;
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