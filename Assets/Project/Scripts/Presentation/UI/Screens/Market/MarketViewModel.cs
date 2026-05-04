using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Market screen (screen.market).
    /// Immutable after construction. No Unity dependencies.
    /// Created by MarketController and passed to MarketView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MarketViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Market".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there is no content.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there is no content.</summary>
        public string EmptyStateBody { get; }

        // ── Filter / search ──────────────────────────────────────────────────

        /// <summary>Current search query text entered by the player.</summary>
        public string SearchText { get; }

        // ── Market content ───────────────────────────────────────────────────

        /// <summary>Trend strip items shown at the top of the screen.</summary>
        public IReadOnlyList<MarketTrendViewModel> Trends { get; }

        /// <summary>Market category cards or rows shown in the category grid/table.</summary>
        public IReadOnlyList<MarketCategoryRowViewModel> Categories { get; }

        /// <summary>Product ranking rows for the currently selected category or global market.</summary>
        public IReadOnlyList<MarketRankingRowViewModel> Rankings { get; }

        /// <summary>Stable ID of the currently selected category. Empty string if none selected.</summary>
        public string SelectedCategoryId { get; }

        // ── Screen states ────────────────────────────────────────────────────

        /// <summary>True when no market data is available to display.</summary>
        public bool HasNoMarketData { get; }

        /// <summary>True when the player company has a tracked position in the currently visible market.</summary>
        public bool HasPlayerPosition { get; }

        public MarketViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            string searchText,
            IReadOnlyList<MarketTrendViewModel> trends,
            IReadOnlyList<MarketCategoryRowViewModel> categories,
            IReadOnlyList<MarketRankingRowViewModel> rankings,
            string selectedCategoryId,
            bool hasNoMarketData,
            bool hasPlayerPosition)
        {
            ScreenTitle        = screenTitle;
            ScreenSubtitle     = screenSubtitle;
            IsLoading          = isLoading;
            HasError           = hasError;
            ErrorMessage       = errorMessage;
            EmptyStateTitle    = emptyStateTitle;
            EmptyStateBody     = emptyStateBody;
            SearchText         = searchText;
            Trends             = trends;
            Categories         = categories;
            Rankings           = rankings;
            SelectedCategoryId = selectedCategoryId;
            HasNoMarketData    = hasNoMarketData;
            HasPlayerPosition  = hasPlayerPosition;
        }
    }
}
