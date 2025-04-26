using System;
using Microsoft.AspNetCore.Components;

namespace Vibe.UI.Services.Dialog
{
    /// <summary>
    /// Base class for dialog event arguments.
    /// </summary>
    public class DialogEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the dialog ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Event arguments for when a dialog is opened.
    /// </summary>
    public class DialogOpenedEventArgs : DialogEventArgs
    {
        /// <summary>
        /// Gets or sets the dialog content.
        /// </summary>
        public RenderFragment Content { get; set; }
    }

    /// <summary>
    /// Event arguments for when a dialog is closed.
    /// </summary>
    public class DialogClosedEventArgs : DialogEventArgs
    {
        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        public object Result { get; set; }
    }
}