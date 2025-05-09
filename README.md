Vibe.UI
======

Vibe.UI is a powerful theme-enabled Blazor component library inspired by Shadcn UI, built primarily with Razor components and C#. This library provides comprehensive theme management capabilities along with beautiful, responsive UI components.

## Features

- **Comprehensive Theme Management** - Create, customize, and switch between themes at runtime
- **Built-in Light and Dark Themes** - Ready to use out of the box
- **Support for External CSS Frameworks** - Integrate with Material, Bootstrap, and Tailwind CSS
- **Customizable CSS Variables** - Fine-tune theming with an extensive set of CSS variables
- **Auto-Detection of System Theme** - Automatically adapt to user's OS theme preference
- **Pure Razor Components** - Built with Razor/C# with minimal JavaScript
- **45+ Accessible UI Components** - From simple buttons to complex data tables and date pickers
- **Complete Theming API** - Programmatic control over themes via C# API
- **Theme Persistence** - Save user theme preferences across sessions
- **Fully Shadcn UI Compatible** - Implementation matches Shadcn UI components for easy migration

## Getting Started

### Installation

First, add the Vibe.UI library to your Blazor project:

```bash
dotnet add package Vibe.UI
```

### Register Services

In your `Program.cs` file (or `Startup.cs` for Blazor Server), register the Vibe.UI services:

```csharp
using Vibe.UI;

// ...

// Basic setup with default options
builder.Services.AddVibe.UI();

// OR with customized options
builder.Services.AddVibe.UI(options => {
    options.DefaultThemeId = "dark"; 
    options.PersistTheme = true;
    
    // Add external theme providers if needed
    options.AddMaterialTheme()
           .AddBootstrapTheme()
           .AddTailwindTheme();
});
```

### Add Theme Root Component

In your `App.razor` file, wrap your application with the `ThemeRoot` component:

```razor
@using Vibe.UI.Components

<ThemeRoot>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
        <NotFound>
            <PageTitle>Not found</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</ThemeRoot>
```

### Add JavaScript Import

In your `index.html` (for Blazor WebAssembly) or `_Host.cshtml` (for Blazor Server) file, make sure to add the Vibe.UI JavaScript:

```html
<script src="_content/Vibe.UI/themeInterop.js"></script>
```

## Using Vibe.UI Components

### Button

```razor
<Button Variant="ButtonVariant.Primary" Size="ButtonSize.Medium">
    Click Me
</Button>
```

### Alert

```razor
<Alert Type="AlertType.Info" Title="Information" IsDismissible="true">
    This is an informational message.
</Alert>
```

### Card

```razor
<Card>
    <CardHeader>
        <CardTitle>Card Title</CardTitle>
        <CardDescription>Card Description</CardDescription>
    </CardHeader>
    <CardContent>
        Card content goes here.
    </CardContent>
    <CardFooter>
        <Button>Submit</Button>
    </CardFooter>
</Card>
```

### Theme Components

Choose from several theme-related components to enhance your user experience:

```razor
<!-- Simple theme toggle between light and dark -->
<ThemeToggle />

<!-- Complete dropdown theme selector -->
<ThemeSelector Label="Choose theme:" />

<!-- Full theme customization panel -->
<ThemePanel 
    Title="Theme Settings"
    CollapsiblePanel="true" 
    InitiallyExpanded="false"
    AllowCustomization="true"
    AllowCreateTheme="true" />
```

## Creating Custom Themed Components

Extend the `ThemedComponentBase` class to create your own themed components:

```razor
@inherits Vibe.UI.Base.ThemedComponentBase

<div class="@CombinedClass">
    <!-- Your component's HTML -->
    <h1>My Custom Component</h1>
    @ChildContent
</div>

@code {
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    // Override to provide your component's CSS class
    protected override string ComponentClass => "my-custom-component";
}
```

## Theme Customization

### Creating Custom Themes

You can create custom themes programmatically:

```csharp
@inject ThemeManager ThemeManager

@code {
    protected override async Task OnInitializedAsync()
    {
        // Create a custom theme based on the light theme
        var customTheme = ThemeManager.CreateCustomTheme(
            "My Custom Theme",
            "light",
            new Dictionary<string, string>
            {
                ["--vibe-primary"] = "#ff5722",
                ["--vibe-accent"] = "#2196f3"
            });
            
        // Apply the custom theme
        await ThemeManager.SetThemeAsync(customTheme.Id);
    }
}
```

### Accessing the Current Theme

You can access the current theme in any component:

```razor
@inject ThemeManager ThemeManager

<p>Current theme: @ThemeManager.CurrentTheme?.Name</p>
```

### Using System Theme Detection

Enable automatic system theme detection in your `ThemeRoot` component:

```razor
<ThemeRoot AutoDetectSystemTheme="true">
    <!-- App content -->
</ThemeRoot>
```

### Adding External CSS Framework Themes

You can integrate external CSS frameworks:

```csharp
builder.Services.AddVibe.UI(options => {
    // Configure Material Design themes
    options.AddMaterialTheme(provider => {
        provider.Name = "Light";
        provider.CdnUrl = "https://cdn.jsdelivr.net/npm/@material/material-components-web@14.0.0/dist/material-components-web.min.css";
    });
    
    // Configure Bootstrap themes
    options.AddBootstrapTheme(provider => {
        provider.Name = "Default";
        provider.CdnUrl = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css";
    });
    
    // Configure Tailwind themes
    options.AddTailwindTheme(provider => {
        provider.Name = "Default";
        provider.CdnUrl = "https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css";
    });
});
```

## CSS Variables

Vibe.UI uses CSS variables for theming. Here are the main variables:

| Variable | Purpose |
|----------|---------|
| `--vibe-background` | Background color |
| `--vibe-foreground` | Text color |
| `--vibe-primary` | Primary color |
| `--vibe-primary-foreground` | Text color on primary background |
| `--vibe-secondary` | Secondary color |
| `--vibe-secondary-foreground` | Text color on secondary background |
| `--vibe-accent` | Accent color |
| `--vibe-accent-foreground` | Text color on accent background |
| `--vibe-muted` | Muted background color |
| `--vibe-muted-foreground` | Text color on muted background |
| `--vibe-card` | Card background color |
| `--vibe-card-foreground` | Text color on card background |
| `--vibe-border` | Border color |
| `--vibe-input` | Input field background color |
| `--vibe-ring` | Focus ring color |
| `--vibe-radius` | Border radius |
| `--vibe-font` | Font family |

## Available Components

Vibe.UI includes a comprehensive set of over 45 components:

### Layout Components
- **AspectRatio** - Container maintaining a specific aspect ratio
- **Card** - Versatile content container with header, body, and footer sections
- **Separator** - Visual divider for content separation
- **Resizable** - Panels with user-adjustable dimensions
- **Sheet** - Slide-in panel for secondary content

### Data Display Components
- **Avatar** - User or entity image representations
- **Badge** - Small status indicators or counts
- **Table** - Standard data table for structured information
- **DataTable** - Enhanced table with sorting, filtering, and pagination
- **Progress** - Visual indicators of completion percentage or activity

### Navigation Components
- **Breadcrumb** - Path-based navigation indicators
- **Menubar** - Horizontal navigation system
- **NavigationMenu** - Hierarchical navigation structure
- **Pagination** - Page navigation controls
- **Tabs** - Content organization into selectable tabs

### Input Components
- **Button** - Clickable controls with multiple variants
- **Checkbox** - Binary selection controls
- **Input** - Text input fields
- **Radio** - Single-selection option buttons
- **Select** - Dropdown selection menu
- **Slider** - Range selection control
- **Switch** - Toggle controls
- **TextArea** - Multi-line text input
- **Toggle** - Alternative toggle control
- **ColorPicker** - Visual color selection tool
- **MultiSelect** - Selection control for multiple options

### Disclosure Components
- **Accordion** - Expandable content sections
- **Collapsible** - Simple show/hide content panels
- **Carousel** - Content slideshow with navigation

### Overlay Components
- **AlertDialog** - Modal dialog requiring user attention
- **Dialog** - Standard modal window
- **Drawer** - Side-sliding panel
- **ContextMenu** - Right-click activated menus
- **HoverCard** - Content preview on hover
- **Popover** - Small content overlay on click
- **Tooltip** - Small information popup on hover

### Feedback Components
- **Alert** - Contextual feedback messages
- **Skeleton** - Loading state placeholders
- **Toast** - Temporary notification messages

### Date & Time Components
- **Calendar** - Date selection calendar
- **DatePicker** - Date selection with popup calendar
- **DateRangePicker** - Selection control for date ranges

### Form Components
- **Form** - Organized form container
- **FormField** - Structured form field wrapper
- **FormLabel** - Accessible form input labels
- **FormMessage** - Validation and help text
- **Combobox** - Combined input and dropdown

### Theme Components
- **ThemeRoot** - Root theme provider component
- **ThemeSelector** - UI for selecting themes
- **ThemeToggle** - Light/dark mode toggle
- **ThemePanel** - Complete theme customization panel

### Utility Components
- **Command** - Keyboard command palette interface
- **ScrollArea** - Custom scrollable container
- **DropdownMenu** - Context-specific dropdown menu

## License

MIT License
