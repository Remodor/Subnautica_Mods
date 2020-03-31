using System.IO;
using System;
using Oculus.Newtonsoft.Json;
namespace Rm_Config
{
    internal class Config<T> where T : new()
    {
        private static T configuration;
        private static string file;
        internal static T LoadConfiguration(string file)
        {
            Config<T>.file = file;
            try
            {
                string configPath = CreateConfigPath();
                Console.WriteLine(string.Format("[{0}] Load configuration:    \"{1}\"", typeof(T).Namespace, configPath));
                string configJson = File.ReadAllText(configPath);
                configuration = JsonConvert.DeserializeObject<T>(configJson);
                Console.WriteLine(string.Format("[{0}] ConfigFile found!", typeof(T).Namespace));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(string.Format("[{0}] ConfigFile not found!", typeof(T).Namespace));
                configuration = new T();
                SaveConfiguration();
            }
            return configuration;
        }
        internal static T LoadConfiguration()
        {
            return LoadConfiguration("config.json");
        }
        internal static void SaveConfiguration()
        {
            Console.WriteLine(string.Format("[{0}] Creating ConfigFile:", typeof(T).Namespace));
            string configPath = CreateConfigPath();
            string configJson = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            File.WriteAllText(configPath, configJson);
            Console.WriteLine(string.Format("[{0}] Done..", typeof(T).Namespace));
        }
        internal static T Get()
        {
            if (configuration == null)
            {
                LoadConfiguration();
            }
            return configuration;
        }

        private static string CreateConfigPath()
        {
            return "./QMods/" + typeof(T).Namespace + "/" + file;
        }
    }
}
