namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Display data for one research track tab (e.g. track.software_engineering).
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    ///
    /// Stable track IDs:
    ///   track.software_engineering, track.hardware_engineering, track.cloud_infrastructure,
    ///   track.security, track.ai_automation, track.ux_product_design, track.developer_tools,
    ///   track.manufacturing, track.marketing_analytics, track.support_operations,
    ///   track.platform_ecosystems
    /// </summary>
    public sealed class ResearchTrackViewModel
    {
        /// <summary>Stable ID for this track, e.g. "track.software_engineering".</summary>
        public string Id { get; }

        /// <summary>Display name for this track tab, e.g. "Software Engineering".</summary>
        public string Name { get; }

        /// <summary>Formatted count of projects in this track, e.g. "4 projects".</summary>
        public string ProjectCount { get; }

        /// <summary>
        /// Semantic visual state for this tab.
        /// Drives USS class toggling — never raw colour values.
        /// Examples: "active", "default", "has-research-in-progress".
        /// </summary>
        public string SemanticState { get; }

        public ResearchTrackViewModel(
            string id,
            string name,
            string projectCount,
            string semanticState)
        {
            Id             = id;
            Name           = name;
            ProjectCount   = projectCount;
            SemanticState  = semanticState;
        }
    }
}
