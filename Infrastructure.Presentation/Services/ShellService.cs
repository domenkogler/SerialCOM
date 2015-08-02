using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Applications;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Presentation
{
    [Export(typeof(IShellService)), Export]
    internal class ShellService : Model, IShellService
    {
        [ImportingConstructor]
        public ShellService(Lazy<IShellViewModel> shellViewModel)
        {
            this.shellViewModel = shellViewModel;
        }

        private readonly Lazy<IShellViewModel> shellViewModel;
        public IView ShellView => shellViewModel.Value.View;

        private void ReplaceDock(object dock, IView view)
        {
            var element = (UIElement) view;
            var dockPanel = ((ShellView) ShellView).DockPanel;
            var dockChildren = dockPanel.Children.OfType<UIElement>();
            foreach (var docked in dockChildren.Where(c => Equals(dock, DockPanel.GetDock(c))).ToArray())
            {
                docked.SetValue(DockPanel.DockProperty, DependencyProperty.UnsetValue);
                dockPanel.Children.Remove(docked);
            }
            element.SetValue(DockPanel.DockProperty, dock);
            try { dockPanel.Children.Add(element); }
            catch(ArgumentException e) { }
        }

        private IView contentView;
        public IView ContentView
        {
            get { return contentView; }
            set
            {
                Set(ref contentView, value);
                ReplaceDock(DependencyProperty.UnsetValue, value);
            }
        }

        private IView topView;
        public IView TopView
        {
            get { return topView; }
            set
            {
                Set(ref topView, value);
                ReplaceDock(Dock.Top, value);
            }
        }

        private IView leftView;
        public IView LeftView
        {
            get { return leftView; }
            set
            {
                Set(ref leftView, value);
                ReplaceDock(Dock.Left, value);
            }
        }

        private IView bottomView;
        public IView BottomView
        {
            get { return bottomView; }
            set
            {
                Set(ref bottomView, value);
                ReplaceDock(Dock.Bottom, value);
            }
        }

        private IView rightView;
        public IView RightView
        {
            get { return rightView; }
            set
            {
                Set(ref rightView, value);
                ReplaceDock(Dock.Right, value);
            }
        }
    }
}
