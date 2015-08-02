using System.ComponentModel.Composition;
using System.Windows;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Applications;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Presentation
{
    [Export(typeof(IPresentationService))]
    internal class PresentationService : IPresentationService
    {
        [ImportingConstructor]
        public PresentationService(IShellService shellService, ITabsViewModel tabsViewModel, IMenuViewModel menuViewModel)
        {
            this.shellService = shellService;
            this.tabsViewModel = tabsViewModel;
            this.menuViewModel = menuViewModel;
        }

        private readonly IShellService shellService;
        private readonly ITabsViewModel tabsViewModel;
        private readonly IMenuViewModel menuViewModel;


        // I would prefer to manage the list of ResourceDictionaries required by a module in an
        // own resource dictionary "ModuleResources.xaml". But this won't work because of this WPF bug:
        // http://connect.microsoft.com/VisualStudio/feedback/details/781727/wpf-resourcedictionary-mergeddictionaries-resolve-issue
        private static readonly string[] moduleResources = new string[]
        {
            //"Resources/ConverterResources.xaml",
        };

        public void Initialize()
        {
            var resourceAssembly = GetType().Assembly;
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            foreach (var resourcePath in moduleResources)
            {
                mergedDictionaries.Add(new ResourceDictionary { Source = resourceAssembly.GetPackUri(resourcePath) });
            }
            
            shellService.TopView = menuViewModel.View;
            shellService.ContentView = tabsViewModel.View;
        }
    }
}