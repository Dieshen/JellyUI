using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Vibe.UI.Services.Dialog
{
    /// <summary>
    /// Service for displaying modal dialogs.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a simple alert dialog with an OK button.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="okText">Text for the OK button.</param>
        /// <returns>A task that completes when the dialog is closed.</returns>
        Task ShowAlertAsync(string title, string message, string okText = "OK");

        /// <summary>
        /// Shows a confirmation dialog with OK and Cancel buttons.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="okText">Text for the OK button.</param>
        /// <param name="cancelText">Text for the Cancel button.</param>
        /// <returns>True if the user clicked OK, false otherwise.</returns>
        Task<bool> ShowConfirmAsync(string title, string message, string okText = "OK", string cancelText = "Cancel");

        /// <summary>
        /// Shows a prompt dialog with a text input.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="defaultValue">The default value for the input.</param>
        /// <param name="okText">Text for the OK button.</param>
        /// <param name="cancelText">Text for the Cancel button.</param>
        /// <returns>The entered value, or null if the user cancelled.</returns>
        Task<string> ShowPromptAsync(string title, string message, string defaultValue = "", string okText = "OK", string cancelText = "Cancel");

        /// <summary>
        /// Shows a custom dialog using the specified component type.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="componentType">The type of the component to render.</param>
        /// <param name="parameters">Parameters to pass to the component.</param>
        /// <returns>The dialog result or null if cancelled/dismissed.</returns>
        Task<object> ShowCustomAsync(string title, Type componentType, DialogParameters parameters = null);

        /// <summary>
        /// Shows a custom dialog using the specified render fragment.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content to render in the dialog.</param>
        /// <returns>The dialog result or null if cancelled/dismissed.</returns>
        Task<object> ShowCustomAsync(string title, RenderFragment content);

        /// <summary>
        /// Closes the currently open dialog with the specified result.
        /// </summary>
        /// <param name="result">The result to return.</param>
        void Close(object result = null);

        /// <summary>
        /// Event raised when a dialog is opened.
        /// </summary>
        event EventHandler<DialogOpenedEventArgs> OnDialogOpened;
        
        /// <summary>
        /// Event raised when a dialog is closed.
        /// </summary>
        event EventHandler<DialogClosedEventArgs> OnDialogClosed;
    }
}