using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Vantage.Core.Services;
using Vantage.SDK;

namespace Vantage.Core
{
    public sealed partial class MainWindow : Window
    {
        private readonly ConfigService _configService;
        private readonly CoreHost _coreHost;
        private readonly PluginManager _pluginManager;
        private readonly ValorantConnector _connector;

        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Vantage Core";
            ExtendsContentIntoTitleBar = true;

            // 1. Init Services
            _configService = new ConfigService();
            _coreHost = new CoreHost(_configService);
            
            // 2. Load Plugins
            _pluginManager = new PluginManager(_coreHost);
            _pluginManager.LoadPlugins();

            // 3. Init Connector
            _connector = new ValorantConnector(_coreHost);
            _connector.Start();

            // 4. Build UI
            InitializeNavigation();
        }

        private void InitializeNavigation()
        {
            // Add Home
            MainNav.SelectedItem = MainNav.MenuItems[0];

            // Add Plugin Pages
            foreach (var plugin in _pluginManager.Plugins)
            {
                if (plugin.HasSettings)
                {
                    var item = new NavigationViewItem()
                    {
                        Content = plugin.Name,
                        Icon = new SymbolIcon(Symbol.Setting),
                        Tag = plugin // Store ref
                    };
                    MainNav.MenuItems.Add(item);
                }
            }

            MainNav.SelectionChanged += MainNav_SelectionChanged;
        }

        private void MainNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                if (item.Tag?.ToString() == "Home")
                {
                    // Show a simple status page
                    ContentFrame.Navigate(typeof(Page)); // Empty page for now
                }
                else if (item.Tag is IVantagePlugin plugin)
                {
                    // Inject Plugin UI
                    var view = plugin.GetSettingsView();
                    if (view != null) 
                    {
                        ContentFrame.Content = view;
                    }
                }
            }
        }
    }
}
