namespace Vantage.Ext.Discord
{
    public class DiscordPluginSettings
    {
        public bool IsPartyPublic { get; set; } = true;
        
        public PresenceTemplates Templates { get; set; } = new PresenceTemplates();

        public class PresenceTemplates
        {
            public string MenuDetails { get; set; } = "In Menus";
            public string MenuState { get; set; } = "{PartyState}";
            
            public string AgentSelectDetails { get; set; } = "Selecting Agent";
            public string AgentSelectState { get; set; } = "{Map}";
            
            public string InGameDetails { get; set; } = "{QueueId} - {Map}";
            public string InGameState { get; set; } = "{Score}";
        }
    }
}
