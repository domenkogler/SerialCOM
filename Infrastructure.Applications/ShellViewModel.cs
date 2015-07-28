using System;
using System.ComponentModel.Composition;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Applications
{
    public interface IShellViewModel : IViewModel
    {
        ICommand AboutCommand { get; }
        IShellService ShellService { get; }
        IDocumentsService DocumentsService { get; }
        void Show();
        string Title { get; }
    }

    [Export, Export(typeof(IShellViewModel))]
    internal class ShellViewModel : ViewModel<IShellView>, IShellViewModel
    {
        [ImportingConstructor]
        public ShellViewModel(IShellView view, IMessageService messageService, IShellService shellService, IDocumentsService documentsService) : base(view)
        {
            this.messageService = messageService;
            ShellService = shellService;
            DocumentsService = documentsService;

            view.Closed += ViewClosed;

            //// Restore the window size when the values are valid.
            //if (Settings.Default.Left >= 0 && Settings.Default.Top >= 0 && Settings.Default.Width > 0 && Settings.Default.Height > 0
            //    && Settings.Default.Left + Settings.Default.Width <= view.VirtualScreenWidth
            //    && Settings.Default.Top + Settings.Default.Height <= view.VirtualScreenHeight)
            //{
            //    ViewCore.Left = Settings.Default.Left;
            //    ViewCore.Top = Settings.Default.Top;
            //    ViewCore.Height = Settings.Default.Height;
            //    ViewCore.Width = Settings.Default.Width;
            //}
            //ViewCore.IsMaximized = Settings.Default.IsMaximized;

            AboutCommand = new Command(ShowAboutMessage);

            ShellService.MenuItems.AddToHierarchy(new MenuItem {GroupName = "Help", Text = "About", Command = AboutCommand});
        }

        private readonly IMessageService messageService;

        public string Title => ApplicationInfo.ProductName;
        public IShellService ShellService { get; }
        public IDocumentsService DocumentsService { get; }

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            //Settings.Default.Left = ViewCore.Left;
            //Settings.Default.Top = ViewCore.Top;
            //Settings.Default.Height = ViewCore.Height;
            //Settings.Default.Width = ViewCore.Width;
            //Settings.Default.IsMaximized = ViewCore.IsMaximized;
        }

        public ICommand AboutCommand { get; }

        private void ShowAboutMessage()
        {
            messageService.ShowMessage(View, $"Medicine device data collector and viewer.\n\n{ApplicationInfo.ProductName} {ApplicationInfo.Version}");
        }
    }
}