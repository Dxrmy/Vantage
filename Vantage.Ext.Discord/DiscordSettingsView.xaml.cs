using Microsoft.UI.Xaml.Controls;
using System;

namespace Vantage.Ext.Discord
{
    public sealed partial class DiscordSettingsView : UserControl
    {
        public DiscordPluginSettings ViewModel { get; set; }
        private Action _saveAction;

        public DiscordSettingsView(DiscordPluginSettings settings, Action saveAction)
        {
            this.InitializeComponent();
            ViewModel = settings;
            _saveAction = saveAction;
        }

        private void SaveBtn_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _saveAction?.Invoke();
        }
    }
}
