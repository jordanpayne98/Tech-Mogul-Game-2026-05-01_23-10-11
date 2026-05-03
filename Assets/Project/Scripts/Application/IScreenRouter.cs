using System;

namespace Project.Application
{
    /// <summary>
    /// Screen navigation interface. Implemented in the Presentation layer.
    /// Application and Core must not depend on the concrete implementation.
    /// Uses flat navigation only — calling OpenScreen replaces the current screen.
    /// </summary>
    public interface IScreenRouter
    {
        /// <summary>Navigate to a screen by stable ID.</summary>
        void OpenScreen(string screenId);

        /// <summary>The stable ID of the currently active screen, or null if none.</summary>
        string CurrentScreenId { get; }

        /// <summary>Fired after every screen transition. Argument is the new screen ID.</summary>
        event Action<string> ScreenChanged;
    }
}
