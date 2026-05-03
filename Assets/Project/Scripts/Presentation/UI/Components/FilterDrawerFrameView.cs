using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Components
{
    /// <summary>
    /// Specialised drawer pre-structured for filter use.
    /// Builds on RightDrawerFrameView with a placeholder search input
    /// and three placeholder dropdown fields.
    ///
    /// Phase 4 content is placeholder only. Real filter logic is deferred to Phase 5.
    ///
    /// Caller pattern:
    ///   var filterDrawer = new FilterDrawerFrameView();
    ///   filterDrawer.InnerView.OnCloseRequested = () => _drawerRouter.CloseDrawer();
    ///   return filterDrawer.Root;
    /// </summary>
    public sealed class FilterDrawerFrameView
    {
        // ─── Private references ───────────────────────────────────────────────────────

        private readonly RightDrawerFrameView _innerView;
        private readonly TextField            _searchInput;
        private readonly DropdownField        _teamDropdown;
        private readonly DropdownField        _roleDropdown;
        private readonly DropdownField        _statusDropdown;

        // ─── Public API ───────────────────────────────────────────────────────────────

        /// <summary>Root element to inject into the drawer host.</summary>
        public VisualElement Root => _innerView.Root;

        /// <summary>Exposes the inner RightDrawerFrameView for callback wiring (OnCloseRequested etc.).</summary>
        public RightDrawerFrameView InnerView => _innerView;

        // ─── Constructor ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates the filter drawer with placeholder search input and dropdown fields.
        /// </summary>
        public FilterDrawerFrameView()
        {
            _innerView = new RightDrawerFrameView("Filters");

            // ── Search input ───────────────────────────────────────────────────────────
            var searchGroup = new VisualElement();
            searchGroup.AddToClassList("filter-drawer__field-group");

            _searchInput = new TextField();
            _searchInput.AddToClassList("search-input");
            // Set placeholder via value until UIToolkit TextField placeholder is set differently
            _searchInput.value = string.Empty;
            // Use label for the placeholder text (since UI Toolkit 2021+ textField has no built-in placeholder)
            // The UXML-based search input pattern places the icon via USS; here we just wire the text field.
            var searchLabel = new Label("Search...");
            searchLabel.AddToClassList("filter-drawer__field-label");
            searchGroup.Add(searchLabel);
            searchGroup.Add(_searchInput);
            _innerView.Body.Add(searchGroup);

            // ── Team dropdown ──────────────────────────────────────────────────────────
            var teamGroup = new VisualElement();
            teamGroup.AddToClassList("filter-drawer__field-group");

            var teamLabel = new UnityEngine.UIElements.Label("[Placeholder] Team");
            teamLabel.AddToClassList("filter-drawer__field-label");
            teamGroup.Add(teamLabel);

            _teamDropdown = new DropdownField();
            _teamDropdown.label = string.Empty;
            _teamDropdown.AddToClassList("dropdown-field");
            teamGroup.Add(_teamDropdown);
            _innerView.Body.Add(teamGroup);

            // ── Role dropdown ──────────────────────────────────────────────────────────
            var roleGroup = new VisualElement();
            roleGroup.AddToClassList("filter-drawer__field-group");

            var roleLabel = new UnityEngine.UIElements.Label("[Placeholder] Role");
            roleLabel.AddToClassList("filter-drawer__field-label");
            roleGroup.Add(roleLabel);

            _roleDropdown = new DropdownField();
            _roleDropdown.label = string.Empty;
            _roleDropdown.AddToClassList("dropdown-field");
            roleGroup.Add(_roleDropdown);
            _innerView.Body.Add(roleGroup);

            // ── Status dropdown ────────────────────────────────────────────────────────
            var statusGroup = new VisualElement();
            statusGroup.AddToClassList("filter-drawer__field-group");

            var statusLabel = new UnityEngine.UIElements.Label("[Placeholder] Status");
            statusLabel.AddToClassList("filter-drawer__field-label");
            statusGroup.Add(statusLabel);

            _statusDropdown = new DropdownField();
            _statusDropdown.label = string.Empty;
            _statusDropdown.AddToClassList("dropdown-field");
            statusGroup.Add(_statusDropdown);
            _innerView.Body.Add(statusGroup);

            // ── Wire clear and apply callbacks ─────────────────────────────────────────
            _innerView.OnClearRequested = ResetFields;
            _innerView.OnApplyRequested = OnApplyClicked;
        }

        // ─── Private helpers ──────────────────────────────────────────────────────────

        /// <summary>Resets all filter fields to their default empty state.</summary>
        private void ResetFields()
        {
            _searchInput.value    = string.Empty;
            _teamDropdown.value   = string.Empty;
            _roleDropdown.value   = string.Empty;
            _statusDropdown.value = string.Empty;

            DebugLogger.Log(DebugCategory.UI,
                "FilterDrawerFrameView: Clear Filters clicked. [Placeholder] All fields reset.");
        }

        /// <summary>Logs apply action. [Placeholder] Real filter logic deferred to Phase 5.</summary>
        private void OnApplyClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "FilterDrawerFrameView: Apply Filters clicked. [Placeholder] No filter logic in Phase 4.");
        }
    }
}
