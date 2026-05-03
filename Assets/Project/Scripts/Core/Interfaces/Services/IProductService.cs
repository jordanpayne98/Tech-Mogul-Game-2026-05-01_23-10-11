using Project.Core.Definitions.Product;
using Project.Core.Definitions.Team;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Product;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Shared product lifecycle service interface.
    /// Stateless — receives all required state as parameters, holds no references.
    /// Does not publish events. Shared between software and hardware product services.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Creates a new ProductProfile from the given request.
        /// Generates a GUID stable ID, derives the product family from the category,
        /// and sets the created date from currentDate.
        /// </summary>
        ProductProfile CreateProductProfile(CreateProductRequest request, GameDateTime currentDate);

        /// <summary>
        /// Creates a zeroed ProductRuntimeState for the given product ID.
        /// Status is set to InConcept. All numeric fields are initialized to 0.
        /// ScoreValues dictionary is populated with all ProductScoreDimension keys set to 0.
        /// </summary>
        ProductRuntimeState CreateProductRuntimeState(string productId);

        /// <summary>
        /// Creates a ProductBudgetProfile from the request's budget fields.
        /// </summary>
        ProductBudgetProfile CreateProductBudgetProfile(string productId, CreateProductRequest request);

        /// <summary>
        /// Computes the total raw progress points required to complete a given product phase.
        /// Applies formula.product.phase_duration.
        /// Returns 0 for non-work phases (ReadyForLaunch, Launched, etc.).
        /// The family parameter selects hardware vs software base hours.
        /// </summary>
        int ComputePhaseRequiredPoints(ProductStatus phase, int featureScope, int qualityTarget, IProductTuning tuning, ProductFamily family);

        /// <summary>
        /// Returns the next ProductStatus after the given phase, or null if no auto-transition applies.
        /// Software: InConcept → InDevelopment → InQA → ReadyForLaunch (null after ReadyForLaunch).
        /// </summary>
        ProductStatus? GetNextPhase(ProductStatus currentPhase, ProductFamily family);

        /// <summary>
        /// Maps a product lifecycle phase to the AssignmentType used for role/skill matching.
        /// </summary>
        AssignmentType GetAssignmentTypeForPhase(ProductStatus phase, ProductFamily family);

        /// <summary>
        /// Returns true for phases where team work produces progress points.
        /// True for InConcept, InDevelopment, InQA. False for all other statuses.
        /// </summary>
        bool IsWorkPhase(ProductStatus status);

        /// <summary>
        /// Validates that a product is eligible for launch.
        /// Fails if Status is not ReadyForLaunch.
        /// </summary>
        bool ValidateLaunch(ProductRuntimeState productState, out string failureReason);

        /// <summary>
        /// Validates that a product is eligible for sunset.
        /// Fails if Status is not Launched or Supported.
        /// </summary>
        bool ValidateSunset(ProductRuntimeState productState, out string failureReason);

        /// <summary>
        /// Derives the ProductFamily (Software or Hardware) from the given ProductCategory.
        /// HardwarePlatform, Peripheral, LaptopDesktopDevice, ServerDevice → Hardware. All others → Software.
        /// </summary>
        ProductFamily DeriveFamily(ProductCategory category);
    }
}
