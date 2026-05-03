namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Immutable display-data for a single stepper step in the Company Creation wizard.
    /// </summary>
    public sealed class CompanyCreationStepViewModel
    {
        public CompanyCreationStepId    StepId            { get; }
        public int                      StepNumber        { get; }
        public string                   DisplayName       { get; }
        public CompanyCreationStepState State             { get; }
        public bool                     IsClickable       { get; }
        public string                   ValidationMessage { get; }

        public CompanyCreationStepViewModel(
            CompanyCreationStepId    stepId,
            int                      stepNumber,
            string                   displayName,
            CompanyCreationStepState state,
            bool                     isClickable,
            string                   validationMessage)
        {
            StepId            = stepId;
            StepNumber        = stepNumber;
            DisplayName       = displayName       ?? string.Empty;
            State             = state;
            IsClickable       = isClickable;
            ValidationMessage = validationMessage ?? string.Empty;
        }
    }
}
