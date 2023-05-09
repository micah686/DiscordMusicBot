using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Node;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Yuna.Managers;
using Spectre.Console;
using System;
using Yuna.Services;
using Color = Spectre.Console.Color;

namespace Yuna
{
    public class Bot
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;


        public Bot()
        {
            AnsiConsole.Write(new FigletText("Music Bot").Centered().Color(Color.DodgerBlue2));


            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = Discord.LogSeverity.Debug,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                LogLevel = Discord.LogSeverity.Debug,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                IgnoreExtraArgs = true

            });

            var collection = new ServiceCollection();

            collection.AddLogging();
            collection.AddSingleton(_client);
            collection.AddSingleton(_commandService);
            collection.AddSingleton<NodeConfiguration>();
            collection.AddSingleton<LavaNode>();
            collection.AddLavaNode(x =>
            {
                x.SelfDeaf = false;
                x.SocketConfiguration = new Victoria.WebSocket.WebSocketConfiguration { BufferSize = 1024 };
            });


            ServiceManager.SetProvider(collection);

        }
        public async Task MainAsync()
        {
            if (string.IsNullOrWhiteSpace(ConfigManager.GetTokenInsecure())) return;
            await CommandManager.LoadCommandsAsync();
            await EventManager.LoadCommands();
            await _client.LoginAsync(TokenType.Bot, ConfigManager.GetTokenInsecure());
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
