using System.Collections.Generic;

namespace Vantage.SDK.Models
{
    public class MatchPlayer
    {
        public string Subject { get; set; } = string.Empty; // PUUID
        public string GameName { get; set; } = string.Empty;
        public string TagLine { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string CharacterId { get; set; } = string.Empty;
        public string CompetitiveTier { get; set; } = "0";
        public int RankedRating { get; set; }
        public int LeaderboardRank { get; set; }
        public int AccountLevel { get; set; }
        
        // Loadout
        public string VandalSkinId { get; set; } = string.Empty;
        public string PhantomSkinId { get; set; } = string.Empty;
        
        public bool IsSelf { get; set; }
        
        // Logic moved to SDK so all plugins agree on definition
        public bool IsSmurfSuspect 
        {
            get 
            {
                // Simple heuristic: Low Level (<30) but High Rank (Diamond+)
                // Tier 21 = Diamond 1, 24 = Radiant
                if (AccountLevel < 30 && int.TryParse(CompetitiveTier, out int tier))
                {
                    return tier >= 21;
                }
                return false;
            }
        }
    }

    public class LiveMatchData
    {
        public string MatchId { get; set; } = string.Empty;
        public List<MatchPlayer> Players { get; set; } = new List<MatchPlayer>();
        public double Team1AvgTier { get; set; }
        public double Team2AvgTier { get; set; }
        public string MapId { get; set; } = string.Empty;
    }
}
