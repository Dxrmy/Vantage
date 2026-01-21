using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Vantage.SDK;

namespace Vantage.Core.Services
{
    public class PluginManager
    {
        private readonly IHost _host;
        public List<IVantagePlugin> Plugins { get; private set; } = new List<IVantagePlugin>();

        public PluginManager(IHost host)
        {
            _host = host;
        }

        public void LoadPlugins()
        {
            string pluginsDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            if (!Directory.Exists(pluginsDir))
            {
                Directory.CreateDirectory(pluginsDir);
            }

            // Also check the base directory for testing (dev modules sometimes output there)
            LoadPluginsFromPath(pluginsDir);
        }

        private void LoadPluginsFromPath(string path) 
        {
            var dlls = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var dll in dlls)
            {
                try
                {
                    LoadPlugin(dll);
                }
                catch (Exception ex)
                {
                    _host.Log($"Failed to load DLL {Path.GetFileName(dll)}: {ex.Message}");
                }
            }
        }

        private void LoadPlugin(string dllPath)
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IVantagePlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in pluginTypes)
            {
                try
                {
                    var plugin = Activator.CreateInstance(type) as IVantagePlugin;
                    if (plugin != null)
                    {
                        Plugins.Add(plugin);
                        plugin.Initialize(_host);
                        _host.Log($"Loaded Plugin: {plugin.Name} by {plugin.Author}");
                    }
                }
                catch (Exception ex)
                {
                    _host.Log($"Error activating plugin type {type.Name}: {ex.Message}");
                }
            }
        }

        public void Shutdown()
        {
            foreach (var plugin in Plugins)
            {
                try { plugin.Shutdown(); } catch { }
            }
        }
    }
}
