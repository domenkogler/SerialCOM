using System;
using System.ComponentModel.Composition;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Applications
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

        private IView contentView;
        public IView ContentView
        {
            get { return contentView; }
            set { Set(ref contentView, value); }
        }

        private IView leftView;
        public IView LeftView
        {
            get { return leftView; }
            set { Set(ref leftView, value); }
        }

        private IView topView;
        public IView TopView
        {
            get { return topView; }
            set { Set(ref topView, value); }
        }

        private IView bottomView;
        public IView BottomView
        {
            get { return bottomView; }
            set { Set(ref bottomView, value); }
        }

        private IView rightView;
        public IView RightView
        {
            get { return rightView; }
            set { Set(ref rightView, value); }
        }
    }
}
