using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Applications
{
    public interface ITabsService
    {
        ObservableCollection<ViewModel> Tabs { get; }

        ViewModel Active { get; set; }
    }

    [Export(typeof(ITabsService)), Export]
    internal class TabsService : Model, ITabsService
    {
        public ObservableCollection<ViewModel> Tabs { get; } = new ObservableCollection<ViewModel>();

        private ViewModel active;
        public ViewModel Active
        {
            get { return active; }
            set { Set(ref active, value); }
        }
    }
}