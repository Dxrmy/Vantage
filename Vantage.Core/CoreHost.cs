using System;
using Vantage.SDK;
using Vantage.SDK.Models;
using Vantage.Core.Services;
using System.Diagnostics;

namespace Vantage.Core
{
    public class CoreHost : IHost
    {
        private readonly ConfigService _configService;

        public CoreHost(ConfigService configService)
        {
            _configService = configService;
        }

        public void Log(string message)
        {
            Debug.WriteLine($"[Vantage] {message}");
            // Future: File Logging or In-App Console
        }

        public T? LoadConfig<T>(string pluginOffset) where T : new()
        {
            return _configService.Load<T>(pluginOffset);
        }

        public void SaveConfig<T>(string pluginOffset, T data)
        {
            _configService.Save(pluginOffset, data);
        }

        public event EventHandler<ValorantPresence>? PresenceUpdated;
        public event EventHandler<LiveMatchData>? MatchUpdated;

        // Internal triggers
        public void DispatchPresence(ValorantPresence p) => PresenceUpdated?.Invoke(this, p);
        public void DispatchMatch(LiveMatchData m) => MatchUpdated?.Invoke(this, m);
    }
}
