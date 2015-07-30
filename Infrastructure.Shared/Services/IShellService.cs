using System.ComponentModel;
using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    /// <summary>
    /// Exposes the functionality of the shell.
    /// </summary>
    public interface IShellService : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the shell view. Use this object as owner when you need to show a modal dialog.
        /// </summary>
        IView ShellView { get; }

        /// <summary>
        /// Gets or sets the views which is shown by the shell.
        /// </summary>
        IView ContentView { get; set; }
        IView TopView { get; set; }
        IView LeftView { get; set; }
        IView BottomView { get; set; }
        IView RightView { get; set; }
    }
}
