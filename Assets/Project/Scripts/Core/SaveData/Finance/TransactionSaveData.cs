namespace Project.Core.SaveData.Finance
{
    /// <summary>
    /// Save data mirroring <c>TransactionRecord</c>.
    /// Nullable enum fields stored as their member name string, or null if not applicable.
    /// <c>GameDateTime</c> serialized as total elapsed hours.
    /// </summary>
    public sealed class TransactionSaveData
    {
        public string Id;

        /// <summary>Serialized <c>TransactionType</c> enum member name.</summary>
        public string Type;

        /// <summary>Serialized nullable <c>ExpenseCategory</c> enum member name. Null if this is a revenue transaction.</summary>
        public string ExpenseCategory;

        /// <summary>Serialized nullable <c>RevenueSource</c> enum member name. Null if this is an expense transaction.</summary>
        public string RevenueSource;

        public long AmountMinorUnits;
        public string Description;
        public string RelatedEntityId;
        public string RelatedEntityType;

        /// <summary>Serialized <c>GameDateTime Date</c> as total elapsed hours.</summary>
        public int DateTotalElapsedHours;
    }
}
