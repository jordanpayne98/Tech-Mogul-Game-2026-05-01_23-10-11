using System.Collections.Generic;

namespace Project.Core.SaveData.Report
{
    /// <summary>
    /// Save data mirroring <c>ReportProfile</c> including inbox state fields.
    /// Enum fields stored as member name strings. <c>GameDateTime</c> serialized as total elapsed hours.
    /// Includes two inline helper types: <see cref="ReportEntityReferenceSaveData"/> and <see cref="ReportKeyValueSaveData"/>.
    /// </summary>
    public sealed class ReportSaveData
    {
        public string Id;

        /// <summary>Serialized <c>ReportType</c> enum member name.</summary>
        public string Type;

        /// <summary>Serialized <c>ReportCategory</c> enum member name.</summary>
        public string Category;

        /// <summary>Serialized <c>ReportPriority</c> enum member name.</summary>
        public string Priority;

        public string Title;
        public string Summary;

        /// <summary>Serialized <c>GameDateTime Date</c> as total elapsed hours.</summary>
        public int DateTotalElapsedHours;

        public List<ReportEntityReferenceSaveData> RelatedEntities;
        public List<ReportKeyValueSaveData> KeyValues;
        public bool RequiresDecision;
        public List<string> AvailableActionIds;

        /// <summary>Serialized <c>ReportReadState</c> enum member name.</summary>
        public string ReadState;

        /// <summary>Serialized <c>ReportInboxState</c> enum member name.</summary>
        public string InboxState;

        public bool IsArchived;
    }

    /// <summary>
    /// Inline helper — save data mirroring <c>ReportEntityReference</c>.
    /// </summary>
    public sealed class ReportEntityReferenceSaveData
    {
        public string EntityId;
        public string EntityType;
    }

    /// <summary>
    /// Inline helper — save data mirroring <c>ReportKeyValue</c>.
    /// </summary>
    public sealed class ReportKeyValueSaveData
    {
        public string Label;
        public string Value;
        public string PreviousValue;
        public string ChangeText;
    }
}
