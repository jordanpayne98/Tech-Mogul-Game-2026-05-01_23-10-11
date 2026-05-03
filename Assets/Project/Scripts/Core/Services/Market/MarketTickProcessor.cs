using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Market;
using Project.Core.Events.Market;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Market
{
    /// <summary>
    /// Tick processor for market simulation at ProcessingOrder 800.
    ///
    /// Per week boundary:
    ///   - Adjusts total demand for all market categories using formula.market.demand_shift.
    ///   - Publishes MarketDemandShiftedEvent when demand changes.
    ///
    /// Per month boundary (runs after weekly demand on the same tick):
    ///   - Decays active trends (formula.market.trend_decay).
    ///   - Expires trends past their EstimatedEndDate (IsActive = false).
    ///   - Optionally spawns a new trend based on TrendSpawnChancePerMonthPercent.
    ///   - Applies customer preference drift (formula.market.preference_drift).
    ///   - Applies expectation growth (formula.market.expectation_growth).
    ///
    /// Does not generate interruptions for MVP.
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public sealed class MarketTickProcessor : ITickProcessor
    {
        private readonly IMarketService  _marketService;
        private readonly IMarketTuning   _tuning;
        private readonly IEventBus       _eventBus;
        private readonly GameSessionState _sessionState;
        private readonly Random          _random;

        public string ProcessorName   => "MarketTickProcessor";
        public int    ProcessingOrder => 800;

        public MarketTickProcessor(
            IMarketService    marketService,
            IMarketTuning     tuning,
            IEventBus         eventBus,
            GameSessionState  sessionState,
            Random            random)
        {
            _marketService = marketService;
            _tuning        = tuning;
            _eventBus      = eventBus;
            _sessionState  = sessionState;
            _random        = random;
        }

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            // Weekly boundary — demand adjustment
            if (context.IsWeekBoundary)
            {
                ProcessWeeklyDemandAdjustment(context);
            }

            // Monthly boundary — structural updates (always after weekly demand on the same tick)
            if (context.IsMonthBoundary)
            {
                ProcessMonthlyTrendLifecycle(context);
                ProcessMonthlyTrendSpawning(context);
                ProcessMonthlyPreferenceDrift();
                ProcessMonthlyExpectationGrowth();
            }

            return TickResult.Succeeded();
        }

        // ── Weekly demand ─────────────────────────────────────────────────────────

        private void ProcessWeeklyDemandAdjustment(TickContext context)
        {
            foreach (var category in _sessionState.MarketCategoryStates)
            {
                var activeTrends  = _marketService.GetActiveTrendsForCategory(_sessionState.TrendStates, category.CategoryType);
                int previousDemand = category.TotalDemand;

                int newDemand = _marketService.ComputeWeeklyDemandShift(category, activeTrends, _tuning, _random);
                category.TotalDemand = newDemand;

                if (newDemand != previousDemand)
                {
                    _eventBus.Publish(new MarketDemandShiftedEvent(category.CategoryType, newDemand, previousDemand));
                }

                DebugLogger.Log(DebugCategory.Simulation,
                    $"[MarketTickProcessor] Demand adjusted. Category: {category.CategoryType}, " +
                    $"Previous: {previousDemand}, New: {newDemand}, Date: {context.CurrentDate}");
            }
        }

        // ── Monthly trend lifecycle ───────────────────────────────────────────────

        private void ProcessMonthlyTrendLifecycle(TickContext context)
        {
            // Snapshot active trends to avoid modification during iteration
            var activeTrends = new List<TrendRuntimeState>();
            foreach (var trend in _sessionState.TrendStates)
            {
                if (trend.IsActive)
                {
                    activeTrends.Add(trend);
                }
            }

            foreach (var trend in activeTrends)
            {
                if (_marketService.IsTrendExpired(trend, context.CurrentDate))
                {
                    // Expire the trend
                    trend.IsActive = false;
                    trend.Strength = 0;

                    // Remove from all affected category ActiveTrendIds
                    RemoveTrendFromCategories(trend.Id);

                    _eventBus.Publish(new TrendChangedEvent(trend.Id, trend.Type, false, 0));

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[MarketTickProcessor] Trend expired. TrendId: {trend.Id}, Type: {trend.Type}, Date: {context.CurrentDate}");
                }
                else
                {
                    // Decay the trend's strength
                    int newStrength = _marketService.ComputeTrendDecay(trend, context.CurrentDate);

                    if (newStrength != trend.Strength)
                    {
                        trend.Strength = newStrength;
                        _eventBus.Publish(new TrendChangedEvent(trend.Id, trend.Type, true, newStrength));

                        DebugLogger.Log(DebugCategory.Simulation,
                            $"[MarketTickProcessor] Trend decayed. TrendId: {trend.Id}, Type: {trend.Type}, Strength: {newStrength}, Date: {context.CurrentDate}");
                    }
                }
            }
        }

        // ── Monthly trend spawning ────────────────────────────────────────────────

        private void ProcessMonthlyTrendSpawning(TickContext context)
        {
            int spawnRoll = _random.Next(100);
            if (spawnRoll >= _tuning.TrendSpawnChancePerMonthPercent)
            {
                return;
            }

            var allCategories = new List<MarketCategoryType>((MarketCategoryType[])Enum.GetValues(typeof(MarketCategoryType)));

            TrendRuntimeState newTrend = _marketService.GenerateTrend(
                _sessionState.TrendStates,
                _tuning,
                context.CurrentDate,
                _random,
                allCategories);

            if (newTrend == null)
            {
                return;
            }

            _sessionState.TrendStates.Add(newTrend);

            // Register the trend ID in each affected market category
            if (newTrend.AffectedCategories != null)
            {
                foreach (var categoryType in newTrend.AffectedCategories)
                {
                    var categoryState = FindCategory(categoryType);
                    if (categoryState != null)
                    {
                        categoryState.ActiveTrendIds.Add(newTrend.Id);
                    }
                }
            }

            _eventBus.Publish(new TrendChangedEvent(newTrend.Id, newTrend.Type, true, newTrend.Strength));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[MarketTickProcessor] Trend spawned. TrendId: {newTrend.Id}, Type: {newTrend.Type}, " +
                $"Strength: {newTrend.Strength}, AffectedCategories: {newTrend.AffectedCategories?.Count ?? 0}, Date: {context.CurrentDate}");
        }

        // ── Monthly preference drift ──────────────────────────────────────────────

        private void ProcessMonthlyPreferenceDrift()
        {
            foreach (var category in _sessionState.MarketCategoryStates)
            {
                var activeTrends = _marketService.GetActiveTrendsForCategory(_sessionState.TrendStates, category.CategoryType);
                _marketService.ComputePreferenceDrift(category, activeTrends, _tuning, _random);
            }
        }

        // ── Monthly expectation growth ────────────────────────────────────────────

        private void ProcessMonthlyExpectationGrowth()
        {
            foreach (var category in _sessionState.MarketCategoryStates)
            {
                _marketService.ComputeExpectationGrowth(category, _tuning);
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private void RemoveTrendFromCategories(string trendId)
        {
            foreach (var category in _sessionState.MarketCategoryStates)
            {
                category.ActiveTrendIds?.Remove(trendId);
            }
        }

        private MarketCategoryRuntimeState FindCategory(MarketCategoryType categoryType)
        {
            foreach (var category in _sessionState.MarketCategoryStates)
            {
                if (category.CategoryType == categoryType)
                {
                    return category;
                }
            }

            return null;
        }
    }
}
