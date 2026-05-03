using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens
{
    /// <summary>
    /// Builds the PlaceholderScreen VisualElement tree programmatically.
    /// Displayed when the ScreenRouter is asked to open an unregistered screen ID,
    /// or any screen not yet implemented in Phase 5+.
    ///
    /// [Placeholder] — all content is a temporary stand-in for Phase 5 screens.
    /// </summary>
    public static class PlaceholderScreenView
    {
        /// <summary>
        /// Returns a VisualElement tree matching the PlaceholderScreen visual spec.
        /// </summary>
        /// <param name="screenId">The stable route ID, e.g. "screen.portal".</param>
        /// <param name="displayName">Human-readable name shown as the title.
        /// Falls back to <paramref name="screenId"/> when null or empty.</param>
        public static VisualElement Create(string screenId, string displayName = null)
        {
            string title = !string.IsNullOrEmpty(displayName) ? displayName : screenId;

            var root = new VisualElement();
            root.AddToClassList("placeholder-screen");

            var icon = new VisualElement();
            icon.AddToClassList("placeholder-screen__icon");
            root.Add(icon);

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("placeholder-screen__title");
            root.Add(titleLabel);

            // [Placeholder] message as specified in Phase 4 Shell and Navigation spec.
            var messageLabel = new Label("[Placeholder] This screen will be implemented in Phase 5.");
            messageLabel.AddToClassList("placeholder-screen__message");
            root.Add(messageLabel);

            var routeLabel = new Label($"Route: {screenId}");
            routeLabel.AddToClassList("placeholder-screen__route-id");
            root.Add(routeLabel);

            return root;
        }
    }
}
