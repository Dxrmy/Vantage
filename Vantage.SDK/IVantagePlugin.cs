using Microsoft.UI.Xaml.Controls;

namespace Vantage.SDK
{
    public interface IVantagePlugin
    {
        string Name { get; }
        string Author { get; }
        string Version { get; }
        
        void Initialize(IHost host);
        void Shutdown();
        
        // UI Injection
        bool HasSettings { get; }
        UserControl? GetSettingsView(); 
    }
}
