using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Yuna.Helpers;
using Yuna.Services;

namespace Yuna.Managers
{
    public static class ConfigManager
    {
        public const string ConfigName = "appConfig.json";
        public const string TokenUnset = "##Token_Not_Set##";
        public const string TokenSecureSet = "##Token_Secured##";
        public static Config BotConfig { get; private set; }

        static ConfigManager()
        {

            if (!File.Exists(ConfigName))
            {
                CreateConfigFile();
                Console.WriteLine("No config file was found. A new one was created, please fill config file and restart the bot.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                SecureToken();
                BotConfig = LoadConfigFile();
            }
        }



        public static void CreateConfigFile()
        {
            Config config = new Config
            {
                Prefix = '-',
                Token = TokenUnset,
            };
            File.WriteAllText(ConfigName, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public static Config LoadConfigFile()
        {
            string json = File.ReadAllText(ConfigName);
            var config = JsonConvert.DeserializeObject<Config>(json);
            return config;
        }

        public static void UpdateConfigFile(Config config)
        {
            File.WriteAllText(ConfigName, JsonConvert.SerializeObject(config, Formatting.Indented));
            BotConfig = config;
        }

        public static void SecureToken()
        {
            var config = LoadConfigFile();
            if (config.Token != TokenUnset && config.Token != TokenSecureSet)
            {
                if (!SecureStore.DoesSecureStoreExist())
                {
                    SecureStore.CreateSecureStore();
                }
                SecureStore.Update(SecureStore._token, config.Token);
                config.Token = TokenSecureSet;
                Console.WriteLine("Token Secured");
                UpdateConfigFile(config);
            }
        }
        public static string GetTokenInsecure()
        {
            return SecureStore.Decrypt(SecureStore._token);
        }
    }

    public struct Config
    {
        public string Token { get; set; }
        public char Prefix { get; set; }
        public ulong ServerGuild { get; set; }
        public ulong BotUserId { get; set;}
    }
}
