using System;
using System.Collections.Generic;
using DiscordRPC;
using Microsoft.UI.Xaml.Controls;
using Vantage.SDK;
using Vantage.SDK.Models;

namespace Vantage.Ext.Discord
{
    public class DiscordPlugin : IVantagePlugin
    {
        public string Name => "Discord RPC";
        public string Author => "Vantage";
        public string Version => "2.0.0";
        public bool HasSettings => true;

        private IHost? _host;
        private DiscordRpcClient? _client;
        private DiscordPluginSettings _settings = new DiscordPluginSettings();
        private const string CLIENT_ID = "811469787657928704";
        private DateTime? _startTime;

        // Whitelist from V3
        private readonly HashSet<string> _validMaps = new HashSet<string>() { 
            "ascent", "bind", "breeze", "fracture", "haven", "icebox", "lotus", "pearl", "split", "sunset", "abyss", 
            "district", "kasbah", "piazza", "drift" 
        };

        public void Initialize(IHost host)
        {
            _host = host;
            _settings = _host.LoadConfig<DiscordPluginSettings>(Name) ?? new DiscordPluginSettings();

            InitializeDiscord();
            
            _host.PresenceUpdated += OnPresenceUpdated;
            _host.Log("Discord Plugin Initialized.");
        }

        private void InitializeDiscord()
        {
            if (_client != null) return;
            
            _client = new DiscordRpcClient(CLIENT_ID);
            _client.Initialize();
            _startTime = DateTime.UtcNow;
        }

        public void Shutdown()
        {
            _host.PresenceUpdated -= OnPresenceUpdated;
            _client?.Dispose();
        }

        public UserControl? GetSettingsView()
        {
            return new DiscordSettingsView(_settings, SaveSettings);
        }

        private void SaveSettings()
        {
            _host.SaveConfig(Name, _settings);
        }

        private void OnPresenceUpdated(object? sender, ValorantPresence presence)
        {
            if (_client == null || !_client.IsInitialized) return;

            if (!presence.IsValid)
            {
                _client.ClearPresence();
                return;
            }

            var (details, state, largeImageKey, smallImageKey) = GetPresenceDetails(presence);

            var rp = new RichPresence()
            {
                Details = details,
                State = state,
                Assets = new Assets()
                {
                    LargeImageKey = largeImageKey,
                    LargeImageText = "Valorant",
                    SmallImageKey = smallImageKey,
                    SmallImageText = smallImageKey
                },
                Timestamps = _startTime.HasValue ? new Timestamps() { Start = _startTime.Value } : null
            };

            // Party Logic
            if (presence.PartySize > 0 && presence.MaxPartySize > 0)
            {
                string syncId = !string.IsNullOrEmpty(presence.PartyCode) ? presence.PartyCode : (presence.PartyId ?? "party");
                
                rp.Party = new Party()
                {
                    ID = syncId, 
                    Size = presence.PartySize,
                    Max = presence.MaxPartySize,
                };

                if (_settings.IsPartyPublic)
                {
                    rp.Secrets = new Secrets() { JoinSecret = "join-" + syncId };
                }
            }

            _client.SetPresence(rp);
        }

        private (string details, string state, string largeImageKey, string smallImageKey) GetPresenceDetails(ValorantPresence presence)
        {
            var tokens = GetTokens(presence);
            
            string details = "";
            string state = "";
            
            switch (presence.SessionLoopState)
            {
                case "MENUS":
                    details = FormatTemplate(_settings.Templates.MenuDetails, tokens);
                    state = FormatTemplate(_settings.Templates.MenuState, tokens);
                    break;
                case "PREGAME":
                    details = FormatTemplate(_settings.Templates.AgentSelectDetails, tokens);
                    state = FormatTemplate(_settings.Templates.AgentSelectState, tokens);
                    break;
                case "INGAME":
                    details = FormatTemplate(_settings.Templates.InGameDetails, tokens);
                    state = FormatTemplate(_settings.Templates.InGameState, tokens);
                    if (presence.IsSpectating)
                    {
                         details = $"Spectating Match on {tokens["{Map}"]}";
                         state = "Watching Player";
                    }
                    break;
                default:
                    details = "Valorant";
                    state = "In Game";
                    break;
            }

            if (string.Equals(presence.ProvisioningFlow, "CustomGame", StringComparison.OrdinalIgnoreCase))
            {
                details = "Custom Match";
                state = "In Game";
            }

            // Asset Logic
            string largeImageKey = "valorant_icon";
            if (tokens.ContainsKey("{Map}"))
            {
               string potentialKey = tokens["{Map}"].ToLower().Replace(" ", "");
               if (_validMaps.Contains(potentialKey)) largeImageKey = potentialKey;
            }

            string smallImageKey = "";
            if (!string.IsNullOrEmpty(presence.CompetitiveTier) && presence.CompetitiveTier != "0")
            {
                smallImageKey = $"rank_{presence.CompetitiveTier}";
            }

            return (details, state, largeImageKey, smallImageKey);
        }

        private string FormatTemplate(string template, Dictionary<string, string> tokens)
        {
            if (string.IsNullOrEmpty(template)) return "";
            string output = template;
            foreach(var kvp in tokens)
            {
                output = output.Replace(kvp.Key, kvp.Value);
            }
            return output;
        }

        private Dictionary<string, string> GetTokens(ValorantPresence presence)
        {
            var dict = new Dictionary<string, string>();
            
            // Map Name Logic
            string mapName = presence.MapId ?? "Unknown";
            if (mapName.Contains("/Game/Maps/", StringComparison.OrdinalIgnoreCase)) 
            {
                var parts = mapName.Split('/');
                mapName = parts[parts.Length - 1];
            }
            if (mapName.Length > 0) mapName = char.ToUpper(mapName[0]) + mapName.Substring(1); 
            
            // Mappings
            if (StringEquals(mapName, "Duality")) mapName = "Bind";
            else if (StringEquals(mapName, "Bonsai")) mapName = "Split";
            else if (StringEquals(mapName, "Ascent")) mapName = "Ascent";
            else if (StringEquals(mapName, "Port")) mapName = "Icebox";
            else if (StringEquals(mapName, "Triad")) mapName = "Haven";
            else if (StringEquals(mapName, "Foxtrot")) mapName = "Breeze";
            else if (StringEquals(mapName, "Canyon")) mapName = "Fracture";
            else if (StringEquals(mapName, "Pitt")) mapName = "Pearl";
            else if (StringEquals(mapName, "Jam")) mapName = "Lotus";
            else if (StringEquals(mapName, "Kasbah")) mapName = "Sunset";
            else if (StringEquals(mapName, "Hurm")) mapName = "Abyss";

            dict["{Map}"] = mapName;
            
            // Queue Logic
            string q = presence.QueueId ?? "";
            if (StringEquals(q, "hurm")) q = "Team Deathmatch";
            else if (StringEquals(q, "valaram")) q = "All Random"; 
            else if (StringEquals(q, "newmap")) q = "Swiftplay";
            else if (StringEquals(q, "ggteam")) q = "Escalation";
            else if (StringEquals(q, "snowball")) q = "Snowball Fight";
            
            if (q.Length > 0) q = char.ToUpper(q[0]) + q.Substring(1);
            dict["{QueueId}"] = q;
            
            dict["{PartySize}"] = presence.PartySize.ToString();
            dict["{PartyMax}"] = presence.MaxPartySize.ToString();
            dict["{PartyState}"] = presence.PartySize > 1 ? "In Party" : "Solo";
            
            dict["{Score}"] = $"{presence.TeamScore} - {presence.EnemyScore}"; 
            dict["{Level}"] = presence.AccountLevel.ToString();

            return dict;
        }

        private bool StringEquals(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
