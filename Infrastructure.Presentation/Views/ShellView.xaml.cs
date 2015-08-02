using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Kogler.SerialCOM.Infrastructure.Applications;

namespace Kogler.SerialCOM.Infrastructure.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export(typeof(IShellView))]
    public partial class ShellView : Window, IShellView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        public double VirtualScreenWidth => SystemParameters.VirtualScreenWidth;
        public double VirtualScreenHeight => SystemParameters.VirtualScreenHeight;

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

