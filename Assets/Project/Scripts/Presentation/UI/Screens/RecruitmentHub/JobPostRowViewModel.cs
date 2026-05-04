namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Stable IDs for job post status values used in the Recruitment Hub Job Posts tab.
    /// </summary>
    public static class JobPostStatusIds
    {
        public const string Active  = "status.active";
        public const string Paused  = "status.paused";
        public const string Closed  = "status.closed";
        public const string Draft   = "status.draft";
        public const string Expired = "status.expired";
    }

    /// <summary>
    /// Pure display-data class for a single job post row in the Recruitment Hub Job Posts tab.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "active", "paused", "closed", "draft", "expired".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class JobPostRowViewModel
    {
        /// <summary>Stable job post ID, e.g. "jobpost.0001".</summary>
        public string Id { get; }

        /// <summary>Job post display title, e.g. "Senior Software Engineer — Core Engine".</summary>
        public string Title { get; }

        /// <summary>Target role label, e.g. "Software Engineer".</summary>
        public string Role { get; }

        /// <summary>Seniority level label, e.g. "Senior".</summary>
        public string Seniority { get; }

        /// <summary>Current post status label, e.g. "Active", "Paused", "Closed".</summary>
        public string Status { get; }

        /// <summary>Pre-formatted applicant count display, e.g. "12 applicants".</summary>
        public string ApplicantCount { get; }

        /// <summary>Pre-formatted posted date display, e.g. "Posted 3 days ago".</summary>
        public string PostedDate { get; }

        /// <summary>Semantic state string: "normal", "active", "paused", "closed", "draft", "expired".</summary>
        public string SemanticState { get; }

        /// <summary>True when this row responds to click/tap for detail expansion.</summary>
        public bool IsClickable { get; }

        public JobPostRowViewModel(
            string id,
            string title,
            string role,
            string seniority,
            string status,
            string applicantCount,
            string postedDate,
            string semanticState,
            bool isClickable)
        {
            Id             = id;
            Title          = title;
            Role           = role;
            Seniority      = seniority;
            Status         = status;
            ApplicantCount = applicantCount;
            PostedDate     = postedDate;
            SemanticState  = semanticState;
            IsClickable    = isClickable;
        }
    }
}
