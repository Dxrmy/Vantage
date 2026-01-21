using System;
using Vantage.SDK.Models;

namespace Vantage.SDK
{
    public interface IHost
    {
        void Log(string message);
        T? LoadConfig<T>(string pluginOffset) where T : new();
        void SaveConfig<T>(string pluginOffset, T data);
        
        // Events
        event EventHandler<ValorantPresence> PresenceUpdated;
        event EventHandler<LiveMatchData> MatchUpdated;
    }
}
