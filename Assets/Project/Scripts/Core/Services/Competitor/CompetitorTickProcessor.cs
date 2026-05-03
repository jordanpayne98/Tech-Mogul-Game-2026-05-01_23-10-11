using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Market;
using Project.Core.Definitions.Product;
using Project.Core.Events.Competitor;
using Project.Core.Events.Market;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Time;
using Project.Core.Services.Market;

namespace Project.Core.Services.Competitor
{
    /// <summary>
    /// Tick processor for AI competitor simulation.
    /// ProcessingOrder 900 — runs after market tick (800) and before events/crisis (1000).
    /// On week boundaries: runs action selection (launch, price change, market expansion).
    /// On month boundaries: recalculates market share across all categories.
    /// When both boundaries align, weekly actions run before monthly share resolution.
    /// Defined in Plan 2J, GDD_10.
    /// </summary>
    public sealed class CompetitorTickProcessor : ITickProcessor
    {
        // ── ITickProcessor ─────────────────────────────────────────────────────────

        public string ProcessorName  => "CompetitorTickProcessor";
        public int    ProcessingOrder => 900;

        // ── Dependencies ──────────────────────────────────────────────────────────

        private readonly ICompetitorService _competitorService;
        private readonly ICompetitorTuning  _competitorTuning;
        private readonly IProductTuning     _productTuning;
        private readonly IEventBus          _eventBus;
        private readonly GameSessionState   _sessionState;
        private readonly Random             _random;

        private static readonly MarketCategoryType[] AllCategories =
            (MarketCategoryType[])Enum.GetValues(typeof(MarketCategoryType));

        // ── Constructor ───────────────────────────────────────────────────────────

        public CompetitorTickProcessor(
            ICompetitorService  competitorService,
            ICompetitorTuning   competitorTuning,
            IProductTuning      productTuning,
            IEventBus           eventBus,
            GameSessionState    sessionState,
            Random              random)
        {
            _competitorService = competitorService ?? throw new ArgumentNullException(nameof(competitorService));
            _competitorTuning  = competitorTuning  ?? throw new ArgumentNullException(nameof(competitorTuning));
            _productTuning     = productTuning     ?? throw new ArgumentNullException(nameof(productTuning));
            _eventBus          = eventBus          ?? throw new ArgumentNullException(nameof(eventBus));
            _sessionState      = sessionState      ?? throw new ArgumentNullException(nameof(sessionState));
            _random            = random            ?? throw new ArgumentNullException(nameof(random));
        }

        // ── ITickProcessor ─────────────────────────────────────────────────────────

        public TickResult ProcessTick(TickContext context)
        {
            // ── Weekly boundary — action selection ────────────────────────────────
            if (context.IsWeekBoundary)
            {
                ProcessWeeklyActions(context.CurrentDate);
            }

            // ── Monthly boundary — market share resolution ────────────────────────
            if (context.IsMonthBoundary)
            {
                ProcessMonthlyMarketShare();
            }

            return TickResult.Succeeded();
        }

        // ── Weekly Actions ────────────────────────────────────────────────────────

        private void ProcessWeeklyActions(GameDateTime currentDate)
        {
            for (int i = 0; i < _sessionState.CompetitorProfiles.Count; i++)
            {
                var profile = _sessionState.CompetitorProfiles[i];
                var state   = FindState(profile.Id);

                if (state == null)
                {
                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[CompetitorTickProcessor] No runtime state found for competitor {profile.Id} — skipping.");
                    continue;
                }

                var action = _competitorService.SelectAction(state, profile, _competitorTuning, currentDate, _random);

                if (action == null)
                {
                    continue;
                }

                switch (action.Value)
                {
                    case CompetitorActionType.LaunchProduct:
                        ExecuteLaunch(profile, state, currentDate);
                        break;

                    case CompetitorActionType.ChangePrice:
                        ExecutePriceChange(state);
                        break;

                    case CompetitorActionType.EnterMarket:
                        ExecuteExpansion(profile);
                        break;
                }
            }
        }

        private void ExecuteLaunch(CompetitorProfile profile, CompetitorRuntimeState state, GameDateTime currentDate)
        {
            if (!_competitorService.CanLaunch(state, currentDate, _competitorTuning, _random))
            {
                return;
            }

            var newProduct = _competitorService.LaunchProduct(profile, state, _competitorTuning, currentDate, _random);

            _sessionState.CompetitorProductStates.Add(newProduct);
            state.ProductIds.Add(newProduct.Id);
            state.LastLaunchDate = currentDate;

            _eventBus.Publish(new CompetitorProductLaunchedEvent(profile.Id, newProduct.Id));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[CompetitorTickProcessor] Competitor {profile.Name} ({profile.Id}) launched '{newProduct.Name}' " +
                $"in {newProduct.Category}. Quality: {newProduct.QualityScore}, Marketing: {newProduct.MarketingStrength}, " +
                $"Price: {newProduct.PriceMinorUnits / 100m:F2}.");
        }

        private void ExecutePriceChange(CompetitorRuntimeState state)
        {
            var result = _competitorService.ChangePrice(
                state,
                _sessionState.CompetitorProductStates,
                _competitorTuning,
                _random);

            if (result == null)
            {
                return;
            }

            var (productId, newPrice) = result.Value;

            // Apply price to the product.
            var product = FindCompetitorProduct(productId);
            if (product != null)
            {
                product.PriceMinorUnits = newPrice;
            }

            _eventBus.Publish(new CompetitorPriceChangedEvent(state.CompetitorId, productId, newPrice));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[CompetitorTickProcessor] Competitor {state.CompetitorId} changed price of product " +
                $"{productId} to {newPrice / 100m:F2}.");
        }

        private void ExecuteExpansion(CompetitorProfile profile)
        {
            var allCategories = new List<MarketCategoryType>(AllCategories);
            var newCategory   = _competitorService.ExpandMarket(profile, allCategories, _random);

            if (newCategory == null)
            {
                return;
            }

            profile.MarketFocus.Add(newCategory.Value);

            _eventBus.Publish(new CompetitorExpansionEvent(profile.Id, newCategory.Value));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[CompetitorTickProcessor] Competitor {profile.Name} ({profile.Id}) expanded into {newCategory.Value}.");
        }

        // ── Monthly Market Share Resolution ───────────────────────────────────────

        private void ProcessMonthlyMarketShare()
        {
            foreach (var category in AllCategories)
            {
                ProcessCategoryShare(category);
            }

            DebugLogger.Log(DebugCategory.Simulation,
                "[CompetitorTickProcessor] Monthly market share resolution complete for all categories.");
        }

        private void ProcessCategoryShare(MarketCategoryType category)
        {
            var marketCategory = FindMarketCategory(category);

            // Collect active player products in this category.
            var playerEntries = CollectPlayerProducts(category);

            // Collect active competitor products in this category.
            var competitorProducts = new List<CompetitorProductRuntimeState>();
            foreach (var cp in _sessionState.CompetitorProductStates)
            {
                if (cp.IsActive && cp.Category == category)
                {
                    competitorProducts.Add(cp);
                }
            }

            int totalActive = playerEntries.Count + competitorProducts.Count;

            if (totalActive == 0)
            {
                if (marketCategory != null)
                {
                    marketCategory.LeaderProductIds  = new List<string>();
                    marketCategory.CompetitiveIntensity = 0;
                }
                return;
            }

            // Gather all prices for percentile calculation.
            var allPrices = new List<long>();
            foreach (var (_, profile, _) in playerEntries)
            {
                allPrices.Add(profile.PriceMinorUnits);
            }
            foreach (var cp in competitorProducts)
            {
                allPrices.Add(cp.PriceMinorUnits);
            }

            // Build scoring list (productId, score).
            var scoringList = new List<(string productId, float score)>();

            // Player products.
            foreach (var (state, profile, budget) in playerEntries)
            {
                int qualityValue   = state.ReviewScore;
                int pricePercentile = _competitorService.ComputePricePercentile(profile.PriceMinorUnits, allPrices);
                int priceScore     = 100 - pricePercentile;

                long marketingBudget = budget != null ? budget.PostLaunchMarketingMonthlyBudgetMinorUnits : 0L;
                long reference       = _productTuning.MarketingBudgetReferenceMinorUnits;
                int  marketingValue  = reference > 0
                    ? Math.Max(0, Math.Min(100, (int)(marketingBudget * 100L / reference)))
                    : 0;

                float score = _competitorService.ComputeProductScore(qualityValue, priceScore, marketingValue, _competitorTuning);
                scoringList.Add((state.ProductId, score));
            }

            // Competitor products.
            foreach (var cp in competitorProducts)
            {
                int pricePercentile = _competitorService.ComputePricePercentile(cp.PriceMinorUnits, allPrices);
                int priceScore      = 100 - pricePercentile;
                float score         = _competitorService.ComputeProductScore(cp.QualityScore, priceScore, cp.MarketingStrength, _competitorTuning);
                scoringList.Add((cp.Id, score));
            }

            // Distribute shares.
            var shareDistribution = _competitorService.DistributeShares(scoringList);
            var shareMap          = new Dictionary<string, int>();
            foreach (var (productId, share) in shareDistribution)
            {
                shareMap[productId] = share;
            }

            // Write player product shares.
            foreach (var (state, _, _) in playerEntries)
            {
                if (shareMap.TryGetValue(state.ProductId, out int share))
                {
                    state.MarketShareBasisPoints = share;
                }
            }

            // Write competitor product shares.
            foreach (var cp in competitorProducts)
            {
                if (shareMap.TryGetValue(cp.Id, out int share))
                {
                    cp.MarketShareBasisPoints = share;
                }
            }

            // Aggregate competitor total share per category on CompetitorRuntimeState.
            foreach (var compState in _sessionState.CompetitorStates)
            {
                int totalShare = 0;
                foreach (var cp in competitorProducts)
                {
                    if (cp.CompetitorId == compState.CompetitorId && shareMap.TryGetValue(cp.Id, out int share))
                    {
                        totalShare += share;
                    }
                }
                compState.MarketShareBasisPoints[category] = totalShare;
            }

            // Update market category metadata.
            if (marketCategory != null)
            {
                marketCategory.CompetitiveIntensity = Math.Max(0, Math.Min(100, totalActive * _competitorTuning.IntensityPerProduct));
                marketCategory.LeaderProductIds     = ComputeLeaderIds(shareMap, 3);
            }
        }

        // ── Helper Methods ────────────────────────────────────────────────────────

        private List<(ProductRuntimeState state, ProductProfile profile, ProductBudgetProfile budget)>
            CollectPlayerProducts(MarketCategoryType category)
        {
            var result = new List<(ProductRuntimeState, ProductProfile, ProductBudgetProfile)>();

            foreach (var productState in _sessionState.ProductStates)
            {
                if (productState.Status != ProductStatus.Launched
                 && productState.Status != ProductStatus.Supported
                 && productState.Status != ProductStatus.Updating
                 && productState.Status != ProductStatus.Mature
                 && productState.Status != ProductStatus.Declining)
                {
                    continue;
                }

                var profile = FindProductProfile(productState.ProductId);
                if (profile == null) continue;

                // Map ProductCategory to MarketCategoryType.
                MarketCategoryType productMarketCategory;
                try
                {
                    productMarketCategory = ProductCategoryMarketMap.GetMarketCategory(profile.Category);
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }

                if (productMarketCategory != category) continue;

                var budget = FindProductBudget(productState.ProductId);
                result.Add((productState, profile, budget));
            }

            return result;
        }

        private List<string> ComputeLeaderIds(Dictionary<string, int> shareMap, int topN)
        {
            var sorted = new List<KeyValuePair<string, int>>(shareMap);
            sorted.Sort((a, b) => b.Value.CompareTo(a.Value));

            var leaders = new List<string>();
            for (int i = 0; i < topN && i < sorted.Count; i++)
            {
                leaders.Add(sorted[i].Key);
            }
            return leaders;
        }

        private CompetitorRuntimeState FindState(string competitorId)
        {
            foreach (var s in _sessionState.CompetitorStates)
            {
                if (s.CompetitorId == competitorId) return s;
            }
            return null;
        }

        private CompetitorProductRuntimeState FindCompetitorProduct(string productId)
        {
            foreach (var p in _sessionState.CompetitorProductStates)
            {
                if (p.Id == productId) return p;
            }
            return null;
        }

        private MarketCategoryRuntimeState FindMarketCategory(MarketCategoryType category)
        {
            foreach (var mc in _sessionState.MarketCategoryStates)
            {
                if (mc.CategoryType == category) return mc;
            }
            return null;
        }

        private ProductProfile FindProductProfile(string productId)
        {
            foreach (var p in _sessionState.ProductProfiles)
            {
                if (p.Id == productId) return p;
            }
            return null;
        }

        private ProductBudgetProfile FindProductBudget(string productId)
        {
            foreach (var b in _sessionState.ProductBudgets)
            {
                if (b.ProductId == productId) return b;
            }
            return null;
        }
    }
}
