using Project.Core.Runtime.Time;

namespace Project.Core.Events.Finance
{
    /// <summary>
    /// Published when the monthly payroll has been processed and cash has been deducted.
    /// Subscribers can use this to update UI projections or log payroll history.
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public sealed class PayrollProcessedEvent
    {
        /// <summary>Stable ID of the company whose payroll was processed.</summary>
        public string CompanyId { get; }

        /// <summary>Total payroll amount deducted in minor currency units.</summary>
        public long TotalPayrollMinorUnits { get; }

        /// <summary>The in-game date on which payroll was processed.</summary>
        public GameDateTime PayrollDate { get; }

        /// <summary>
        /// Creates a new PayrollProcessedEvent.
        /// </summary>
        public PayrollProcessedEvent(string companyId, long totalPayrollMinorUnits, GameDateTime payrollDate)
        {
            CompanyId              = companyId;
            TotalPayrollMinorUnits = totalPayrollMinorUnits;
            PayrollDate            = payrollDate;
        }
    }
}
