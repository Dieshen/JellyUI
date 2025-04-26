# Vibe UI Services

This document provides guidance on using the services provided by Vibe UI.

## Table of Contents

- [Toast Service](#toast-service)
- [Dialog Service](#dialog-service)

## Toast Service

The Toast Service allows you to programmatically display toast notifications in your application.

### Setup

1. Register the service in your application:

```csharp
// This is done automatically when calling services.AddVibe.UI()
```

2. Add the ToastContainer component to your application layout:

```razor
<ToastContainer Position="bottom-right" MaxToasts="5" />
```

3. Inject the IToastService where needed:

```csharp
@inject Vibe.UI.Services.Toast.IToastService ToastService
```

### Usage

```csharp
// Show a default toast
await ToastService.ShowAsync("Title", "This is a notification message");

// Show a success toast
await ToastService.ShowSuccessAsync("Success", "Operation completed successfully");

// Show an error toast
await ToastService.ShowErrorAsync("Error", "An error occurred");

// Show a warning toast
await ToastService.ShowWarningAsync("Warning", "This action may have consequences");

// Show an info toast
await ToastService.ShowInfoAsync("Info", "Here's some information");

// Show a custom toast with longer duration (10 seconds)
await ToastService.ShowCustomAsync("Custom Toast", "This is a custom toast", "info", null, 10000);
```

## Dialog Service

The Dialog Service allows you to programmatically display modal dialogs in your application.

### Setup

1. Register the service in your application:

```csharp
// This is done automatically when calling services.AddVibe.UI()
```

2. Add the DialogContainer component to your application layout:

```razor
<DialogContainer />
```

3. Inject the IDialogService where needed:

```csharp
@inject Vibe.UI.Services.Dialog.IDialogService DialogService
```

### Usage

```csharp
// Show a simple alert dialog
await DialogService.ShowAlertAsync("Information", "This is an alert message");

// Show a confirmation dialog and handle the result
var confirmed = await DialogService.ShowConfirmAsync("Confirm Action", 
                                                   "Are you sure you want to proceed?", 
                                                   "Yes", "No");
if (confirmed)
{
    // User confirmed the action
}
else
{
    // User cancelled the action
}

// Show a prompt dialog and get user input
var userInput = await DialogService.ShowPromptAsync("User Input", 
                                                  "Please enter your name:", 
                                                  "John Doe");
if (userInput != null)
{
    // User entered: userInput
}

// Show a custom dialog with a component
var parameters = new DialogParameters()
    .Add("UserId", 123)
    .Add("UserName", "John Doe");

var result = await DialogService.ShowCustomAsync("User Profile", 
                                               typeof(UserProfileDialog), 
                                               parameters);
```

### Custom Dialog Components

You can create custom dialog components to use with the Dialog Service. Here's an example:

```razor
@inject IDialogService DialogService

<div class="custom-dialog-content">
    <h3>User Profile: @UserName</h3>
    <p>ID: @UserId</p>
    <div class="dialog-actions">
        <button @onclick="() => DialogService.Close('save')">Save Changes</button>
        <button @onclick="() => DialogService.Close()">Cancel</button>
    </div>
</div>

@code {
    [Parameter]
    public int UserId { get; set; }
    
    [Parameter]
    public string UserName { get; set; }
}
```

The DialogService.Close method can be called with any object that you want to return as the dialog result.