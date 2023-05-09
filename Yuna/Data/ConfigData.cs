using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Yuna.Helpers;
using Yuna.Services;

namespace Yuna.Data
{
    //public class ConfigData
    //{
    //    public const string ConfigName = "appConfig.json";
    //    public const string TokenUnset = "##Token_Not_Set##";
    //    public const string TokenSecureSet = "##Token_Secured##";
    //    public static Config Config { get; set; }

    //    //Initialize the BotConfig and Global Properties.
    //    public async Task InitializeAsync()
    //    {
    //        //Checking whether config.json exists.
    //        if (!File.Exists(ConfigName))
    //        {
    //            CreateConfigFile();
    //            await LogService.LogCritAsync("Bot", "No config file was found. A new one was created, please fill config file and restart the bot.");
    //            await Task.Delay(-1);
    //        }
    //        SecureToken();
    //        Config = LoadConfigFile();

    //    }

    //    public static void CreateConfigFile()
    //    {
    //        Config config = new Config
    //        {
    //            Prefix = "-",
    //            Token = TokenUnset,
    //            LavalinkHost = "localhost",
    //            LavalinkPassword = "lavalink password",
    //            LavalinkPort = 2333
    //        };
    //        File.WriteAllText(ConfigName, JsonConvert.SerializeObject(config, Formatting.Indented));
    //    }

    //    public static Config LoadConfigFile()
    //    {
    //        string json = File.ReadAllText(ConfigName);
    //        var config = JsonConvert.DeserializeObject<Config>(json);
    //        return config;
    //    }

    //    public static void UpdateConfigFile(Config config)
    //    {
    //        File.WriteAllText(ConfigName, JsonConvert.SerializeObject(config, Formatting.Indented));
    //        Config = config;
    //    }

    //    public static void SecureToken()
    //    {
    //        var config = LoadConfigFile();
    //        if (config.Token != TokenUnset && config.Token != TokenSecureSet)
    //        {
    //            if (!SecureStore.DoesSecureStoreExist())
    //            {
    //                SecureStore.CreateSecureStore();
    //            }
    //            SecureStore.Update(SecureStore._token, config.Token);
    //            config.Token = TokenSecureSet;
    //            Console.WriteLine("Token Secured");
    //            UpdateConfigFile(config);
    //        }
    //    }
    //    public static string GetTokenInsecure()
    //    {
    //        return SecureStore.Decrypt(SecureStore._token);
    //    }
    //}
}
