namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Immutable display-data for a single founder profile.
    /// </summary>
    public sealed class FounderProfileViewModel
    {
        public string FirstName         { get; }
        public string LastName          { get; }
        public int    Age               { get; }
        public string Nationality       { get; }
        public string Location          { get; }
        public string Background        { get; }
        public string PrimarySkillProfile { get; }

        /// <summary>True when all required fields have been set.</summary>
        public bool IsComplete { get; }

        public FounderProfileViewModel(
            string firstName,
            string lastName,
            int    age,
            string nationality,
            string location,
            string background,
            string primarySkillProfile,
            bool   isComplete)
        {
            FirstName          = firstName          ?? string.Empty;
            LastName           = lastName           ?? string.Empty;
            Age                = age;
            Nationality        = nationality        ?? string.Empty;
            Location           = location           ?? string.Empty;
            Background         = background         ?? string.Empty;
            PrimarySkillProfile = primarySkillProfile ?? string.Empty;
            IsComplete         = isComplete;
        }
    }
}
