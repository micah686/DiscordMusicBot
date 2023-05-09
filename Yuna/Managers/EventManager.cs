using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuna.Services;
using Spectre.Console;

namespace Yuna.Managers
{
    public static class EventManager
    {
        private static LavaNode _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode>();
        private static DiscordSocketClient _client = ServiceManager.GetService<DiscordSocketClient>();
        private static CommandService _commandService = ServiceManager.GetService<CommandService>();

        public static Task LoadCommands()
        {

            _client.Log += message =>
            {
                LoggingService.Log($"({message.Source})\t{message.Message}", message.Severity);
                return Task.CompletedTask;
            };

            _commandService.Log += message =>
            {
                LoggingService.Log($"({message.Source})\t{message.Message}", message.Severity);
                return Task.CompletedTask;
            };

            _client.Ready += ClientReady;
            _client.MessageReceived += OnMessageRecieved;

            return Task.CompletedTask;
        }

        private async static Task OnMessageRecieved(SocketMessage arg)
        {
            LoggingService.Log(arg.ToString());
            var argPos = 0;
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot || message.Channel is IDMChannel) return;


            if (!(message.HasCharPrefix(ConfigManager.BotConfig.Prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            var result = await _commandService.ExecuteAsync(context, argPos, ServiceManager.Provider);

            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand) return;
            }
        }

        private static async Task ClientReady()
        {
            try
            {
                await _lavaNode.ConnectAsync();
            }
            catch (Exception)
            {
                throw;
            }

            var topCanvas = new Canvas(Console.WindowWidth, 2);
            for (var i = 0; i < topCanvas.Width; i++) { topCanvas.SetPixel(i, 0, Spectre.Console.Color.Green4); }
            AnsiConsole.Write(topCanvas);
            AnsiConsole.Write(new FigletText("Ready").Centered().Color(Spectre.Console.Color.Green));
            var bottomCanvas = new Canvas(Console.WindowWidth, 4);
            for (var i = 0; i < bottomCanvas.Width; i++) { bottomCanvas.SetPixel(i, 0, Spectre.Console.Color.Green4); }
            AnsiConsole.Write(bottomCanvas);


            await _client.SetStatusAsync(Discord.UserStatus.Online);
            await _client.SetGameAsync($"Prefix: {ConfigManager.BotConfig.Prefix}", null, ActivityType.Listening);
        }
    }
}
