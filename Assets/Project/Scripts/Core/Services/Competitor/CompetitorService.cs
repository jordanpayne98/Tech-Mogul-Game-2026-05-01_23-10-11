using System;
using System.Collections.Generic;
using Project.Core.Definitions.Market;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Competitor
{
    /// <summary>
    /// Stateless core domain service for AI competitor simulation.
    /// Implements profile generation, action selection, product launch, price change,
    /// market expansion, and market share scoring/distribution logic.
    /// Does not hold state references and does not publish events.
    /// Defined in Plan 2J, GDD_10.
    /// </summary>
    public sealed class CompetitorService : ICompetitorService
    {
        // ── Placeholder competitor names [Placeholder] ─────────────────────────────

        private static readonly string[] PlaceholderNames =
        {
            "Apex Industries",
            "Nova Systems",
            "Titan Corp",
            "Vertex Labs",
            "Zenith Technologies",
            "Polar Dynamics",
            "Nexus Software",
            "Catalyst Group",
            "Meridian Tech",
            "Stratos Digital"
        };

        // ── ICompetitorService — Initialization ───────────────────────────────────

        public CompetitorProfile GenerateCompetitorProfile(
            ICompetitorTuning           tuning,
            List<MarketCategoryType>    allCategories,
            Random                      random)
        {
            string id = Guid.NewGuid().ToString("N");

            var archetypeValues      = (CompetitorArchetype[])Enum.GetValues(typeof(CompetitorArchetype));
            var pricingStyleValues   = (CompetitorPricingStyle[])Enum.GetValues(typeof(CompetitorPricingStyle));
            var marketingStyleValues = (CompetitorMarketingStyle[])Enum.GetValues(typeof(CompetitorMarketingStyle));

            var archetype      = archetypeValues[random.Next(archetypeValues.Length)];
            var pricingStyle   = pricingStyleValues[random.Next(pricingStyleValues.Length)];
            var marketingStyle = marketingStyleValues[random.Next(marketingStyleValues.Length)];

            var defaults = CompetitorArchetypeDefaults.GetDefaults(archetype);

            // Pick 1–2 random market focus categories.
            int focusCount = random.Next(1, 3);
            var shuffled   = new List<MarketCategoryType>(allCategories);
            ShuffleList(shuffled, random);
            var marketFocus = shuffled.GetRange(0, Math.Min(focusCount, shuffled.Count));

            string name = PlaceholderNames[random.Next(PlaceholderNames.Length)];

            return new CompetitorProfile
            {
                Id             = id,
                Name           = name,
                Archetype      = archetype,
                MarketFocus    = marketFocus,
                RiskAppetite   = defaults.RiskAppetite,
                PricingStyle   = pricingStyle,
                MarketingStyle = marketingStyle
            };
        }

        public CompetitorRuntimeState CreateCompetitorRuntimeState(CompetitorProfile profile)
        {
            var defaults = CompetitorArchetypeDefaults.GetDefaults(profile.Archetype);

            return new CompetitorRuntimeState
            {
                CompetitorId            = profile.Id,
                CashStrength            = defaults.CashStrength,
                Reputation              = defaults.Reputation,
                HiringStrength          = defaults.HiringStrength,
                ResearchStrength        = defaults.ResearchStrength,
                LaunchCadence           = defaults.LaunchCadence,
                ProductIds              = new List<string>(),
                MarketShareBasisPoints  = new System.Collections.Generic.Dictionary<MarketCategoryType, int>(),
                LastLaunchDate          = null
            };
        }

        public CompetitorProductRuntimeState GenerateInitialProduct(
            CompetitorProfile           profile,
            CompetitorRuntimeState      state,
            ICompetitorTuning           tuning,
            GameDateTime                currentDate,
            Random                      random)
        {
            var product = LaunchProduct(profile, state, tuning, currentDate, random);
            state.ProductIds.Add(product.Id);
            state.LastLaunchDate = currentDate;
            return product;
        }

        // ── ICompetitorService — Action Selection ─────────────────────────────────

        public CompetitorActionType? SelectAction(
            CompetitorRuntimeState      state,
            CompetitorProfile           profile,
            ICompetitorTuning           tuning,
            GameDateTime                currentDate,
            Random                      random)
        {
            // Roll action chance gate.
            if (random.Next(100) >= tuning.CompetitorActionChancePerWeekPercent)
            {
                return null;
            }

            // Weighted action roll.
            int launchWeight     = tuning.CompetitorLaunchChanceWeight;
            int priceWeight      = tuning.CompetitorPriceChangeChanceWeight;
            int expandWeight     = tuning.CompetitorExpandChanceWeight;
            int totalWeight      = launchWeight + priceWeight + expandWeight;

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = random.Next(totalWeight);

            if (roll < launchWeight)
            {
                return CompetitorActionType.LaunchProduct;
            }

            if (roll < launchWeight + priceWeight)
            {
                return CompetitorActionType.ChangePrice;
            }

            return CompetitorActionType.EnterMarket;
        }

        // ── ICompetitorService — Launch ───────────────────────────────────────────

        public bool CanLaunch(
            CompetitorRuntimeState      state,
            GameDateTime                currentDate,
            ICompetitorTuning           tuning,
            Random                      random)
        {
            // Cadence gate.
            if (state.LastLaunchDate != null)
            {
                int daysSinceLastLaunch = (currentDate.TotalElapsedHours - state.LastLaunchDate.TotalElapsedHours)
                                        / GameDateTime.HoursPerDay;
                int minDaysBetweenLaunches = Math.Max(7, 30 - state.LaunchCadence / 5);

                if (daysSinceLastLaunch < minDaysBetweenLaunches)
                {
                    return false;
                }
            }

            // Monthly chance gate.
            if (random.Next(100) >= tuning.CompetitorLaunchChancePerMonthPercent)
            {
                return false;
            }

            return true;
        }

        public CompetitorProductRuntimeState LaunchProduct(
            CompetitorProfile           profile,
            CompetitorRuntimeState      state,
            ICompetitorTuning           tuning,
            GameDateTime                currentDate,
            Random                      random)
        {
            // Pick random category from market focus.
            var category = profile.MarketFocus.Count > 0
                ? profile.MarketFocus[random.Next(profile.MarketFocus.Count)]
                : MarketCategoryType.Game;

            // formula.competitor.launch_quality
            int baseQuality   = random.Next(tuning.CompetitorBaseQualityMin, tuning.CompetitorBaseQualityMax + 1);
            int researchBonus = state.ResearchStrength / 5; // 0–20
            int quality       = Math.Max(0, Math.Min(100, baseQuality + researchBonus));

            // formula.competitor.launch_price
            long basePriceRange = tuning.CompetitorBasePriceMaxMinorUnits - tuning.CompetitorBasePriceMinMinorUnits;
            long basePriceRaw   = basePriceRange > 0
                ? tuning.CompetitorBasePriceMinMinorUnits + (long)(random.NextDouble() * basePriceRange)
                : tuning.CompetitorBasePriceMinMinorUnits;

            float priceMultiplier = GetPriceMultiplier(profile.PricingStyle, tuning);
            long  price           = Math.Max(tuning.CompetitorMinPriceMinorUnits, (long)(basePriceRaw * priceMultiplier));

            // formula.competitor.launch_marketing
            int baseMarketing = random.Next(20, 60);
            int styleBonus    = GetMarketingStyleBonus(profile.MarketingStyle);
            int cashBonus     = state.CashStrength / 5; // 0–20
            int marketing     = Math.Max(0, Math.Min(100, baseMarketing + styleBonus + cashBonus));

            return new CompetitorProductRuntimeState
            {
                Id                     = Guid.NewGuid().ToString("N"),
                CompetitorId           = profile.Id,
                Name                   = GenerateProductName(profile, category),
                Category               = category,
                PriceMinorUnits        = price,
                QualityScore           = quality,
                MarketingStrength      = marketing,
                MarketShareBasisPoints = 0,
                LaunchDate             = currentDate,
                IsActive               = true
            };
        }

        // ── ICompetitorService — Price Change ─────────────────────────────────────

        public (string productId, long newPrice)? ChangePrice(
            CompetitorRuntimeState                  state,
            List<CompetitorProductRuntimeState>     products,
            ICompetitorTuning                       tuning,
            Random                                  random)
        {
            // Filter to active products owned by this competitor.
            var active = new List<CompetitorProductRuntimeState>();
            foreach (var p in products)
            {
                if (p.IsActive && p.CompetitorId == state.CompetitorId)
                {
                    active.Add(p);
                }
            }

            if (active.Count == 0)
            {
                return null;
            }

            var target       = active[random.Next(active.Count)];
            int rangePercent = tuning.CompetitorPriceChangeRangePercent;
            int changePct    = random.Next(-rangePercent, rangePercent + 1);

            long newPrice = (long)(target.PriceMinorUnits * (1.0 + changePct / 100.0));
            newPrice      = Math.Max(tuning.CompetitorMinPriceMinorUnits, newPrice);

            return (target.Id, newPrice);
        }

        // ── ICompetitorService — Market Expansion ─────────────────────────────────

        public MarketCategoryType? ExpandMarket(
            CompetitorProfile           profile,
            List<MarketCategoryType>    allCategories,
            Random                      random)
        {
            var available = new List<MarketCategoryType>();
            foreach (var cat in allCategories)
            {
                if (!profile.MarketFocus.Contains(cat))
                {
                    available.Add(cat);
                }
            }

            if (available.Count == 0)
            {
                return null;
            }

            return available[random.Next(available.Count)];
        }

        // ── ICompetitorService — Market Share ─────────────────────────────────────

        public int ComputePricePercentile(long priceMinorUnits, List<long> allPricesInCategory)
        {
            if (allPricesInCategory == null || allPricesInCategory.Count <= 1)
            {
                return 50;
            }

            // Check if all prices are equal.
            long first = allPricesInCategory[0];
            bool allSame = true;
            foreach (var p in allPricesInCategory)
            {
                if (p != first) { allSame = false; break; }
            }

            if (allSame)
            {
                return 50;
            }

            // Sort ascending (copy).
            var sorted = new List<long>(allPricesInCategory);
            sorted.Sort();

            // Find rank (0-based index) of this price.
            int rank = sorted.IndexOf(priceMinorUnits);
            if (rank < 0) rank = 0;

            return rank * 100 / (sorted.Count - 1);
        }

        public float ComputeProductScore(
            int                 qualityValue,
            int                 priceScore,
            int                 marketingValue,
            ICompetitorTuning   tuning)
        {
            int totalWeight = tuning.MarketShareQualityWeight
                            + tuning.MarketSharePriceWeight
                            + tuning.MarketShareMarketingWeight;

            if (totalWeight <= 0)
            {
                return 0f;
            }

            return (qualityValue   * tuning.MarketShareQualityWeight
                  + priceScore     * tuning.MarketSharePriceWeight
                  + marketingValue * tuning.MarketShareMarketingWeight)
                  / (float)totalWeight;
        }

        public List<(string productId, int shareBasisPoints)> DistributeShares(
            List<(string productId, float score)> productScores)
        {
            var result = new List<(string productId, int shareBasisPoints)>();

            if (productScores == null || productScores.Count == 0)
            {
                return result;
            }

            float totalScore = 0f;
            foreach (var (_, score) in productScores)
            {
                totalScore += score;
            }
            totalScore = Math.Max(1.0f, totalScore);

            // Floor each share and track remainders.
            var floors     = new int[productScores.Count];
            var remainders = new double[productScores.Count];
            int totalFloored = 0;

            for (int i = 0; i < productScores.Count; i++)
            {
                double rawShare   = productScores[i].score / totalScore * 10000.0;
                floors[i]         = (int)Math.Floor(rawShare);
                remainders[i]     = rawShare - floors[i];
                totalFloored     += floors[i];
            }

            // Largest-remainder: distribute deficit via descending remainder order.
            int deficit = 10000 - totalFloored;

            // Build index array sorted by remainder descending.
            var indices = new int[productScores.Count];
            for (int i = 0; i < indices.Length; i++) indices[i] = i;
            Array.Sort(indices, (a, b) => remainders[b].CompareTo(remainders[a]));

            for (int k = 0; k < deficit && k < productScores.Count; k++)
            {
                floors[indices[k]]++;
            }

            for (int i = 0; i < productScores.Count; i++)
            {
                result.Add((productScores[i].productId, floors[i]));
            }

            return result;
        }

        // ── Private Helpers ───────────────────────────────────────────────────────

        private static float GetPriceMultiplier(CompetitorPricingStyle style, ICompetitorTuning tuning)
        {
            switch (style)
            {
                case CompetitorPricingStyle.Premium:              return tuning.CompetitorPremiumPriceMultiplier;
                case CompetitorPricingStyle.LowCost:              return tuning.CompetitorLowCostPriceMultiplier;
                case CompetitorPricingStyle.AggressiveDiscounting: return tuning.CompetitorLowCostPriceMultiplier * 0.8f;
                default:                                           return 1.0f;
            }
        }

        private static int GetMarketingStyleBonus(CompetitorMarketingStyle style)
        {
            switch (style)
            {
                case CompetitorMarketingStyle.BrandHeavy:         return 25;
                case CompetitorMarketingStyle.PerformanceFocused: return 15;
                case CompetitorMarketingStyle.EnterpriseSales:    return 10;
                case CompetitorMarketingStyle.CommunityLed:       return 5;
                case CompetitorMarketingStyle.Balanced:           return 10;
                case CompetitorMarketingStyle.Minimal:            return 0;
                default:                                          return 0;
            }
        }

        private static string GenerateProductName(CompetitorProfile profile, MarketCategoryType category)
        {
            return $"{profile.Name} {category} v1";
        }

        private static void ShuffleList<T>(List<T> list, Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T   tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}
