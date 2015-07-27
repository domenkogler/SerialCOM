using System;
using System.ComponentModel.Composition;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Applications
{
    [Export(typeof(IShellService)), Export]
    public class ShellService : Model, IShellService
    {
        [ImportingConstructor]
        public ShellService(Lazy<IShellViewModel> shellViewModel)
        {
            this.shellViewModel = shellViewModel;
        }

        private readonly Lazy<IShellViewModel> shellViewModel;

        public IView ShellView => shellViewModel.Value.View;
        public MenuItemsCollection MenuItems { get; } = new MenuItemsCollection();

        private IView contentView;
        public IView ContentView
        {
            get { return contentView; }
            set { Set(ref contentView, value); }
        }

        public void AddMenuItems(params MenuItem[] items)
        {
            
        }

        public void RemoveMenuItems(params MenuItem[] items)
        {
            
        }
    }
}
