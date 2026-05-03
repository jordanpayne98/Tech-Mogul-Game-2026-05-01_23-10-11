using System;
using System.Collections.Generic;
using Project.Core.Definitions.Report;
using Project.Core.Runtime.Report;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Report;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Report domain runtime types to and from their save data equivalents.
    /// Covers ReportProfile and InboxRuntimeState.
    /// Report read/inbox states from InboxRuntimeState are denormalized onto ReportSaveData.
    /// All methods are static — this mapper holds no state.
    /// </summary>
    public static class ReportSaveMapper
    {
        // ─── ReportProfile ────────────────────────────────────────────────────────

        /// <summary>
        /// Converts a ReportProfile to ReportSaveData, including read/inbox state from InboxRuntimeState.
        /// </summary>
        public static ReportSaveData ToSaveData(ReportProfile profile, InboxRuntimeState inbox)
        {
            var relatedEntities = new List<ReportEntityReferenceSaveData>(profile.RelatedEntities.Count);
            foreach (ReportEntityReference entity in profile.RelatedEntities)
            {
                relatedEntities.Add(new ReportEntityReferenceSaveData
                {
                    EntityId   = entity.EntityId,
                    EntityType = entity.EntityType
                });
            }

            var keyValues = new List<ReportKeyValueSaveData>(profile.KeyValues.Count);
            foreach (ReportKeyValue kv in profile.KeyValues)
            {
                keyValues.Add(new ReportKeyValueSaveData
                {
                    Label         = kv.Label,
                    Value         = kv.Value,
                    PreviousValue = kv.PreviousValue,
                    ChangeText    = kv.ChangeText
                });
            }

            // Denormalize read and inbox state from the inbox onto the save record.
            inbox.ReportReadStates.TryGetValue(profile.Id, out ReportReadState readState);
            inbox.ReportInboxStates.TryGetValue(profile.Id, out ReportInboxState inboxState);

            // IsArchived is derived from the inbox state.
            bool isArchived = inboxState == ReportInboxState.Archived;

            return new ReportSaveData
            {
                Id                  = profile.Id,
                Type                = profile.Type.ToString(),
                Category            = profile.Category.ToString(),
                Priority            = profile.Priority.ToString(),
                Title               = profile.Title,
                Summary             = profile.Summary,
                DateTotalElapsedHours = profile.Date.TotalElapsedHours,
                RelatedEntities     = relatedEntities,
                KeyValues           = keyValues,
                RequiresDecision    = profile.RequiresDecision,
                AvailableActionIds  = profile.AvailableActionIds,
                ReadState           = readState.ToString(),
                InboxState          = inboxState.ToString(),
                IsArchived          = isArchived
            };
        }

        public static ReportProfile FromSaveData(ReportSaveData data)
        {
            var relatedEntities = new List<ReportEntityReference>(data.RelatedEntities.Count);
            foreach (ReportEntityReferenceSaveData entity in data.RelatedEntities)
            {
                relatedEntities.Add(new ReportEntityReference
                {
                    EntityId   = entity.EntityId,
                    EntityType = entity.EntityType
                });
            }

            var keyValues = new List<ReportKeyValue>(data.KeyValues.Count);
            foreach (ReportKeyValueSaveData kv in data.KeyValues)
            {
                keyValues.Add(new ReportKeyValue
                {
                    Label         = kv.Label,
                    Value         = kv.Value,
                    PreviousValue = kv.PreviousValue,
                    ChangeText    = kv.ChangeText
                });
            }

            return new ReportProfile
            {
                Id                 = data.Id,
                Type               = Enum.Parse<ReportType>(data.Type),
                Category           = Enum.Parse<ReportCategory>(data.Category),
                Priority           = Enum.Parse<ReportPriority>(data.Priority),
                Title              = data.Title,
                Summary            = data.Summary,
                Date               = GameDateTime.FromTotalHours(data.DateTotalElapsedHours),
                RelatedEntities    = relatedEntities,
                KeyValues          = keyValues,
                RequiresDecision   = data.RequiresDecision,
                AvailableActionIds = data.AvailableActionIds
            };
        }

        // ─── InboxRuntimeState ────────────────────────────────────────────────────

        public static InboxSaveData ToSaveData(InboxRuntimeState state)
        {
            var readStates = new Dictionary<string, string>(state.ReportReadStates.Count);
            foreach (KeyValuePair<string, ReportReadState> pair in state.ReportReadStates)
            {
                readStates[pair.Key] = pair.Value.ToString();
            }

            var inboxStates = new Dictionary<string, string>(state.ReportInboxStates.Count);
            foreach (KeyValuePair<string, ReportInboxState> pair in state.ReportInboxStates)
            {
                inboxStates[pair.Key] = pair.Value.ToString();
            }

            return new InboxSaveData
            {
                CompanyId          = state.CompanyId,
                ReportIds          = state.ReportIds,
                ReportReadStates   = readStates,
                ReportInboxStates  = inboxStates,
                UnreadCount        = state.UnreadCount,
                DecisionRequiredCount = state.DecisionRequiredCount
            };
        }

        public static InboxRuntimeState FromSaveData(InboxSaveData data, List<ReportSaveData> reportSaveData)
        {
            var readStates = new Dictionary<string, ReportReadState>(data.ReportReadStates.Count);
            foreach (KeyValuePair<string, string> pair in data.ReportReadStates)
            {
                readStates[pair.Key] = Enum.Parse<ReportReadState>(pair.Value);
            }

            var inboxStates = new Dictionary<string, ReportInboxState>(data.ReportInboxStates.Count);
            foreach (KeyValuePair<string, string> pair in data.ReportInboxStates)
            {
                inboxStates[pair.Key] = Enum.Parse<ReportInboxState>(pair.Value);
            }

            return new InboxRuntimeState
            {
                CompanyId             = data.CompanyId,
                ReportIds             = data.ReportIds,
                ReportReadStates      = readStates,
                ReportInboxStates     = inboxStates,
                UnreadCount           = data.UnreadCount,
                DecisionRequiredCount = data.DecisionRequiredCount
            };
        }
    }
}
