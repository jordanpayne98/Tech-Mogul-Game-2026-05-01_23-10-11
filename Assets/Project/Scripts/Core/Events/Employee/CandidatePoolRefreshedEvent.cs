namespace Project.Core.Events.Employee
{
    /// <summary>
    /// Published after the recruitment candidate pool is refreshed (initial generation or monthly top-up).
    /// </summary>
    public sealed class CandidatePoolRefreshedEvent
    {
        /// <summary>Number of new candidates added during this refresh.</summary>
        public int NewCandidateCount { get; }

        public CandidatePoolRefreshedEvent(int newCandidateCount)
        {
            NewCandidateCount = newCandidateCount;
        }
    }
}
