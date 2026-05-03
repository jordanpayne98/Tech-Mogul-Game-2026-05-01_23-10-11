using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Team;
using Project.Core.Events.Product;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Product
{
    /// <summary>
    /// Daily phase transition processor for all products (Software and Hardware).
    /// Runs at Order 3 each simulation day. Checks whether any product's assigned teams
    /// have accumulated enough RawProgressPoints to complete the current phase,
    /// then triggers the auto-transition to the next phase.
    ///
    /// ReadyForLaunch is a gate — it never auto-transitions. Launch requires
    /// the player to explicitly call LaunchProductUseCase.
    ///
    /// Phase transition resets RawProgressPoints on all targeting assignments to 0
    /// and updates their AssignmentType so TeamAssignmentRequirementRules applies
    /// the correct role/skill matching for the new phase.
    /// </summary>
    public sealed class ProductPhaseTickProcessor : ITickProcessor
    {
        private readonly IProductService  _productService;
        private readonly IProductTuning   _tuning;
        private readonly GameSessionState _sessionState;
        private readonly IEventBus        _eventBus;

        public string ProcessorName   => "ProductPhaseTickProcessor";
        public int    ProcessingOrder => 300;

        public ProductPhaseTickProcessor(
            IProductService  productService,
            IProductTuning   tuning,
            GameSessionState sessionState,
            IEventBus        eventBus)
        {
            _productService = productService;
            _tuning         = tuning;
            _sessionState   = sessionState;
            _eventBus       = eventBus;
        }

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            if (!context.IsDayBoundary)
            {
                return TickResult.Succeeded();
            }

            foreach (var productState in _sessionState.ProductStates)
            {
                // Only process products that are in a work phase.
                if (!_productService.IsWorkPhase(productState.Status))
                {
                    continue;
                }

                // Find the matching profile to get featureScope and qualityTarget.
                var profile = _sessionState.ProductProfiles
                    .FirstOrDefault(p => p.Id == productState.ProductId);

                if (profile == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[ProductPhaseTickProcessor] No profile found for ProductId: {productState.ProductId}. Skipping.");
                    continue;
                }

                // Sum raw progress from all assignments targeting this product.
                int totalRawProgress = 0;
                foreach (var assignment in _sessionState.AssignmentStates)
                {
                    if (assignment.TargetType == AssignmentTargetType.Product
                        && assignment.TargetId == productState.ProductId)
                    {
                        totalRawProgress += assignment.RawProgressPoints;
                    }
                }

                // Compute required points for this phase.
                int requiredPoints = _productService.ComputePhaseRequiredPoints(
                    productState.Status,
                    profile.FeatureScope,
                    profile.QualityTarget,
                    _tuning,
                    profile.Family);

                if (requiredPoints <= 0)
                {
                    continue;
                }

                // Update derived progress percent on the product runtime state.
                productState.ProgressPercent = System.Math.Min(100,
                    totalRawProgress * 100 / requiredPoints);

                // Check for phase completion.
                if (totalRawProgress < requiredPoints)
                {
                    continue;
                }

                // Determine the next phase.
                var nextPhase = _productService.GetNextPhase(productState.Status, profile.Family);
                if (nextPhase == null)
                {
                    continue;
                }

                var completedPhase = productState.Status;

                // Transition the product to the next phase.
                productState.Status          = nextPhase.Value;
                productState.ProgressPercent = 0;

                // Determine the new assignment type for the next phase.
                AssignmentType newAssignmentType = _productService.GetAssignmentTypeForPhase(
                    nextPhase.Value,
                    profile.Family);

                // Reset progress on all assignments targeting this product and update their type.
                foreach (var assignment in _sessionState.AssignmentStates)
                {
                    if (assignment.TargetType == AssignmentTargetType.Product
                        && assignment.TargetId == productState.ProductId)
                    {
                        assignment.RawProgressPoints = 0;
                        assignment.ProgressPercent   = 0;
                        assignment.Type              = newAssignmentType;
                    }
                }

                DebugLogger.Log(DebugCategory.Simulation,
                    $"[ProductPhaseTickProcessor] Phase completed. ProductId: {productState.ProductId}, "
                    + $"{completedPhase} → {nextPhase.Value}, NewAssignmentType: {newAssignmentType}");

                _eventBus.Publish(new ProductPhaseCompletedEvent(
                    productState.ProductId,
                    completedPhase,
                    nextPhase.Value));
            }

            return TickResult.Succeeded();
        }
    }
}
