namespace Project.Core.Definitions.Contract
{
    /// <summary>
    /// The category of work a contract requires.
    /// Covers all 16 GDD-defined contract types across software, hardware, and service domains.
    /// Defined in GDD_12, Appendix A.6.
    /// </summary>
    public enum ContractType
    {
        WebsiteBuild,
        MobileAppPrototype,
        InternalBusinessTool,
        CloudMigration,
        SecurityAudit,
        DataDashboard,
        QATestingSupport,
        HardwarePrototype,
        DeviceTesting,
        ManufacturingConsultation,
        PeripheralDesign,
        ServerSetup,
        MarketingCampaign,
        EnterpriseIntegration,
        SupportOutsourcing,
        ResearchStudy
    }
}
