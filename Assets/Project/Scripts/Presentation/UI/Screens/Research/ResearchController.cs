using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Research screen.
    /// Builds a [Placeholder] static ViewModel with 11 research tracks, available projects,
    /// locked projects, and assigned research data.
    /// Wires view click callbacks: track selection filters projects; project clicks open detail modal.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ResearchController
    {
        private readonly ResearchView _view;
        private readonly IScreenRouter _screenRouter;
        private readonly IModalRouter  _modalRouter;

        // ─── State ───────────────────────────────────────────────────────────────────

        /// <summary>Currently selected track ID. Defaults to first track on init.</summary>
        private string _activeTrackId;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public ResearchController(
            ResearchView  view,
            IScreenRouter screenRouter,
            IModalRouter  modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnTrackSelected  += HandleTrackSelected;
            _view.OnProjectClicked += HandleProjectClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            _activeTrackId = TrackIds.SoftwareEngineering;
            ResearchViewModel viewModel = BuildViewModel(_activeTrackId);
            _view.Bind(viewModel);
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleTrackSelected(string trackId)
        {
            if (string.IsNullOrEmpty(trackId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ResearchController: track selected — '{trackId}'.");

            _activeTrackId = trackId;

            // Rebuild ViewModel with the new active track to filter the project lists.
            ResearchViewModel viewModel = BuildViewModel(_activeTrackId);
            _view.Bind(viewModel);
        }

        private void HandleProjectClicked(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ResearchController: project clicked — '{projectId}'. " +
                "[Placeholder] Research detail modal wiring deferred to Phase 6+.");

            // [Placeholder] — Phase 6+ will open modal.research_detail with this project ID as context.
            // For now, log only so the screen is interactive without crashing.
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] ResearchViewModel for Phase 5.
        /// All values are hardcoded demo data. Replace with service calls in Phase 6+.
        /// </summary>
        private static ResearchViewModel BuildViewModel(string activeTrackId)
        {
            IReadOnlyList<ResearchTrackViewModel>      tracks            = BuildTracks(activeTrackId);
            IReadOnlyList<ResearchProjectRowViewModel> availableProjects = BuildAvailableProjects(activeTrackId);
            IReadOnlyList<ResearchProjectRowViewModel> lockedProjects    = BuildLockedProjects(activeTrackId);
            AssignedResearchViewModel                  assignedResearch  = BuildAssignedResearch();

            return new ResearchViewModel(
                screenTitle:            "Research",
                screenSubtitle:         "[Placeholder] Phase 5 static data — Phase 6+ wires live simulation.",
                isLoading:              false,
                hasError:               false,
                errorMessage:           string.Empty,
                emptyStateTitle:        "No Research Tracks",
                emptyStateBody:         "Research tracks will appear here when available.",
                tracks:                 tracks,
                activeTrackId:          activeTrackId,
                availableProjects:      availableProjects,
                lockedProjects:         lockedProjects,
                assignedResearch:       assignedResearch,
                hasNoAvailableProjects: availableProjects == null || availableProjects.Count == 0);
        }

        // ─── Private — track builders ─────────────────────────────────────────────────

        private static IReadOnlyList<ResearchTrackViewModel> BuildTracks(string activeTrackId)
        {
            return new List<ResearchTrackViewModel>
            {
                new ResearchTrackViewModel(
                    id:            TrackIds.SoftwareEngineering,
                    name:          "Software Engineering",
                    projectCount:  "5 projects",
                    semanticState: activeTrackId == TrackIds.SoftwareEngineering ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.HardwareEngineering,
                    name:          "Hardware Engineering",
                    projectCount:  "4 projects",
                    semanticState: activeTrackId == TrackIds.HardwareEngineering ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.CloudInfrastructure,
                    name:          "Cloud Infrastructure",
                    projectCount:  "4 projects",
                    semanticState: activeTrackId == TrackIds.CloudInfrastructure ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.Security,
                    name:          "Security",
                    projectCount:  "3 projects",
                    semanticState: activeTrackId == TrackIds.Security ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.AiAutomation,
                    name:          "AI & Automation",
                    projectCount:  "4 projects",
                    semanticState: activeTrackId == TrackIds.AiAutomation ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.UxProductDesign,
                    name:          "UX / Product Design",
                    projectCount:  "3 projects",
                    semanticState: activeTrackId == TrackIds.UxProductDesign ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.DeveloperTools,
                    name:          "Developer Tools",
                    projectCount:  "4 projects",
                    semanticState: activeTrackId == TrackIds.DeveloperTools ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.Manufacturing,
                    name:          "Manufacturing",
                    projectCount:  "3 projects",
                    semanticState: activeTrackId == TrackIds.Manufacturing ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.MarketingAnalytics,
                    name:          "Marketing Analytics",
                    projectCount:  "3 projects",
                    semanticState: activeTrackId == TrackIds.MarketingAnalytics ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.SupportOperations,
                    name:          "Support Operations",
                    projectCount:  "3 projects",
                    semanticState: activeTrackId == TrackIds.SupportOperations ? "active" : "default"),

                new ResearchTrackViewModel(
                    id:            TrackIds.PlatformEcosystems,
                    name:          "Platform Ecosystems",
                    projectCount:  "3 projects",
                    semanticState: activeTrackId == TrackIds.PlatformEcosystems ? "active" : "default"),
            };
        }

        // ─── Private — available project builders ─────────────────────────────────────

        /// <summary>
        /// Returns 4–6 available projects filtered by the active track.
        /// Software Engineering track receives the richest set of demo data for Phase 5.
        /// Other tracks return a generic 2-project placeholder set.
        /// </summary>
        private static IReadOnlyList<ResearchProjectRowViewModel> BuildAvailableProjects(string activeTrackId)
        {
            if (activeTrackId == TrackIds.SoftwareEngineering)
            {
                return BuildSoftwareEngineeringAvailableProjects();
            }

            if (activeTrackId == TrackIds.AiAutomation)
            {
                return BuildAiAutomationAvailableProjects();
            }

            // Generic placeholder for remaining tracks
            return BuildGenericAvailableProjects(activeTrackId);
        }

        private static IReadOnlyList<ResearchProjectRowViewModel> BuildSoftwareEngineeringAvailableProjects()
        {
            return new List<ResearchProjectRowViewModel>
            {
                new ResearchProjectRowViewModel(
                    id:                 "research.project.compiler_optimisation",
                    name:               "Compiler Optimisation",
                    track:              "Software Engineering",
                    requiredSkill:      "Compiler Engineering",
                    duration:           "8 weeks",
                    cost:               "$120,000",
                    unlocks:            "Advanced Compiler Toolkit",
                    riskLevel:          "Low",
                    obsolescenceRisk:   "Low",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    "Alpha App",
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),

                new ResearchProjectRowViewModel(
                    id:                 "research.project.memory_management",
                    name:               "Advanced Memory Management",
                    track:              "Software Engineering",
                    requiredSkill:      "Systems Programming",
                    duration:           "10 weeks",
                    cost:               "$95,000",
                    unlocks:            "Low-Latency Runtime",
                    riskLevel:          "Medium",
                    obsolescenceRisk:   "Low",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    string.Empty,
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),

                new ResearchProjectRowViewModel(
                    id:                 "research.project.distributed_testing",
                    name:               "Distributed Test Infrastructure",
                    track:              "Software Engineering",
                    requiredSkill:      "QA Engineering",
                    duration:           "6 weeks",
                    cost:               "$60,000",
                    unlocks:            "Parallel Test Runner",
                    riskLevel:          "Low",
                    obsolescenceRisk:   "Medium",
                    prerequisites:      string.Empty,
                    assignedTeam:       "Engineering Team A",
                    completionEstimate: "Week 14",
                    relatedProducts:    "Beta Service",
                    status:             "In Progress",
                    semanticState:      "in-progress",
                    isClickable:        true,
                    isLocked:           false),

                new ResearchProjectRowViewModel(
                    id:                 "research.project.static_analysis",
                    name:               "Static Analysis Toolchain",
                    track:              "Software Engineering",
                    requiredSkill:      "Developer Experience",
                    duration:           "5 weeks",
                    cost:               "$45,000",
                    unlocks:            "Code Quality Dashboard",
                    riskLevel:          "Low",
                    obsolescenceRisk:   "Low",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    string.Empty,
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),
            };
        }

        private static IReadOnlyList<ResearchProjectRowViewModel> BuildAiAutomationAvailableProjects()
        {
            return new List<ResearchProjectRowViewModel>
            {
                new ResearchProjectRowViewModel(
                    id:                 "research.project.ml_pipeline",
                    name:               "ML Training Pipeline",
                    track:              "AI & Automation",
                    requiredSkill:      "Machine Learning",
                    duration:           "12 weeks",
                    cost:               "$200,000",
                    unlocks:            "Automated Model Training",
                    riskLevel:          "High",
                    obsolescenceRisk:   "Medium",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    string.Empty,
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),

                new ResearchProjectRowViewModel(
                    id:                 "research.project.data_labelling",
                    name:               "Data Labelling Framework",
                    track:              "AI & Automation",
                    requiredSkill:      "Data Engineering",
                    duration:           "6 weeks",
                    cost:               "$75,000",
                    unlocks:            "Labelling Workflow Suite",
                    riskLevel:          "Low",
                    obsolescenceRisk:   "Low",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    string.Empty,
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),

                new ResearchProjectRowViewModel(
                    id:                 "research.project.inference_optimisation",
                    name:               "Inference Optimisation",
                    track:              "AI & Automation",
                    requiredSkill:      "ML Systems",
                    duration:           "9 weeks",
                    cost:               "$140,000",
                    unlocks:            "Edge Inference Runtime",
                    riskLevel:          "Medium",
                    obsolescenceRisk:   "High",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    "Alpha App",
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),
            };
        }

        private static IReadOnlyList<ResearchProjectRowViewModel> BuildGenericAvailableProjects(string activeTrackId)
        {
            // [Placeholder] Generic two-project set for tracks without dedicated Phase 5 data.
            string trackDisplay = FormatTrackName(activeTrackId);

            return new List<ResearchProjectRowViewModel>
            {
                new ResearchProjectRowViewModel(
                    id:                 $"research.project.placeholder_a.{activeTrackId}",
                    name:               $"[Placeholder] {trackDisplay} Research A",
                    track:              trackDisplay,
                    requiredSkill:      "[Placeholder] Skill",
                    duration:           "8 weeks",
                    cost:               "$80,000",
                    unlocks:            "[Placeholder] Capability",
                    riskLevel:          "Medium",
                    obsolescenceRisk:   "Low",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    string.Empty,
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),

                new ResearchProjectRowViewModel(
                    id:                 $"research.project.placeholder_b.{activeTrackId}",
                    name:               $"[Placeholder] {trackDisplay} Research B",
                    track:              trackDisplay,
                    requiredSkill:      "[Placeholder] Skill",
                    duration:           "6 weeks",
                    cost:               "$55,000",
                    unlocks:            "[Placeholder] Capability",
                    riskLevel:          "Low",
                    obsolescenceRisk:   "Low",
                    prerequisites:      string.Empty,
                    assignedTeam:       string.Empty,
                    completionEstimate: "Unassigned",
                    relatedProducts:    string.Empty,
                    status:             "Available",
                    semanticState:      "available",
                    isClickable:        true,
                    isLocked:           false),
            };
        }

        // ─── Private — locked project builders ────────────────────────────────────────

        /// <summary>
        /// Returns 3–4 locked projects filtered by the active track.
        /// Locked projects show prerequisites clearly; hidden formula values are not revealed per Section 14 lock.
        /// </summary>
        private static IReadOnlyList<ResearchProjectRowViewModel> BuildLockedProjects(string activeTrackId)
        {
            if (activeTrackId == TrackIds.SoftwareEngineering)
            {
                return BuildSoftwareEngineeringLockedProjects();
            }

            // Generic placeholder locked projects for other tracks
            return BuildGenericLockedProjects(activeTrackId);
        }

        private static IReadOnlyList<ResearchProjectRowViewModel> BuildSoftwareEngineeringLockedProjects()
        {
            return new List<ResearchProjectRowViewModel>
            {
                new ResearchProjectRowViewModel(
                    id:                 "research.project.jit_compiler",
                    name:               "JIT Compiler Framework",
                    track:              "Software Engineering",
                    requiredSkill:      "Compiler Engineering",
                    duration:           "16 weeks",
                    cost:               "$250,000",
                    unlocks:            "Dynamic Code Optimisation",
                    riskLevel:          "High",
                    obsolescenceRisk:   "Low",
                    prerequisites:      "Compiler Optimisation must be completed",
                    assignedTeam:       string.Empty,
                    completionEstimate: "—",
                    relatedProducts:    string.Empty,
                    status:             "Locked",
                    semanticState:      "locked",
                    isClickable:        true,
                    isLocked:           true),

                new ResearchProjectRowViewModel(
                    id:                 "research.project.formal_verification",
                    name:               "Formal Verification System",
                    track:              "Software Engineering",
                    requiredSkill:      "Formal Methods",
                    duration:           "20 weeks",
                    cost:               "$320,000",
                    unlocks:            "Certified Safety Runtime",
                    riskLevel:          "High",
                    obsolescenceRisk:   "Low",
                    prerequisites:      "Static Analysis Toolchain and Advanced Memory Management must be completed",
                    assignedTeam:       string.Empty,
                    completionEstimate: "—",
                    relatedProducts:    string.Empty,
                    status:             "Locked",
                    semanticState:      "locked",
                    isClickable:        true,
                    isLocked:           true),

                new ResearchProjectRowViewModel(
                    id:                 "research.project.runtime_telemetry",
                    name:               "Runtime Telemetry Platform",
                    track:              "Software Engineering",
                    requiredSkill:      "Observability Engineering",
                    duration:           "10 weeks",
                    cost:               "$110,000",
                    unlocks:            "Live Diagnostics Dashboard",
                    riskLevel:          "Medium",
                    obsolescenceRisk:   "Medium",
                    prerequisites:      "Distributed Test Infrastructure must be completed",
                    assignedTeam:       string.Empty,
                    completionEstimate: "—",
                    relatedProducts:    string.Empty,
                    status:             "Locked",
                    semanticState:      "locked",
                    isClickable:        true,
                    isLocked:           true),
            };
        }

        private static IReadOnlyList<ResearchProjectRowViewModel> BuildGenericLockedProjects(string activeTrackId)
        {
            // [Placeholder] Generic locked project set for tracks without dedicated Phase 5 data.
            string trackDisplay = FormatTrackName(activeTrackId);

            return new List<ResearchProjectRowViewModel>
            {
                new ResearchProjectRowViewModel(
                    id:                 $"research.project.locked_a.{activeTrackId}",
                    name:               $"[Placeholder] {trackDisplay} Advanced Research A",
                    track:              trackDisplay,
                    requiredSkill:      "[Placeholder] Advanced Skill",
                    duration:           "14 weeks",
                    cost:               "$180,000",
                    unlocks:            "[Placeholder] Advanced Capability",
                    riskLevel:          "High",
                    obsolescenceRisk:   "Medium",
                    prerequisites:      "[Placeholder] Basic research must be completed",
                    assignedTeam:       string.Empty,
                    completionEstimate: "—",
                    relatedProducts:    string.Empty,
                    status:             "Locked",
                    semanticState:      "locked",
                    isClickable:        true,
                    isLocked:           true),
            };
        }

        // ─── Private — assigned research builder ──────────────────────────────────────

        private static AssignedResearchViewModel BuildAssignedResearch()
        {
            // [Placeholder] One assigned research project shown in the assigned panel.
            // Core simulation wiring in Phase 6+. Assign Team action is placeholder.
            return new AssignedResearchViewModel(
                hasAssignedResearch: true,
                projectName:         "Distributed Test Infrastructure",
                assignedTeam:        "Engineering Team A",
                progress:            "62%",
                completionEstimate:  "Week 14",
                semanticState:       "in-progress");
        }

        // ─── Private — track name formatting helper ───────────────────────────────────

        private static string FormatTrackName(string trackId)
        {
            // Simple stable-ID to display name mapping for placeholder purposes.
            switch (trackId)
            {
                case TrackIds.SoftwareEngineering:   return "Software Engineering";
                case TrackIds.HardwareEngineering:   return "Hardware Engineering";
                case TrackIds.CloudInfrastructure:   return "Cloud Infrastructure";
                case TrackIds.Security:              return "Security";
                case TrackIds.AiAutomation:          return "AI & Automation";
                case TrackIds.UxProductDesign:       return "UX / Product Design";
                case TrackIds.DeveloperTools:        return "Developer Tools";
                case TrackIds.Manufacturing:         return "Manufacturing";
                case TrackIds.MarketingAnalytics:    return "Marketing Analytics";
                case TrackIds.SupportOperations:     return "Support Operations";
                case TrackIds.PlatformEcosystems:    return "Platform Ecosystems";
                default:                             return trackId;
            }
        }

        // ─── Private — stable track ID constants ──────────────────────────────────────

        /// <summary>
        /// Stable track ID constants scoped to the controller.
        /// Matches the track IDs documented in ResearchTrackViewModel.
        /// </summary>
        private static class TrackIds
        {
            public const string SoftwareEngineering = "track.software_engineering";
            public const string HardwareEngineering = "track.hardware_engineering";
            public const string CloudInfrastructure = "track.cloud_infrastructure";
            public const string Security            = "track.security";
            public const string AiAutomation        = "track.ai_automation";
            public const string UxProductDesign     = "track.ux_product_design";
            public const string DeveloperTools      = "track.developer_tools";
            public const string Manufacturing       = "track.manufacturing";
            public const string MarketingAnalytics  = "track.marketing_analytics";
            public const string SupportOperations   = "track.support_operations";
            public const string PlatformEcosystems  = "track.platform_ecosystems";
        }
    }
}
