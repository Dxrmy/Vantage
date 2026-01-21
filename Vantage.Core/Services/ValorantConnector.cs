using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Vantage.SDK.Models;

namespace Vantage.Core.Services
{
    public class ValorantConnector
    {
        private readonly CoreHost _host;
        private HttpClient? _client;
        private string? _puuid;
        private CancellationTokenSource? _cts;

        public bool IsConnected { get; private set; }

        public ValorantConnector(CoreHost host)
        {
            _host = host;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(Loop);
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private async Task Loop()
        {
            _host.Log("Starting Valorant Connector Loop...");
            while (_cts != null && !_cts.IsCancellationRequested)
            {
                try
                {
                    if (!IsRiotClientRunning())
                    {
                        // _host.Log("Waiting for Riot Client...");
                        await Task.Delay(5000);
                        continue;
                    }

                    if (_client == null || !IsConnected)
                    {
                        if (await InitializeAsync())
                        {
                            _host.Log("Connected to Riot Client!");
                        }
                        else
                        {
                            await Task.Delay(2000);
                            continue;
                        }
                    }

                    // Poll Presence
                    await FetchPresence();
                    
                    // Poll rate
                    await Task.Delay(2000); // 2s polling
                }
                catch (Exception ex)
                {
                    _host.Log($"Connector Error: {ex.Message}");
                    await Task.Delay(5000);
                }
            }
        }

        private bool IsRiotClientRunning() => Process.GetProcessesByName("RiotClientServices").Length > 0;

        private async Task<bool> InitializeAsync()
        {
            string lockfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games\\Riot Client\\Config\\lockfile");
            if (!File.Exists(lockfilePath)) return false;

            try
            {
                using (var fs = new FileStream(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    string content = await sr.ReadToEndAsync();
                    string[] parts = content.Split(':');
                    if (parts.Length < 5) return false;

                    string port = parts[2];
                    string password = parts[3];
                    string protocol = parts[4];

                    var handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (m, c, ch, p) => true;

                    _client = new HttpClient(handler);
                    _client.BaseAddress = new Uri($"{protocol}://127.0.0.1:{port}/");
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{password}")));
                    
                    // Get PUUID to verify
                    var resp = await _client.GetAsync("riotclient/v1/sessions");
                    if (resp.IsSuccessStatusCode)
                    {
                        // Simplified parsing for session to get PUUID if needed
                        // For now just assuming success
                        IsConnected = true;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private async Task FetchPresence()
        {
            if (_client == null) return;
            var response = await _client.GetAsync("chat/v4/presences");
            if (!response.IsSuccessStatusCode) 
            {
                IsConnected = false; 
                return; 
            }

            string json = await response.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<PresenceResponseRoot>(json);
            
            // Just take the first valid presence that isn't offline, ideally we filter by our PUUID if we had it
            // For now, grabbing the first one that has "valorant" info is a decent heuristic for prototype
            var myPresence = root?.presences?.FirstOrDefault(p => !string.IsNullOrEmpty(p.@private));

            if (myPresence != null)
            {
                try 
                {
                   // Decode Base64 private data
                   byte[] data = Convert.FromBase64String(myPresence.@private);
                   string decoded = Encoding.UTF8.GetString(data);
                   var presenceData = JsonConvert.DeserializeObject<ValorantPresence>(decoded);
                   
                   if (presenceData != null)
                   {
                       presenceData.IsValid = true;
                       _host.DispatchPresence(presenceData);
                   }
                }
                catch {}
            }
        }

        // Internal Models for Parsing
        private class PresenceResponseRoot
        {
            public System.Collections.Generic.List<PresenceItem>? presences { get; set; }
        }
        private class PresenceItem
        {
            public string? puuid { get; set; }
            public string? @private { get; set; }
        }
    }
}
