using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Controllers
{
    /// <summary>
    /// View controller that manages sidebar navigation interaction.
    /// Wires nav item click callbacks to the ScreenRouter and keeps
    /// the active state class in sync with router state changes.
    ///
    /// Screen ID associations are declared in a static name-to-ID map that mirrors
    /// the element names defined in MainShell.uxml. This avoids reliance on custom
    /// UXML attributes that are not supported at runtime in Unity 6.
    /// </summary>
    public sealed class SidebarView
    {
        private const string GroupClass       = "sidebar__group";
        private const string GroupHeaderClass = "sidebar__group-header";
        private const string GroupItemsClass  = "sidebar__group-items";
        private const string IsActiveClass    = "is-active";
        private const string IsCollapsedClass = "is-collapsed";

        /// <summary>
        /// Maps UXML element name → stable screen ID.
        /// Must stay in sync with element names in MainShell.uxml.
        /// </summary>
        private static readonly Dictionary<string, string> ElementNameToScreenId = new()
        {
            { "NavItemDashboard",      ScreenIds.Portal         },
            { "NavItemInbox",          ScreenIds.Reports        },
            { "NavItemCalendar",       ScreenIds.Calendar       },
            { "NavItemCompany",        ScreenIds.Company        },
            { "NavItemRecruitment",    ScreenIds.Recruitment    },
            { "NavItemEmployees",      ScreenIds.Employees      },
            { "NavItemTeams",          ScreenIds.Teams          },
            { "NavItemProducts",       ScreenIds.Products       },
            { "NavItemContracts",      ScreenIds.Contracts      },
            { "NavItemResearch",       ScreenIds.Research       },
            { "NavItemInfrastructure", ScreenIds.Infrastructure },
            { "NavItemMarket",         ScreenIds.Market         },
            { "NavItemCompetitors",    ScreenIds.Competitors    },
            { "NavItemFinance",        ScreenIds.Finance        },
            { "NavItemReports",        ScreenIds.ReportsFinance },
        };

        private readonly VisualElement _sidebarRoot;
        private readonly IScreenRouter _screenRouter;

        // Maps screenId → nav item VisualElement for fast active-state updates.
        private readonly Dictionary<string, VisualElement> _navItemMap;

        /// <param name="sidebarRoot">The SidebarNavigation root element from the shell UXML.</param>
        /// <param name="screenRouter">The screen router to drive navigation.</param>
        public SidebarView(VisualElement sidebarRoot, IScreenRouter screenRouter)
        {
            _sidebarRoot  = sidebarRoot  ?? throw new System.ArgumentNullException(nameof(sidebarRoot));
            _screenRouter = screenRouter ?? throw new System.ArgumentNullException(nameof(screenRouter));
            _navItemMap   = new Dictionary<string, VisualElement>(System.StringComparer.Ordinal);

            BuildNavItemMap();
            RegisterNavItemCallbacks();
            RegisterGroupToggleCallbacks();

            // Keep active state in sync when the router navigates programmatically.
            _screenRouter.ScreenChanged += OnScreenChanged;
        }

        // ─── Public API ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Highlights the nav item that matches the given screen ID.
        /// Removes the active class from all other nav items.
        /// </summary>
        public void SetActiveItem(string screenId)
        {
            foreach (KeyValuePair<string, VisualElement> entry in _navItemMap)
            {
                entry.Value.EnableInClassList(IsActiveClass, entry.Key == screenId);
            }
        }

        // ─── Private ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds a screenId → VisualElement map by querying nav items by name
        /// and resolving their screen IDs from the static ElementNameToScreenId map.
        /// </summary>
        private void BuildNavItemMap()
        {
            foreach (KeyValuePair<string, string> entry in ElementNameToScreenId)
            {
                VisualElement navItem = _sidebarRoot.Q(entry.Key);

                if (navItem != null)
                {
                    _navItemMap[entry.Value] = navItem;
                }
                else
                {
                    DebugLogger.LogWarning(DebugCategory.Navigation,
                        $"SidebarView: nav item element '{entry.Key}' not found in sidebar UXML. " +
                        "Check that the element name matches MainShell.uxml.");
                }
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"SidebarView: built nav item map with {_navItemMap.Count} entries.");
        }

        /// <summary>Registers click callbacks on every mapped nav item.</summary>
        private void RegisterNavItemCallbacks()
        {
            foreach (KeyValuePair<string, VisualElement> entry in _navItemMap)
            {
                // Capture local copies for the lambda to avoid closure over loop variable.
                string screenId  = entry.Key;
                VisualElement el = entry.Value;

                el.RegisterCallback<ClickEvent>(_ =>
                {
                    DebugLogger.Log(DebugCategory.Navigation,
                        $"SidebarView: nav item clicked — '{screenId}'.");
                    _screenRouter.OpenScreen(screenId);
                });
            }
        }

        /// <summary>
        /// Registers click callbacks on group headers to toggle group collapse/expand.
        /// Toggles <c>is-collapsed</c> on the group root and shows/hides the items container.
        /// </summary>
        private void RegisterGroupToggleCallbacks()
        {
            UQueryBuilder<VisualElement> groups = _sidebarRoot.Query<VisualElement>(className: GroupClass);
            groups.ForEach(group =>
            {
                VisualElement header = group.Q(className: GroupHeaderClass);
                VisualElement items  = group.Q(className: GroupItemsClass);

                if (header == null || items == null)
                {
                    return;
                }

                header.RegisterCallback<ClickEvent>(_ =>
                {
                    bool collapsed = group.ClassListContains(IsCollapsedClass);

                    if (collapsed)
                    {
                        group.RemoveFromClassList(IsCollapsedClass);
                        items.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        group.AddToClassList(IsCollapsedClass);
                        items.style.display = DisplayStyle.None;
                    }

                    DebugLogger.Log(DebugCategory.UI,
                        $"SidebarView: group '{group.name}' toggled to {(collapsed ? "expanded" : "collapsed")}.");
                });
            });
        }

        /// <summary>Responds to screen router state changes to keep the active highlight in sync.</summary>
        private void OnScreenChanged(string screenId)
        {
            SetActiveItem(screenId);
        }
    }
}
