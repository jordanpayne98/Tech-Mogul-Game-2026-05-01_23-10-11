namespace Project.Core.SaveData.Product
{
    /// <summary>
    /// Save data mirroring <c>SoftwareRuntimeMetrics</c>.
    /// </summary>
    public sealed class SoftwareMetricsSaveData
    {
        public string ProductId;
        public int NewUsersThisMonth;
        public int ChurnBasisPoints;
        public int InfrastructureLoad;
        public int UptimeBasisPoints;
        public int BugCount;
        public int SecurityRisk;
        public int SupportTickets;
        public int FeatureSatisfaction;
    }
}
