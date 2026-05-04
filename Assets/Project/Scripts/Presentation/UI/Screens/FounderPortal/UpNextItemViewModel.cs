using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// Pure display-data class for the "Up Next" upcoming milestones card in the Founder Portal.
    /// Immutable after construction. No Unity dependencies.
    /// </summary>
    public sealed class UpNextItemViewModel
    {
        /// <summary>Card title, e.g. "Upcoming Milestones".</summary>
        public string Title { get; }

        /// <summary>Whether there are any milestone items to display.</summary>
        public bool HasItems { get; }

        /// <summary>Message shown when there are no items, e.g. "No upcoming milestones".</summary>
        public string EmptyMessage { get; }

        /// <summary>Pre-formatted milestone description strings.</summary>
        public IReadOnlyList<string> Items { get; }

        public UpNextItemViewModel(
            string title,
            bool hasItems,
            string emptyMessage,
            IReadOnlyList<string> items)
        {
            Title        = title;
            HasItems     = hasItems;
            EmptyMessage = emptyMessage;
            Items        = items;
        }
    }
}
