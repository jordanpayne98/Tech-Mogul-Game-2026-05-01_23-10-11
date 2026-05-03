namespace Project.Core.Runtime.Product
{
    /// <summary>
    /// Mutable domain-specific metrics for Software products.
    /// Tracks operational health indicators such as user growth, churn, infrastructure,
    /// uptime, bug density, security risk, and support load.
    /// ChurnBasisPoints and UptimeBasisPoints use a 0–10000 range (100 = 1%).
    /// </summary>
    public sealed class SoftwareRuntimeMetrics
    {
        /// <summary>Stable ID of the ProductProfile these metrics belong to.</summary>
        public string ProductId;

        /// <summary>Number of new users acquired in the current simulation month.</summary>
        public int NewUsersThisMonth;

        /// <summary>
        /// Monthly user churn expressed in basis points (0–10000).
        /// 100 basis points = 1% churn rate.
        /// </summary>
        public int ChurnBasisPoints;

        /// <summary>Current infrastructure load level. Higher values indicate strain.</summary>
        public int InfrastructureLoad;

        /// <summary>
        /// Service uptime expressed in basis points (0–10000).
        /// 10000 = 100% uptime.
        /// </summary>
        public int UptimeBasisPoints;

        /// <summary>Number of known bugs currently outstanding in the product.</summary>
        public int BugCount;

        /// <summary>Security risk level. Higher values indicate greater exposure.</summary>
        public int SecurityRisk;

        /// <summary>Number of open support tickets associated with this product.</summary>
        public int SupportTickets;

        /// <summary>Feature satisfaction score reflecting how well features meet user expectations (0–100).</summary>
        public int FeatureSatisfaction;
    }
}
