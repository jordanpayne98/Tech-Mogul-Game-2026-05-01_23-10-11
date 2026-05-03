using System;

namespace Project.Application
{
    /// <summary>
    /// Modal navigation interface. Implemented in the Presentation layer.
    /// Application and Core must not depend on the concrete implementation.
    /// Only one modal may be open at a time.
    /// </summary>
    public interface IModalRouter
    {
        /// <summary>Open a modal by stable ID with optional context data.</summary>
        void OpenModal(string modalId, object context = null);

        /// <summary>Close the currently open modal, if any.</summary>
        void CloseModal();

        /// <summary>Whether a modal is currently displayed.</summary>
        bool IsModalOpen { get; }

        /// <summary>The stable ID of the currently active modal, or null if none.</summary>
        string CurrentModalId { get; }

        /// <summary>Fired when a modal opens. Argument is the modal ID.</summary>
        event Action<string> ModalOpened;

        /// <summary>Fired when a modal closes.</summary>
        event Action ModalClosed;
    }
}
