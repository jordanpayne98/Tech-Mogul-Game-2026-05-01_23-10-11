using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Settings
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Settings screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates category sidebar tabs and setting rows with appropriate controls.
    /// Exposes events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core settings wiring deferred to Phase 6+.
    /// </summary>
    public sealed class SettingsView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from SettingsScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Events ──────────────────────────────────────────────────────────────────

        /// <summary>Fired when a category tab is clicked. Argument is the category's stable ID.</summary>
        public event Action<string> OnCategorySelected;

        /// <summary>
        /// Fired when a setting control value changes.
        /// Argument 1 is the setting's stable ID. Argument 2 is the new string value.
        /// </summary>
        public event Action<string, string> OnSettingChanged;

        /// <summary>Fired when the Apply button is clicked.</summary>
        public event Action OnApplyClicked;

        /// <summary>Fired when the Reset to Defaults button is clicked.</summary>
        public event Action OnResetClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Category sidebar ─────────────────────────────────────────────────────────

        private readonly VisualElement _categoryList;

        // ─── Settings panel ───────────────────────────────────────────────────────────

        private readonly Label         _panelTitle;
        private readonly VisualElement _settingRowsList;

        // ─── Footer ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _unsavedChangesIndicator;
        private readonly Button        _applyButton;
        private readonly Button        _resetButton;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public SettingsView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "SettingsView: root VisualElement is null. View cannot be initialized.");

                // Provide a non-null fallback so callers can safely reference Root without crashing.
                Root = new VisualElement();
                return;
            }

            Root = root;

            // ── State containers ─────────────────────────────────────────────────────

            _contentContainer = QueryElement(root, "ContentContainer");
            _loadingState     = QueryElement(root, "LoadingState");
            _errorState       = QueryElement(root, "ErrorState");
            _emptyState       = QueryElement(root, "EmptyState");

            // ── Header ───────────────────────────────────────────────────────────────

            _headerTitle    = root.Q<Label>("HeaderTitle");
            _headerSubtitle = root.Q<Label>("HeaderSubtitle");

            LogIfNull(_headerTitle,    "HeaderTitle");
            LogIfNull(_headerSubtitle, "HeaderSubtitle");

            // ── Category sidebar ─────────────────────────────────────────────────────

            _categoryList = QueryElement(root, "CategoryList");

            // ── Settings panel ───────────────────────────────────────────────────────

            _panelTitle      = root.Q<Label>("PanelTitle");
            _settingRowsList = QueryElement(root, "SettingRowsList");

            LogIfNull(_panelTitle, "PanelTitle");

            // ── Footer ───────────────────────────────────────────────────────────────

            _unsavedChangesIndicator = QueryElement(root, "UnsavedChangesIndicator");

            _applyButton = root.Q<Button>("ApplyButton");
            _resetButton = root.Q<Button>("ResetButton");

            LogIfNull(_applyButton, "ApplyButton");
            LogIfNull(_resetButton, "ResetButton");

            // ── Error / empty text labels ────────────────────────────────────────────

            _errorMessage    = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_errorMessage,    "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");

            // ── Footer button callbacks ───────────────────────────────────────────────

            if (_applyButton != null)
            {
                _applyButton.clicked += () => OnApplyClicked?.Invoke();
            }

            if (_resetButton != null)
            {
                _resetButton.clicked += () => OnResetClicked?.Invoke();
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds the category sidebar and setting rows on each call.
        /// </summary>
        public void Bind(SettingsScreenViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "SettingsView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No data available.");
                return;
            }

            // ── Loading state ────────────────────────────────────────────────────────

            if (viewModel.IsLoading)
            {
                SetVisible(_loadingState,     true);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       false);
                return;
            }

            // ── Error state ──────────────────────────────────────────────────────────

            if (viewModel.HasError)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            // ── Empty state ──────────────────────────────────────────────────────────

            bool hasCategories = viewModel.Categories != null && viewModel.Categories.Count > 0;

            if (!hasCategories)
            {
                SetVisible(_loadingState,     false);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       true);

                if (_emptyStateTitle != null)
                {
                    _emptyStateTitle.text = viewModel.EmptyStateTitle;
                }

                if (_emptyStateBody != null)
                {
                    _emptyStateBody.text = viewModel.EmptyStateBody;
                }

                return;
            }

            // ── Normal content state ─────────────────────────────────────────────────

            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, true);
            SetVisible(_errorState,       false);
            SetVisible(_emptyState,       false);

            // Header
            if (_headerTitle != null)
            {
                _headerTitle.text = viewModel.ScreenTitle;
            }

            if (_headerSubtitle != null)
            {
                _headerSubtitle.text = viewModel.ScreenSubtitle;
            }

            // Category sidebar
            BuildCategorySidebar(viewModel);

            // Active category panel title and rows
            BuildSettingRows(viewModel);

            // Footer state
            ApplyFooterState(viewModel);
        }

        // ─── Private — category sidebar ───────────────────────────────────────────────

        private void BuildCategorySidebar(SettingsScreenViewModel viewModel)
        {
            if (_categoryList == null)
            {
                return;
            }

            _categoryList.Clear();

            if (viewModel.Categories == null)
            {
                return;
            }

            foreach (SettingsCategoryViewModel category in viewModel.Categories)
            {
                VisualElement tabEl = CreateCategoryTab(category);
                _categoryList.Add(tabEl);
            }
        }

        private VisualElement CreateCategoryTab(SettingsCategoryViewModel category)
        {
            var tab = new VisualElement();
            tab.AddToClassList("settings__category-tab");
            tab.AddToClassList("text-body");

            ApplySemanticStateClass(tab, category.SemanticState, "settings__category-tab");

            var label = new Label(category.Label);
            label.AddToClassList("settings__category-tab__label");
            label.AddToClassList("text-body");
            tab.Add(label);

            // Append [Placeholder] badge for Audio and Controls
            if (category.Id == SettingsCategoryIds.Audio || category.Id == SettingsCategoryIds.Controls)
            {
                var badge = new Label("[P]");
                badge.AddToClassList("settings__category-tab__placeholder-badge");
                tab.Add(badge);
            }

            if (category.SemanticState != "disabled")
            {
                string categoryId = category.Id;
                tab.RegisterCallback<ClickEvent>(_ => OnCategorySelected?.Invoke(categoryId));
            }
            else
            {
                tab.AddToClassList("is-disabled");
            }

            return tab;
        }

        // ─── Private — setting rows ────────────────────────────────────────────────────

        private void BuildSettingRows(SettingsScreenViewModel viewModel)
        {
            if (_settingRowsList == null)
            {
                return;
            }

            _settingRowsList.Clear();

            // Update panel title to active category label
            if (_panelTitle != null && viewModel.Categories != null)
            {
                string panelLabel = string.Empty;
                foreach (SettingsCategoryViewModel cat in viewModel.Categories)
                {
                    if (cat.Id == viewModel.ActiveCategoryId)
                    {
                        panelLabel = cat.Label;
                        break;
                    }
                }

                _panelTitle.text = panelLabel;
            }

            if (viewModel.CurrentCategoryRows == null)
            {
                return;
            }

            foreach (SettingRowViewModel row in viewModel.CurrentCategoryRows)
            {
                VisualElement rowEl = CreateSettingRow(row);
                _settingRowsList.Add(rowEl);
            }
        }

        private VisualElement CreateSettingRow(SettingRowViewModel row)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("settings__row");

            ApplySemanticStateClass(rowEl, row.SemanticState, "settings__row");

            if (!row.IsEnabled)
            {
                rowEl.AddToClassList("is-disabled");
            }

            // ── Label block ──────────────────────────────────────────────────────────

            var labelBlock = new VisualElement();
            labelBlock.AddToClassList("settings__row__label-block");

            var labelEl = new Label(row.Label);
            labelEl.AddToClassList("settings__row__label");
            labelEl.AddToClassList("text-body");
            labelBlock.Add(labelEl);

            if (!string.IsNullOrEmpty(row.Description))
            {
                var descEl = new Label(row.Description);
                descEl.AddToClassList("settings__row__description");
                descEl.AddToClassList("text-small");
                labelBlock.Add(descEl);
            }

            if (row.IsPlaceholder)
            {
                var placeholderTag = new Label("[Placeholder]");
                placeholderTag.AddToClassList("settings__row__placeholder-tag");
                placeholderTag.AddToClassList("text-tiny");
                labelBlock.Add(placeholderTag);
            }

            rowEl.Add(labelBlock);

            // ── Control ───────────────────────────────────────────────────────────────

            VisualElement controlEl = CreateControl(row);
            controlEl.AddToClassList("settings__row__control");
            rowEl.Add(controlEl);

            return rowEl;
        }

        /// <summary>
        /// Creates the appropriate UI control for the given control type.
        /// Emits OnSettingChanged when the control value changes.
        /// </summary>
        private VisualElement CreateControl(SettingRowViewModel row)
        {
            string settingId = row.Id;

            switch (row.ControlType)
            {
                case SettingControlTypes.Toggle:
                {
                    var toggle = new Toggle();
                    toggle.SetValueWithoutNotify(row.CurrentValue == "true" || row.CurrentValue == "On");
                    toggle.SetEnabled(row.IsEnabled);
                    toggle.RegisterValueChangedCallback(evt =>
                    {
                        string newVal = evt.newValue ? "true" : "false";
                        OnSettingChanged?.Invoke(settingId, newVal);
                    });
                    return toggle;
                }

                case SettingControlTypes.Dropdown:
                {
                    var dropdown = new DropdownField();
                    dropdown.label = string.Empty;

                    if (row.Options != null)
                    {
                        dropdown.choices = new List<string>(row.Options);
                    }

                    dropdown.SetValueWithoutNotify(row.CurrentValue);
                    dropdown.SetEnabled(row.IsEnabled);
                    dropdown.RegisterValueChangedCallback(evt =>
                    {
                        OnSettingChanged?.Invoke(settingId, evt.newValue ?? string.Empty);
                    });
                    return dropdown;
                }

                case SettingControlTypes.Slider:
                {
                    var sliderRow = new VisualElement();
                    sliderRow.style.flexDirection = FlexDirection.Row;
                    sliderRow.style.alignItems    = Align.Center;

                    var slider = new Slider(0f, 100f);
                    slider.SetEnabled(row.IsEnabled);

                    if (float.TryParse(row.CurrentValue, out float parsedValue))
                    {
                        slider.SetValueWithoutNotify(parsedValue);
                    }

                    var valueLabel = new Label(row.CurrentValue);
                    valueLabel.AddToClassList("text-small");
                    valueLabel.style.minWidth = 36;
                    valueLabel.style.unityTextAlign = UnityEngine.TextAnchor.MiddleRight;

                    slider.RegisterValueChangedCallback(evt =>
                    {
                        string newVal = evt.newValue.ToString("F0");
                        valueLabel.text = newVal + "%";
                        OnSettingChanged?.Invoke(settingId, newVal);
                    });

                    sliderRow.Add(slider);
                    sliderRow.Add(valueLabel);
                    return sliderRow;
                }

                case SettingControlTypes.Button:
                {
                    var btn = new Button(() => OnSettingChanged?.Invoke(settingId, "clicked"));
                    btn.text = string.IsNullOrEmpty(row.CurrentValue) ? "Open" : row.CurrentValue;
                    btn.SetEnabled(row.IsEnabled);
                    return btn;
                }

                default:
                {
                    // Unknown control type — render a read-only label as fallback.
                    var fallback = new Label(row.CurrentValue);
                    fallback.AddToClassList("text-small");
                    DebugLogger.LogWarning(DebugCategory.UI,
                        $"SettingsView: unknown ControlType '{row.ControlType}' for setting '{row.Id}'. " +
                        "Rendering read-only label as fallback.");
                    return fallback;
                }
            }
        }

        // ─── Private — footer state ───────────────────────────────────────────────────

        private void ApplyFooterState(SettingsScreenViewModel viewModel)
        {
            SetVisible(_unsavedChangesIndicator, viewModel.HasUnsavedChanges);

            if (_applyButton != null)
            {
                _applyButton.SetEnabled(viewModel.CanApply);
            }

            if (_resetButton != null)
            {
                _resetButton.SetEnabled(viewModel.CanReset);
            }
        }

        // ─── Private — error helper ───────────────────────────────────────────────────

        private void ShowError(string message)
        {
            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, false);
            SetVisible(_errorState,       true);
            SetVisible(_emptyState,       false);

            if (_errorMessage != null)
            {
                _errorMessage.text = message;
            }
        }

        // ─── Private — element helpers ────────────────────────────────────────────────

        private static VisualElement QueryElement(VisualElement root, string name)
        {
            VisualElement el = root.Q(name);
            if (el == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"SettingsView: element '{name}' not found in UXML. " +
                    "Check SettingsScreen.uxml element names.");
            }

            return el;
        }

        private static void LogIfNull(VisualElement element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"SettingsView: element '{name}' not found in UXML.");
            }
        }

        private static void SetVisible(VisualElement element, bool visible)
        {
            if (element == null)
            {
                return;
            }

            if (visible)
            {
                element.RemoveFromClassList("is-hidden");
            }
            else
            {
                element.AddToClassList("is-hidden");
            }
        }

        /// <summary>
        /// Applies semantic state classes to a VisualElement using the project's USS state conventions.
        /// SemanticState "active" → is-active; "disabled" → is-disabled; "warning" → has-warning; "danger" → has-error.
        /// </summary>
        private static void ApplySemanticStateClass(VisualElement element, string semanticState, string prefix = null)
        {
            if (string.IsNullOrEmpty(semanticState) || semanticState == "normal")
            {
                return;
            }

            switch (semanticState)
            {
                case "active":
                    element.AddToClassList("is-active");
                    break;
                case "disabled":
                    element.AddToClassList("is-disabled");
                    break;
                case "warning":
                    element.AddToClassList("has-warning");
                    break;
                case "danger":
                    element.AddToClassList("has-error");
                    break;
            }
        }
    }
}
