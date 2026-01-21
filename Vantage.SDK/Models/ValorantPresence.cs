using System.Collections.Generic;

namespace Vantage.SDK.Models
{
    public class ValorantPresence
    {
        public bool IsValid { get; set; } = false;
        public string SessionLoopState { get; set; } = string.Empty;
        public string PartyId { get; set; } = string.Empty;
        public int PartySize { get; set; }
        public int MaxPartySize { get; set; }
        public string QueueId { get; set; } = string.Empty;
        public int AccountLevel { get; set; }
        public string CompetitiveTier { get; set; } = "0";
        public string LeaderboardPosition { get; set; } = string.Empty;
        public string PlayerCardId { get; set; } = string.Empty;
        public string PlayerTitleId { get; set; } = string.Empty;
        public string PartyCode { get; set; } = string.Empty;
        public bool IsPartyOwner { get; set; }
        
        // Match Extensions
        public string MatchId { get; set; } = string.Empty;
        public string MapId { get; set; } = string.Empty;
        public int TeamScore { get; set; }
        public int EnemyScore { get; set; }
        public string ProvisioningFlow { get; set; } = string.Empty;

        // Power Fields
        public string EquippedSkinId { get; set; } = string.Empty;
        public bool IsSpectating { get; set; }
        public string SpectatingSubject { get; set; } = string.Empty;
        public string PregameMatchId { get; set; } = string.Empty;
    }
}
