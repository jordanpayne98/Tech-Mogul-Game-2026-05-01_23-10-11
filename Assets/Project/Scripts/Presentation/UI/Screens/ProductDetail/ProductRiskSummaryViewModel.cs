namespace Project.Presentation.UI.Screens.ProductDetail
{
    /// <summary>
    /// Pure display-data class for the right-hand risk and quality summary panel on the Product Detail screen.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — static data only in Phase 5.
    /// </summary>
    public sealed class ProductRiskSummaryViewModel
    {
        /// <summary>Pre-formatted overall score string, e.g. "72 / 100".</summary>
        public string OverallScore { get; }

        /// <summary>Pre-formatted quality value string, e.g. "High".</summary>
        public string Quality { get; }

        /// <summary>Pre-formatted creativity value string, e.g. "Medium".</summary>
        public string Creativity { get; }

        /// <summary>Pre-formatted stability value string, e.g. "High".</summary>
        public string Stability { get; }

        /// <summary>Pre-formatted bug risk value string, e.g. "Low".</summary>
        public string BugRisk { get; }

        /// <summary>Pre-formatted QA confidence value string, e.g. "85%".</summary>
        public string QaConfidence { get; }

        /// <summary>Pre-formatted infrastructure readiness value string, e.g. "Ready".</summary>
        public string InfrastructureReadiness { get; }

        /// <summary>Pre-formatted support readiness value string, e.g. "Partial".</summary>
        public string SupportReadiness { get; }

        /// <summary>Pre-formatted development budget string, e.g. "$240,000 / mo".</summary>
        public string DevelopmentBudget { get; }

        /// <summary>Pre-formatted pre-launch marketing monthly budget string, e.g. "$50,000 / mo".</summary>
        public string PreLaunchMarketingBudget { get; }

        /// <summary>Pre-formatted post-launch marketing monthly budget string, e.g. "$30,000 / mo".</summary>
        public string PostLaunchMarketingBudget { get; }

        /// <summary>Pre-formatted post-launch support monthly budget string, e.g. "$15,000 / mo".</summary>
        public string PostLaunchSupportBudget { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        public ProductRiskSummaryViewModel(
            string overallScore,
            string quality,
            string creativity,
            string stability,
            string bugRisk,
            string qaConfidence,
            string infrastructureReadiness,
            string supportReadiness,
            string developmentBudget,
            string preLaunchMarketingBudget,
            string postLaunchMarketingBudget,
            string postLaunchSupportBudget,
            string semanticState)
        {
            OverallScore               = overallScore;
            Quality                    = quality;
            Creativity                 = creativity;
            Stability                  = stability;
            BugRisk                    = bugRisk;
            QaConfidence               = qaConfidence;
            InfrastructureReadiness    = infrastructureReadiness;
            SupportReadiness           = supportReadiness;
            DevelopmentBudget          = developmentBudget;
            PreLaunchMarketingBudget   = preLaunchMarketingBudget;
            PostLaunchMarketingBudget  = postLaunchMarketingBudget;
            PostLaunchSupportBudget    = postLaunchSupportBudget;
            SemanticState              = semanticState;
        }
    }
}
