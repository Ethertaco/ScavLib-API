using System;
using BepInEx.Configuration;

namespace ScavLib.config
{

    public class ConfigManager
    {
        private readonly ConfigFile _config;

        public ConfigManager(ConfigFile config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description = "")
        {
            return _config.Bind(section, key, defaultValue, description);
        }

        public ConfigEntry<T> Bind<T>(string section, string key, T defaultValue,
            string description, AcceptableValueBase acceptableValues)
        {
            return _config.Bind(section, key, defaultValue,
                new ConfigDescription(description, acceptableValues));
        }

        public ConfigEntry<T> Bind<T>(string section, string key, T defaultValue,
            ConfigDescription configDescription)
        {
            return _config.Bind(section, key, defaultValue, configDescription);
        }
    }
}
