using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VibeUI.Themes.Models
{
    /// <summary>
    /// Represents a theme in the application
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Gets or sets the unique identifier for the theme.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the theme.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the theme type (Built-in, Custom, Material, Bootstrap, Tailwind, etc.)
        /// </summary>
        public ThemeType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the default theme.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of CSS variables for this theme.
        /// </summary>
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the CSS class name for this theme.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets any external stylesheet URLs associated with this theme.
        /// </summary>
        public List<string> ExternalStylesheets { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the parent theme ID, if this is a derived theme.
        /// </summary>
        public string ParentThemeId { get; set; }

        /// <summary>
        /// Creates a new instance of the Theme class.
        /// </summary>
        public Theme()
        {
        }

        /// <summary>
        /// Creates a new instance of the Theme class with the specified name and type.
        /// </summary>
        /// <param name="name">The name of the theme.</param>
        /// <param name="type">The theme type.</param>
        public Theme(string name, ThemeType type)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Type = type;
        }
    }

    /// <summary>
    /// Represents the type of a theme.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ThemeType
    {
        /// <summary>
        /// A built-in theme provided by the library.
        /// </summary>
        BuiltIn,

        /// <summary>
        /// A custom theme created by the user.
        /// </summary>
        Custom,

        /// <summary>
        /// A Material Design theme.
        /// </summary>
        Material,

        /// <summary>
        /// A Bootstrap theme.
        /// </summary>
        Bootstrap,

        /// <summary>
        /// A Tailwind CSS theme.
        /// </summary>
        Tailwind
    }
}