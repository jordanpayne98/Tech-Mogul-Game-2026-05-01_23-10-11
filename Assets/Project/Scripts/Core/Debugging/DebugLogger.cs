using Project.Core.Interfaces;
using UnityEngine;

namespace Project.Core.Debugging
{
    /// <summary>
    /// Centralized static logging API. All project code must use this instead of raw Debug.Log.
    /// Must be initialized by the bootstrapper before use via Initialize(IDebugConfig).
    /// If called before initialization, logs a single warning and falls back to raw Debug.Log.
    /// </summary>
    public static class DebugLogger
    {
        private static IDebugConfig _config;
        private static bool _uninitializedWarningLogged;

        // -------------------------------------------------------------------------
        // Initialization
        // -------------------------------------------------------------------------

        /// <summary>
        /// Registers the active DebugConfig. Called once by the bootstrapper.
        /// </summary>
        public static void Initialize(IDebugConfig config)
        {
            _config = config;
            _uninitializedWarningLogged = false;
        }

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        public static void Log(DebugCategory category, string message, Object context = null)
        {
            if (!IsLoggingEnabled(category))
            {
                return;
            }

            string formatted = FormatMessage(category, message, LogLevel.Log);
            Debug.Log(formatted, context);
        }

        public static void LogWarning(DebugCategory category, string message, Object context = null)
        {
            if (!IsLoggingEnabled(category))
            {
                return;
            }

            string formatted = FormatMessage(category, message, LogLevel.Warning);
            Debug.LogWarning(formatted, context);
        }

        public static void LogError(DebugCategory category, string message, Object context = null)
        {
            if (!IsLoggingEnabled(category))
            {
                return;
            }

            string formatted = FormatMessage(category, message, LogLevel.Error);
            Debug.LogError(formatted, context);
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        private enum LogLevel
        {
            Log,
            Warning,
            Error
        }

        private static bool IsLoggingEnabled(DebugCategory category)
        {
            if (_config != null)
            {
                return _config.ShouldLog(category);
            }

            // Pre-bootstrap fallback: emit one warning, then pass through.
            if (!_uninitializedWarningLogged)
            {
                _uninitializedWarningLogged = true;
                Debug.LogWarning("[DebugLogger] Not initialized. Call DebugLogger.Initialize(config) in the bootstrapper. Falling back to raw Debug.Log.");
            }

            return true;
        }

        private static string FormatMessage(DebugCategory category, string message, LogLevel level)
        {
            string colour = GetColourForCategory(category);
            string categoryLabel = level switch
            {
                LogLevel.Warning => $"{category} Warning",
                LogLevel.Error   => $"{category} Error",
                _                => category.ToString()
            };

            return $"<color={colour}>[{categoryLabel}]</color> {message}";
        }

        private static string GetColourForCategory(DebugCategory category)
        {
            return category switch
            {
                DebugCategory.Bootstrap       => "#4FC3F7",
                DebugCategory.SaveLoad        => "#4FC3F7",
                DebugCategory.UI              => "#00BCD4",
                DebugCategory.Navigation      => "#00BCD4",
                DebugCategory.Validation      => "#FFD54F",
                DebugCategory.Performance     => "#FF9800",
                DebugCategory.Testing         => "#CE93D8",
                DebugCategory.Visual          => "#81C784",
                DebugCategory.Audio           => "#81C784",
                DebugCategory.Input           => "#B0BEC5",
                DebugCategory.DataDefinitions => "#B0BEC5",
                DebugCategory.Simulation      => "#FFA726",
                _                             => "#FFFFFF"   // General, Configuration
            };
        }
    }
}
