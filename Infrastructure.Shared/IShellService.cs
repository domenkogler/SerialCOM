﻿using System.ComponentModel;
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
        /// Gets or sets the content view which is shown by the shell.
        /// </summary>
        IView ContentView { get; set; }
        
        /// <summary>
        /// Adds the specified menu items.
        /// </summary>
        /// <param name="items">The tool bar commands.</param>
        void AddMenuItems(params MenuItem[] items);

        /// <summary>
        /// Clears the menu items.
        /// </summary>
        void RemoveMenuItems(params MenuItem[] items);
    }
}
