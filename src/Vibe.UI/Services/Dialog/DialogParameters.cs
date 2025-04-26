using System;
using System.Collections.Generic;

namespace Vibe.UI.Services.Dialog
{
    /// <summary>
    /// Represents parameters passed to a dialog.
    /// </summary>
    public class DialogParameters
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        /// <summary>
        /// Adds a parameter with the specified name and value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The current DialogParameters instance.</returns>
        public DialogParameters Add(string name, object value)
        {
            _parameters[name] = value;
            return this;
        }

        /// <summary>
        /// Gets a parameter value by name.
        /// </summary>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        /// <param name="name">The parameter name.</param>
        /// <returns>The parameter value.</returns>
        public T Get<T>(string name)
        {
            if (_parameters.TryGetValue(name, out var value))
            {
                return (T)value;
            }

            return default;
        }

        /// <summary>
        /// Tries to get a parameter value by name.
        /// </summary>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>True if the parameter exists; otherwise, false.</returns>
        public bool TryGet<T>(string name, out T value)
        {
            value = default;
            if (_parameters.TryGetValue(name, out var obj))
            {
                value = (T)obj;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a dictionary containing all parameters.
        /// </summary>
        /// <returns>A dictionary of parameters.</returns>
        public Dictionary<string, object> GetAll()
        {
            return new Dictionary<string, object>(_parameters);
        }

        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        public int Count => _parameters.Count;
    }
}