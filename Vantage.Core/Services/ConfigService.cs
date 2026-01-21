using Newtonsoft.Json;
using System.IO;

namespace Vantage.Core.Services
{
    public class ConfigService
    {
        private readonly string _configDir;

        public ConfigService()
        {
            _configDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config");
            if (!Directory.Exists(_configDir))
            {
                Directory.CreateDirectory(_configDir);
            }
        }

        public T? Load<T>(string pluginName) where T : new()
        {
            string path = Path.Combine(_configDir, $"{pluginName}.json");
            if (!File.Exists(path)) return new T();

            try
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return new T();
            }
        }

        public void Save<T>(string pluginName, T data)
        {
            string path = Path.Combine(_configDir, $"{pluginName}.json");
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
