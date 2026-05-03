namespace Project.Core.Definitions.Product
{
    /// <summary>
    /// Unifies all product types under a single enum.
    /// Game, DevelopmentTool, and OperatingSystem belong to the Software family.
    /// HardwarePlatform belongs to the Hardware family.
    /// </summary>
    public enum ProductCategory
    {
        /// <summary>Software family. A consumer game title.</summary>
        Game,

        /// <summary>Software family. A tool sold to developers or studios.</summary>
        DevelopmentTool,

        /// <summary>Software family. An operating system product.</summary>
        OperatingSystem,

        /// <summary>Hardware family. A physical computing or gaming platform.</summary>
        HardwarePlatform,

        /// <summary>Hardware family. A peripheral device (keyboard, mouse, controller, etc.).</summary>
        Peripheral,

        /// <summary>Hardware family. A laptop or desktop computing device.</summary>
        LaptopDesktopDevice,

        /// <summary>Hardware family. Server or networking hardware.</summary>
        ServerDevice,

        /// <summary>Software family. A web-based platform or service.</summary>
        WebPlatform,

        /// <summary>Software family. A productivity application.</summary>
        ProductivityApp,

        /// <summary>Software family. A business SaaS product.</summary>
        BusinessSaaS
    }
}
