using System;

namespace Vibe.UI.Services.Toast
{
    /// <summary>
    /// Represents arguments for toast notification events.
    /// </summary>
    public class ToastEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the unique identifier of the toast.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the toast.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description/message of the toast.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the variant/type of the toast.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Gets or sets the icon of the toast.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the duration of the toast in milliseconds.
        /// </summary>
        public int Duration { get; set; }
    }
}