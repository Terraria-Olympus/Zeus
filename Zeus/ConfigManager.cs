using System;
using System.IO;
using Tomlet;

namespace Zeus
{
    public class ConfigManager
    {
        public string ConfigPath => Path.Combine(Environment.CurrentDirectory, "config.toml");

        public ConfigData Data = new ConfigData();

        public ConfigManager()
        {
            if (!File.Exists(ConfigPath))
                File.Create(ConfigPath).Close();
            else
                Data = TomletMain.To<ConfigData>(File.ReadAllText(ConfigPath));
        }

        public void Save()
        {
            File.WriteAllText(ConfigPath, TomletMain.TomlStringFrom(Data));
        }

        public class ConfigData
        {
            public string Theme = "Dark";
        }
    }
}
