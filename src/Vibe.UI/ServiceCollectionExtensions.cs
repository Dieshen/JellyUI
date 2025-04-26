using Microsoft.Extensions.DependencyInjection;
using VibeUI.Themes.Models;
using VibeUI.Themes.Services;

namespace VibeUI
{
    /// <summary>
    /// Extension methods for registering VibeUI services with dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds VibeUI services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddVibeUI(this IServiceCollection services)
        {
            return AddVibeUI(services, options => { });
        }

        /// <summary>
        /// Adds VibeUI services to the service collection with customized theme options.
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
    }
}