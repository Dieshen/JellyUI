using System;
using System.Collections.Generic;

namespace VibeUI.Themes.Models
{
    /// <summary>
    /// Options for configuring themes in the application
    /// </summary>
    public class ThemeOptions
    {
        /// <summary>
        /// Gets or sets the default theme ID.
        /// </summary>
        public string DefaultThemeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to persist the theme choice.
        /// </summary>
        public bool PersistTheme { get; set; } = true;

        /// <summary>
        /// Gets or sets the storage key used when persisting theme choice.
        /// </summary>
        public string StorageKey { get; set; } = "vibe-ui-theme";

        /// <summary>
        /// Gets or sets a value indicating whether to load external stylesheets for themes.
        /// </summary>
        public bool LoadExternalStylesheets { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable theme hot-swapping.
        /// </summary>
        public bool EnableHotSwap { get; set; } = true;

        /// <summary>
        /// Gets or sets the built-in themes to include.
        /// </summary>
        public List<string> IncludeBuiltInThemes { get; set; } = new List<string> { "light", "dark" };

        /// <summary>
        /// Gets or sets the external theme providers to enable.
        /// </summary>
        public List<ExternalThemeProvider> EnabledExternalProviders { get; set; } = new List<ExternalThemeProvider>();
    }

    /// <summary>
    /// Represents an external theme provider
    /// </summary>
    public class ExternalThemeProvider
    {
        /// <summary>
        /// Gets or sets the name of the external provider.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the provider.
        /// </summary>
        public ThemeType Type { get; set; }

        /// <summary>
        /// Gets or sets the CDN URL for the provider's stylesheet.
        /// </summary>
        public string CdnUrl { get; set; }

        /// <summary>
        /// Gets or sets the CSS class name prefix for this provider.
        /// </summary>
        public string CssClassPrefix { get; set; }

        /// <summary>
        /// Gets or sets any additional stylesheets to include.
        /// </summary>
        public List<string> AdditionalStylesheets { get; set; } = new List<string>();
    }
}