using Project.Core.Debugging;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Shell
{
    /// <summary>
    /// Fixes the UIDocument panel layout so the root UXML content fills the full viewport.
    /// Unity's UIDocument wraps cloned VisualTreeAsset content in a TemplateContainer
    /// that does not stretch by default. This MonoBehaviour sets flex-grow on both
    /// rootVisualElement and the TemplateContainer so .main-shell can fill the panel.
    ///
    /// The TemplateContainer fix is deferred via schedule.Execute() to ensure UIDocument
    /// has fully cloned the UXML tree before we access child elements.
    ///
    /// Attach to the same GameObject as the UIDocument (UIShell).
    /// Phase 4B structural fix only — no routing, no gameplay logic.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class UIShellLayoutFixer : MonoBehaviour
    {
        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "UIShellLayoutFixer — UIDocument component not found.", this);
                return;
            }

            var root = uiDocument.rootVisualElement;
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "UIShellLayoutFixer — rootVisualElement is null.", this);
                return;
            }

            // Ensure rootVisualElement fills the panel.
            root.style.flexGrow = 1;

            // Apply TemplateContainer fix immediately if the tree is already populated.
            if (root.childCount > 0)
            {
                ApplyTemplateContainerFix(root);
            }
            else
            {
                // Defer to next frame to ensure UIDocument has cloned the UXML tree.
                root.schedule.Execute(() => ApplyTemplateContainerFix(root));
            }
        }

        private void ApplyTemplateContainerFix(VisualElement root)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                root[i].style.flexGrow = 1;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"UIShellLayoutFixer — applied flexGrow to rootVisualElement and " +
                $"{root.childCount} child container(s).", this);
        }
    }
}
