﻿using Discord.Commands;
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
using System.Xml.Linq;
using Yuna.Modules;
using Yuna.Handlers;
using System.IO;

namespace Yuna.Managers
{
    public static class EventManager
    {
        private static LavaNode _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode>();
        private static DiscordSocketClient _client = ServiceManager.GetService<DiscordSocketClient>();
        private static CommandService _commandService = ServiceManager.GetService<CommandService>();
        private static bool _closing = false;

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

            Console.CancelKeyPress += (s, e) => BotExit(e);
            AppDomain.CurrentDomain.ProcessExit +=  (s, e) =>  BotExit(e);

            if(File.Exists(Constants.LAUNCHSTATE_FILE) && File.ReadAllBytes(Constants.LAUNCHSTATE_FILE).SequenceEqual(Constants.START_BYTES))
            {
                LoggingService.Log("Previous exit was unclean.", LogSeverity.Warning);
            }
            File.WriteAllBytes(Constants.LAUNCHSTATE_FILE, Constants.START_BYTES);
        }


        private static void BotExit(EventArgs e)
        {
           if(_closing) return;
            try
            {
                _closing = true;
                var context = AudioModule.Instance.GetDiscordContext();
                                
                _lavaNode.TryGetPlayer(context.Item1, out var player);
                Task.Run(() => { _lavaNode.LeaveAsync(player.VoiceChannel); });

                var embedLeave = EmbedHandler.BasicEmbed("🚫 Music", $"I've left.", Discord.Color.Red).Result;
                Task.Run(() => { _client.GetGuild(context.Item1.Id).GetTextChannel(context.Item2.Id).SendMessageAsync(embed: embedLeave); });                                
                LoggingService.Log($"Music bot has left", Spectre.Console.Color.Gold1, true);
                File.WriteAllBytes(Constants.LAUNCHSTATE_FILE, Constants.END_BYTES);
            }
            catch (Exception ex)
            {

                throw;
            }

            
            Environment.Exit(0);
        }
    }
}
