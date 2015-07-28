using System.ComponentModel.Composition;
using System.Windows;
using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    [Export(typeof(IPresentationService))]
    internal class PresentationService : IPresentationService
    {
        // I would prefer to manage the list of ResourceDictionaries required by a module in an
        // own resource dictionary "ModuleResources.xaml". But this won't work because of this WPF bug:
        // http://connect.microsoft.com/VisualStudio/feedback/details/781727/wpf-resourcedictionary-mergeddictionaries-resolve-issue
        private static readonly string[] moduleResources = new string[]
        {
            "Resources/Dictionary.xaml",
        };

        public void Initialize()
        {
            var resourceAssembly = GetType().Assembly;
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            foreach (string resourcePath in moduleResources)
            {
                mergedDictionaries.Add(new ResourceDictionary
                {
                    Source = resourceAssembly.GetPackUri(resourcePath)
                });
            }
        }
    }
}