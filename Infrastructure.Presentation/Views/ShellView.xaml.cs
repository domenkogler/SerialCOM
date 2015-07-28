using System.ComponentModel.Composition;
using System.Windows;
using Kogler.SerialCOM.Infrastructure.Applications;

namespace Kogler.SerialCOM.Infrastructure.Presentation
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    [Export(typeof(IShellView))]
    public partial class ShellView : IShellView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        public double VirtualScreenWidth { get { return SystemParameters.VirtualScreenWidth; } }
        public double VirtualScreenHeight { get { return SystemParameters.VirtualScreenHeight; } }
        public bool IsMaximized
        {
            get { return WindowState == WindowState.Maximized; }
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }
    }
}
