using System;
using System.Threading.Tasks;

namespace Vibe.UI.Services.Toast
{
    /// <summary>
    /// Service for displaying toast notifications.
    /// </summary>
    public interface IToastService
    {
        /// <summary>
        /// Shows a default toast notification.
        /// </summary>
        /// <param name="title">The title of the toast.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds. Default is 5000ms.</param>
        /// <returns>A task representing the operation.</returns>
        Task ShowAsync(string title, string message = null, int duration = 5000);

        /// <summary>
        /// Shows a success toast notification.
        /// </summary>
        /// <param name="title">The title of the toast.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds. Default is 5000ms.</param>
        /// <returns>A task representing the operation.</returns>
        Task ShowSuccessAsync(string title, string message = null, int duration = 5000);

        /// <summary>
        /// Shows an error toast notification.
        /// </summary>
        /// <param name="title">The title of the toast.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds. Default is 5000ms.</param>
        /// <returns>A task representing the operation.</returns>
        Task ShowErrorAsync(string title, string message = null, int duration = 5000);

        /// <summary>
        /// Shows a warning toast notification.
        /// </summary>
        /// <param name="title">The title of the toast.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds. Default is 5000ms.</param>
        /// <returns>A task representing the operation.</returns>
        Task ShowWarningAsync(string title, string message = null, int duration = 5000);

        /// <summary>
        /// Shows an info toast notification.
        /// </summary>
        /// <param name="title">The title of the toast.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="duration">Duration in milliseconds. Default is 5000ms.</param>
        /// <returns>A task representing the operation.</returns>
        Task ShowInfoAsync(string title, string message = null, int duration = 5000);

        /// <summary>
        /// Shows a custom toast notification.
        /// </summary>
        /// <param name="title">The title of the toast.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="variant">The variant style (default, success, error, warning, info).</param>
        /// <param name="icon">Optional icon to display.</param>
        /// <param name="duration">Duration in milliseconds.</param>
        /// <returns>A task representing the operation.</returns>
        Task ShowCustomAsync(string title, string message, string variant, string icon = null, int duration = 5000);

        /// <summary>
        /// Event raised when a toast notification is added.
        /// </summary>
        event EventHandler<ToastEventArgs> OnToastAdded;
        
        /// <summary>
        /// Event raised when a toast notification is removed.
        /// </summary>
        event EventHandler<ToastEventArgs> OnToastRemoved;
    }
}