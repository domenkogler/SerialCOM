﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Applications;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Presentation
{
    internal class DesignShellViewModel : IShellViewModel
    {
        public DesignShellViewModel()
        {
            //ShellService.MenuItems.AddToHierarchy(
            //    new MenuItem {GroupName = "Menu1", Text = "Command1"},
            //    new MenuItem {GroupName = "Menu2", Text = "Command 2"});
        }

        public IView View { get; }
        public string Title { get; } = ApplicationInfo.ProductName;
        public ICommand AboutCommand { get; }
        public IShellService ShellService { get; } = new MockShellService();
        public ITabsService TabsService { get; } = new MockTabsService();
        public void Show() { }

        private class MockShellService : IShellService
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public IView ShellView { get; }
            public IView ContentView { get; set; }
            public IView TopView { get; set; }
            public IView LeftView { get; set; }
            public IView BottomView { get; set; }
            public IView RightView { get; set; }
        }

        private class MockTabsService : ITabsService
        {
            public ObservableCollection<ViewModel> Tabs { get; } = new ObservableCollection<ViewModel>();
            public ViewModel Active { get; set; }
        }
    }
}