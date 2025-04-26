// Theme management interop functions

window.VibeUI = window.VibeUI || {};
window.VibeUI.Themes = window.VibeUI.Themes || {};

/**
 * Theme management interop functions
 */
window.VibeUI.Themes = {
    /**
     * Applies a theme to the document
     * @param {string} themeClass - CSS class for the theme
     * @param {Object} cssVariables - CSS variables to apply
     * @returns {Promise<boolean>} - Promise that resolves when the theme is applied
     */
    applyTheme: function (themeClass, cssVariables) {
        return new Promise((resolve) => {
            document.documentElement.className = themeClass;
            
            let style = document.getElementById('vibe-theme-style');
            if (!style) {
                style = document.createElement('style');
                style.id = 'vibe-theme-style';
                document.head.appendChild(style);
            }

            const cssVars = Object.entries(cssVariables).map(([key, value]) => `${key}: ${value};`).join(' ');
            style.textContent = `:root { ${cssVars} }`;
            
            resolve(true);
        });
    },

    /**
     * Manages external stylesheets for themes
     * @param {Array<string>} stylesheets - URLs of stylesheets to load
     * @returns {Promise<boolean>} - Promise that resolves when stylesheets are managed
     */
    manageStylesheets: function (stylesheets) {
        return new Promise((resolve) => {
            // Get existing theme stylesheets
            const existingLinks = Array.from(document.head.querySelectorAll('link[data-vibe-theme]'))
                .map(link => link.href);
            
            // Remove stylesheets that are no longer needed
            document.head.querySelectorAll('link[data-vibe-theme]').forEach(link => {
                if (!stylesheets.includes(link.href)) {
                    link.remove();
                }
            });
            
            // Add new stylesheets
            stylesheets.forEach(url => {
                if (!existingLinks.includes(url)) {
                    const link = document.createElement('link');
                    link.rel = 'stylesheet';
                    link.href = url;
                    link.setAttribute('data-vibe-theme', 'true');
                    document.head.appendChild(link);
                }
            });
            
            resolve(true);
        });
    },

    /**
     * Gets the saved theme ID from local storage
     * @param {string} storageKey - Key to use in local storage
     * @returns {Promise<string>} - Promise that resolves with the saved theme ID
     */
    getSavedTheme: function (storageKey) {
        return new Promise((resolve) => {
            const savedTheme = localStorage.getItem(storageKey);
            resolve(savedTheme);
        });
    },

    /**
     * Saves the theme ID to local storage
     * @param {string} storageKey - Key to use in local storage
     * @param {string} themeId - Theme ID to save
     * @returns {Promise<boolean>} - Promise that resolves when the theme is saved
     */
    saveTheme: function (storageKey, themeId) {
        return new Promise((resolve) => {
            localStorage.setItem(storageKey, themeId);
            resolve(true);
        });
    },

    /**
     * Registers for system theme change events (light/dark mode)
     * @param {DotNetReference} dotNetRef - Reference to the .NET object
     * @param {string} methodName - Name of the method to call when the system theme changes
     * @returns {Promise<boolean>} - Promise that resolves when the listener is registered
     */
    registerSystemThemeListener: function (dotNetRef, methodName) {
        return new Promise((resolve) => {
            const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
            
            const listener = (e) => {
                const isDarkMode = e.matches;
                dotNetRef.invokeMethodAsync(methodName, isDarkMode);
            };
            
            mediaQuery.addEventListener('change', listener);
            
            // Store the listener reference for later cleanup
            window.VibeUI.Themes._systemThemeListener = {
                mediaQuery: mediaQuery,
                listener: listener,
                dotNetRef: dotNetRef
            };
            
            // Initial call
            listener(mediaQuery);
            
            resolve(true);
        });
    },

    /**
     * Unregisters the system theme listener
     * @returns {Promise<boolean>} - Promise that resolves when the listener is unregistered
     */
    unregisterSystemThemeListener: function () {
        return new Promise((resolve) => {
            const listener = window.VibeUI.Themes._systemThemeListener;
            if (listener) {
                listener.mediaQuery.removeEventListener('change', listener.listener);
                delete window.VibeUI.Themes._systemThemeListener;
            }
            resolve(true);
        });
    },

    /**
     * Gets the current system theme (light or dark)
     * @returns {Promise<string>} - Promise that resolves with 'dark' or 'light'
     */
    getSystemTheme: function () {
        return new Promise((resolve) => {
            const isDarkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;
            resolve(isDarkMode ? 'dark' : 'light');
        });
    }
};