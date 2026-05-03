using System;

namespace Project.Application
{
    /// <summary>
    /// Right drawer navigation interface. Implemented in the Presentation layer.
    /// Application and Core must not depend on the concrete implementation.
    /// Only one drawer may be open at a time.
    /// </summary>
    public interface IRightDrawerRouter
    {
        /// <summary>Open a drawer by stable ID with optional context data.</summary>
        void OpenDrawer(string drawerId, object context = null);

        /// <summary>Close the currently open drawer, if any.</summary>
        void CloseDrawer();

        /// <summary>Whether a drawer is currently open.</summary>
        bool IsDrawerOpen { get; }

        /// <summary>The stable ID of the currently active drawer, or null if none.</summary>
        string CurrentDrawerId { get; }

        /// <summary>Fired when a drawer opens. Argument is the drawer ID.</summary>
        event Action<string> DrawerOpened;

        /// <summary>Fired when a drawer closes.</summary>
        event Action DrawerClosed;
    }
}
