namespace Project.Core.Runtime.Report
{
    /// <summary>
    /// A structured label/value pair embedded in a ReportProfile.
    /// Used to surface key metrics or data points within a report (e.g. revenue, headcount).
    /// PreviousValue and ChangeText are nullable — omit for data points with no comparison context.
    /// </summary>
    public struct ReportKeyValue
    {
        /// <summary>Human-readable label for the data point (e.g. "Monthly Revenue").</summary>
        public string Label;

        /// <summary>Current formatted value (e.g. "$1,200,000").</summary>
        public string Value;

        /// <summary>Previous formatted value for comparison. Null if no comparison is available.</summary>
        public string PreviousValue;

        /// <summary>Human-readable change description (e.g. "+12% vs last month"). Null if not applicable.</summary>
        public string ChangeText;
    }
}
