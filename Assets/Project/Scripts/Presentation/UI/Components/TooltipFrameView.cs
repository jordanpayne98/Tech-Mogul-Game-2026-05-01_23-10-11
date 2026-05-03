using UnityEngine.UIElements;

namespace Project.Presentation.UI.Components
{
    /// <summary>
    /// Factory for the TooltipFrame VisualElement.
    /// Creates a styled tooltip element ready to be injected into the TooltipHost.
    ///
    /// Phase 4: Visual style validation only.
    /// Positioning logic (follow mouse / anchor to element) is deferred to Phase 5.
    /// </summary>
    public static class TooltipFrameView
    {
        // ─── Public Factory ───────────────────────────────────────────────────────

        /// <summary>
        /// Creates a tooltip VisualElement with the given content text.
        /// </summary>
        /// <param name="content">Text to display inside the tooltip.</param>
        public static VisualElement Create(string content)
        {
            // ── Root ───────────────────────────────────────────────────────────────
            var root = new VisualElement();
            root.AddToClassList("tooltip-frame");

            // ── Content Label ──────────────────────────────────────────────────────
            var contentLabel = new Label(content ?? string.Empty);
            contentLabel.AddToClassList("tooltip-frame__content");
            root.Add(contentLabel);

            return root;
        }
    }
}
