using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Vibe.UI.Themes.Models;

namespace Vibe.UI.Themes.Services
{
    /// <summary>
    /// Service for managing themes in the application
    /// </summary>
    public class ThemeManager : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ThemeOptions _options;
        private readonly List<Theme> _themes = new();
        private Theme _currentTheme;
        private bool _initialized;
        private DotNetObjectReference<ThemeManager> _objRef;

        /// <summary>
        /// Event raised when the theme changes
        /// </summary>
        public event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        /// <summary>
        /// Gets the current theme
        /// </summary>
        public Theme CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets all available themes
        /// </summary>
        public IReadOnlyList<Theme> AvailableThemes => _themes.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the ThemeManager class
        /// </summary>
        /// <param name="jsRuntime">The JS runtime</param>
        /// <param name="options">Theme configuration options</param>
        public ThemeManager(IJSRuntime jsRuntime, ThemeOptions options = null)
        {
            _jsRuntime = jsRuntime;
            _options = options ?? new ThemeOptions();
            _objRef = DotNetObjectReference.Create(this);
        }

        /// <summary>
        /// Initializes the theme manager
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            LoadBuiltInThemes();
            LoadExternalThemeProviders();

            if (_options.PersistTheme)
            {
                var savedThemeId = await _jsRuntime.InvokeAsync<string>(
                    "localStorage.getItem",
                    _options.StorageKey);
                
                if (!string.IsNullOrEmpty(savedThemeId))
                {
                    var savedTheme = _themes.FirstOrDefault(t => t.Id == savedThemeId);
                    if (savedTheme != null)
                    {
                        await SetThemeAsync(savedTheme);
                        _initialized = true;
                        return;
                    }
                }
            }

            // Set default theme
            var defaultTheme = _themes.FirstOrDefault(t => t.Id == _options.DefaultThemeId)
                ?? _themes.FirstOrDefault(t => t.IsDefault)
                ?? _themes.FirstOrDefault();

            if (defaultTheme != null)
            {
                await SetThemeAsync(defaultTheme);
            }

            _initialized = true;
        }

        /// <summary>
        /// Sets the current theme
        /// </summary>
        /// <param name="themeId">The ID of the theme to set</param>
        public async Task SetThemeAsync(string themeId)
        {
            var theme = _themes.FirstOrDefault(t => t.Id == themeId);
            if (theme != null)
            {
                await SetThemeAsync(theme);
            }
        }

        /// <summary>
        /// Sets the current theme
        /// </summary>
        /// <param name="theme">The theme to set</param>
        public async Task SetThemeAsync(Theme theme)
        {
            if (theme == null)
                throw new ArgumentNullException(nameof(theme));

            var oldTheme = _currentTheme;
            _currentTheme = theme;

            await ApplyThemeAsync();

            if (_options.PersistTheme)
            {
                await _jsRuntime.InvokeVoidAsync(
                    "localStorage.setItem",
                    _options.StorageKey,
                    theme.Id);
            }

            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(oldTheme, theme));
        }

        /// <summary>
        /// Registers a new theme
        /// </summary>
        /// <param name="theme">The theme to register</param>
        /// <returns>The registered theme</returns>
        public Theme RegisterTheme(Theme theme)
        {
            if (theme == null)
                throw new ArgumentNullException(nameof(theme));

            if (string.IsNullOrEmpty(theme.Id))
                theme.Id = Guid.NewGuid().ToString();

            if (_themes.Any(t => t.Id == theme.Id))
                throw new ArgumentException($"Theme with ID '{theme.Id}' already exists.", nameof(theme));

            _themes.Add(theme);
            return theme;
        }

        /// <summary>
        /// Creates a new custom theme based on an existing theme
        /// </summary>
        /// <param name="name">The name for the new theme</param>
        /// <param name="baseThemeId">The ID of the theme to base the new theme on</param>
        /// <param name="customVariables">Custom variables to override in the base theme</param>
        /// <returns>The newly created theme</returns>
        public Theme CreateCustomTheme(string name, string baseThemeId, Dictionary<string, string> customVariables)
        {
            var baseTheme = _themes.FirstOrDefault(t => t.Id == baseThemeId);
            if (baseTheme == null)
                throw new ArgumentException($"Base theme with ID '{baseThemeId}' not found.", nameof(baseThemeId));

            var customTheme = new Theme
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Type = ThemeType.Custom,
                ParentThemeId = baseThemeId,
                CssClass = $"vibe-theme-{name.ToLowerInvariant().Replace(" ", "-")}",
                Variables = new Dictionary<string, string>(baseTheme.Variables)
            };

            // Override variables with custom ones
            foreach (var kvp in customVariables)
            {
                customTheme.Variables[kvp.Key] = kvp.Value;
            }

            _themes.Add(customTheme);
            return customTheme;
        }

        /// <summary>
        /// Updates an existing theme
        /// </summary>
        /// <param name="themeId">The ID of the theme to update</param>
        /// <param name="variables">The variables to update</param>
        /// <returns>The updated theme</returns>
        public async Task<Theme> UpdateThemeAsync(string themeId, Dictionary<string, string> variables)
        {
            var theme = _themes.FirstOrDefault(t => t.Id == themeId);
            if (theme == null)
                throw new ArgumentException($"Theme with ID '{themeId}' not found.", nameof(themeId));

            if (theme.Type == ThemeType.BuiltIn)
                throw new InvalidOperationException("Built-in themes cannot be modified.");

            foreach (var kvp in variables)
            {
                theme.Variables[kvp.Key] = kvp.Value;
            }

            if (_currentTheme.Id == themeId)
            {
                await ApplyThemeAsync();
            }

            return theme;
        }

        /// <summary>
        /// Removes a custom theme
        /// </summary>
        /// <param name="themeId">The ID of the theme to remove</param>
        /// <returns>True if the theme was removed; otherwise, false</returns>
        public bool RemoveTheme(string themeId)
        {
            var theme = _themes.FirstOrDefault(t => t.Id == themeId);
            if (theme == null)
                return false;

            if (theme.Type == ThemeType.BuiltIn)
                throw new InvalidOperationException("Built-in themes cannot be removed.");

            if (_currentTheme.Id == themeId)
            {
                throw new InvalidOperationException("Cannot remove the current theme.");
            }

            return _themes.Remove(theme);
        }

        private void LoadBuiltInThemes()
        {
            if (_options.IncludeBuiltInThemes.Contains("light"))
            {
                RegisterBuiltInLightTheme();
            }

            if (_options.IncludeBuiltInThemes.Contains("dark"))
            {
                RegisterBuiltInDarkTheme();
            }
        }

        private void RegisterBuiltInLightTheme()
        {
            var lightTheme = new Theme
            {
                Id = "light",
                Name = "Light",
                Type = ThemeType.BuiltIn,
                IsDefault = true,
                CssClass = "vibe-theme-light",
                Variables = new Dictionary<string, string>
                {
                    ["--vibe-background"] = "#ffffff",
                    ["--vibe-foreground"] = "#111111",
                    ["--vibe-primary"] = "#0066cc",
                    ["--vibe-primary-foreground"] = "#ffffff",
                    ["--vibe-secondary"] = "#f4f4f4",
                    ["--vibe-secondary-foreground"] = "#333333",
                    ["--vibe-accent"] = "#ff4500",
                    ["--vibe-accent-foreground"] = "#ffffff",
                    ["--vibe-muted"] = "#f1f1f1",
                    ["--vibe-muted-foreground"] = "#666666",
                    ["--vibe-card"] = "#ffffff",
                    ["--vibe-card-foreground"] = "#111111",
                    ["--vibe-border"] = "#e2e2e2",
                    ["--vibe-input"] = "#ffffff",
                    ["--vibe-ring"] = "#0066cc",
                    ["--vibe-radius"] = "0.5rem",
                    ["--vibe-font"] = "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif"
                }
            };

            RegisterTheme(lightTheme);
        }

        private void RegisterBuiltInDarkTheme()
        {
            var darkTheme = new Theme
            {
                Id = "dark",
                Name = "Dark",
                Type = ThemeType.BuiltIn,
                CssClass = "vibe-theme-dark",
                Variables = new Dictionary<string, string>
                {
                    ["--vibe-background"] = "#1a1a1a",
                    ["--vibe-foreground"] = "#ffffff",
                    ["--vibe-primary"] = "#0099ff",
                    ["--vibe-primary-foreground"] = "#ffffff",
                    ["--vibe-secondary"] = "#2a2a2a",
                    ["--vibe-secondary-foreground"] = "#f7f7f7",
                    ["--vibe-accent"] = "#ff4500",
                    ["--vibe-accent-foreground"] = "#ffffff",
                    ["--vibe-muted"] = "#313131",
                    ["--vibe-muted-foreground"] = "#a0a0a0",
                    ["--vibe-card"] = "#222222",
                    ["--vibe-card-foreground"] = "#ffffff",
                    ["--vibe-border"] = "#404040",
                    ["--vibe-input"] = "#2a2a2a",
                    ["--vibe-ring"] = "#0099ff",
                    ["--vibe-radius"] = "0.5rem",
                    ["--vibe-font"] = "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif"
                }
            };

            RegisterTheme(darkTheme);
        }

        private void LoadExternalThemeProviders()
        {
            foreach (var provider in _options.EnabledExternalProviders)
            {
                switch (provider.Type)
                {
                    case ThemeType.Material:
                        RegisterMaterialTheme(provider);
                        break;
                    case ThemeType.Bootstrap:
                        RegisterBootstrapTheme(provider);
                        break;
                    case ThemeType.Tailwind:
                        RegisterTailwindTheme(provider);
                        break;
                }
            }
        }

        private void RegisterMaterialTheme(ExternalThemeProvider provider)
        {
            var theme = new Theme
            {
                Id = $"material-{provider.Name.ToLowerInvariant()}",
                Name = $"Material {provider.Name}",
                Type = ThemeType.Material,
                CssClass = $"{provider.CssClassPrefix ?? "material"}-theme",
                ExternalStylesheets = new List<string> { provider.CdnUrl }
            };

            if (provider.AdditionalStylesheets != null)
            {
                theme.ExternalStylesheets.AddRange(provider.AdditionalStylesheets);
            }

            RegisterTheme(theme);
        }

        private void RegisterBootstrapTheme(ExternalThemeProvider provider)
        {
            var theme = new Theme
            {
                Id = $"bootstrap-{provider.Name.ToLowerInvariant()}",
                Name = $"Bootstrap {provider.Name}",
                Type = ThemeType.Bootstrap,
                CssClass = $"{provider.CssClassPrefix ?? "bootstrap"}-theme",
                ExternalStylesheets = new List<string> { provider.CdnUrl }
            };

            if (provider.AdditionalStylesheets != null)
            {
                theme.ExternalStylesheets.AddRange(provider.AdditionalStylesheets);
            }

            RegisterTheme(theme);
        }

        private void RegisterTailwindTheme(ExternalThemeProvider provider)
        {
            var theme = new Theme
            {
                Id = $"tailwind-{provider.Name.ToLowerInvariant()}",
                Name = $"Tailwind {provider.Name}",
                Type = ThemeType.Tailwind,
                CssClass = $"{provider.CssClassPrefix ?? "tailwind"}-theme",
                ExternalStylesheets = new List<string> { provider.CdnUrl }
            };

            if (provider.AdditionalStylesheets != null)
            {
                theme.ExternalStylesheets.AddRange(provider.AdditionalStylesheets);
            }

            RegisterTheme(theme);
        }

        private async Task ApplyThemeAsync()
        {
            if (_currentTheme == null)
                return;

            // Apply CSS variables
            await _jsRuntime.InvokeVoidAsync(
                "eval",
                $@"
                    document.documentElement.className = '{_currentTheme.CssClass}';
                    const style = document.getElementById('vibe-theme-style') || (() => {{
                        const s = document.createElement('style');
                        s.id = 'vibe-theme-style';
                        document.head.appendChild(s);
                        return s;
                    }})();
                    
                    style.textContent = ':root {{ {string.Join(" ", _currentTheme.Variables.Select(v => $"{v.Key}: {v.Value};"))} }}';
                ");

            // Load external stylesheets if needed
            if (_options.LoadExternalStylesheets && _currentTheme.ExternalStylesheets?.Count > 0)
            {
                await _jsRuntime.InvokeVoidAsync(
                    "eval",
                    $@"
                    const stylesheets = {JsonSerializer.Serialize(_currentTheme.ExternalStylesheets)};
                    const existingLinks = Array.from(document.head.querySelectorAll('link[data-vibe-theme]')).map(l => l.href);
                    
                    // Remove old stylesheets
                    document.head.querySelectorAll('link[data-vibe-theme]').forEach(link => {{
                        if (!stylesheets.includes(link.href)) {{
                            link.remove();
                        }}
                    }});
                    
                    // Add new stylesheets
                    stylesheets.forEach(url => {{
                        if (!existingLinks.includes(url)) {{
                            const link = document.createElement('link');
                            link.rel = 'stylesheet';
                            link.href = url;
                            link.setAttribute('data-vibe-theme', 'true');
                            document.head.appendChild(link);
                        }}
                    }});
                ");
            }
        }

        /// <summary>
        /// Disposes the theme manager
        /// </summary>
        public void Dispose()
        {
            _objRef?.Dispose();
        }
    }

    /// <summary>
    /// Event arguments for theme changed events
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the old theme
        /// </summary>
        public Theme OldTheme { get; }

        /// <summary>
        /// Gets the new theme
        /// </summary>
        public Theme NewTheme { get; }

        /// <summary>
        /// Initializes a new instance of the ThemeChangedEventArgs class
        /// </summary>
        /// <param name="oldTheme">The old theme</param>
        /// <param name="newTheme">The new theme</param>
        public ThemeChangedEventArgs(Theme oldTheme, Theme newTheme)
        {
            OldTheme = oldTheme;
            NewTheme = newTheme;
        }
    }
}
