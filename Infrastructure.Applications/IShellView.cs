using System;
using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Applications
{
    public interface IShellView : IView
    {
        double VirtualScreenWidth { get; }
        double VirtualScreenHeight { get; }
        double Left { get; set; }
        double Top { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        bool IsMaximized { get; set; }

        event EventHandler Closed;

        void Show();
        void Close();
    }
}