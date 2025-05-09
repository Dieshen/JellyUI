using Microsoft.Extensions.DependencyInjection;
using Vibe.UI.Services.Dialog;
using Vibe.UI.Services.Toast;
using Vibe.UI.Themes.Models;
using Vibe.UI.Themes.Services;

namespace Vibe.UI
{
    /// <summary>
    /// Extension methods for registering Vibe.UI services with dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Vibe.UI services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddVibeUI(this IServiceCollection services)
        {
            return AddVibeUI(services, options => { });
        }

        /// <summary>
        /// Adds Vibe.UI services to the service collection with customized theme options.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">The action to configure theme options.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddVibeUI(this IServiceCollection services, Action<ThemeOptions> configureOptions)
        {
            // Configure theme options
            var options = new ThemeOptions();
            configureOptions(options);
            services.AddSingleton(options);

            // Register the theme manager
            services.AddScoped<ThemeManager>();
            
            // Register toast and dialog services
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IDialogService, DialogService>();

            return services;
        }

        /// <summary>
        /// Adds support for Material Design themes.
        /// </summary>
        /// <param name="options">The theme options.</param>
        /// <returns>The theme options for chaining.</returns>
        public static ThemeOptions AddMaterialTheme(this ThemeOptions options)
        {
            options.EnabledExternalProviders.Add(new ExternalThemeProvider
            {
                Name = "Default",
                Type = ThemeType.Material,
                CdnUrl = "https://cdn.jsdelivr.net/npm/@material/material-components-web@14.0.0/dist/material-components-web.min.css",
                CssClassPrefix = "mdc"
            });
            
            return options;
        }

        /// <summary>
        /// Adds support for Bootstrap themes.
        /// </summary>
        /// <param name="options">The theme options.</param>
        /// <returns>The theme options for chaining.</returns>
        public static ThemeOptions AddBootstrapTheme(this ThemeOptions options)
        {
            options.EnabledExternalProviders.Add(new ExternalThemeProvider
            {
                Name = "Default",
                Type = ThemeType.Bootstrap,
                CdnUrl = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css",
                CssClassPrefix = "bootstrap"
            });
            
            return options;
        }

        /// <summary>
        /// Adds support for Tailwind CSS themes.
        /// </summary>
        /// <param name="options">The theme options.</param>
        /// <returns>The theme options for chaining.</returns>
        public static ThemeOptions AddTailwindTheme(this ThemeOptions options)
        {
            options.EnabledExternalProviders.Add(new ExternalThemeProvider
            {
                Name = "Default",
                Type = ThemeType.Tailwind,
                CdnUrl = "https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css",
                CssClassPrefix = "tailwind"
            });
            
            return options;
        }

        /// <summary>
        /// Applies external stylesheets for the selected theme.
        /// </summary>
        /// <param name="themeManager">The theme manager.</param>
        /// <param name="themeId">The ID of the theme to apply.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task ApplyExternalStylesheetsAsync(this ThemeManager themeManager, string themeId)
        {
            var theme = themeManager.AvailableThemes.FirstOrDefault(t => t.Id == themeId);
            if (theme != null && theme.ExternalStylesheets?.Count > 0)
            {
                await themeManager.SetThemeAsync(theme);
            }
        }
    }
}
