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
using System.Xml.Linq;
using Yuna.Modules;
using Yuna.Handlers;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace Yuna.Managers
{
    public static class EventManager
    {
        private static LavaNode _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode>();
        private static DiscordSocketClient _client = ServiceManager.GetService<DiscordSocketClient>();
        private static CommandService _commandService = ServiceManager.GetService<CommandService>();

        internal static bool exitSystem = false;

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            CleanupAndExit();
            //do your cleanup here
            Thread.Sleep(5000); //simulate some cleanup delay

            Console.WriteLine("Cleanup complete");

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion




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

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //if (File.Exists(Constants.LAUNCHSTATE_FILE) && File.ReadAllBytes(Constants.LAUNCHSTATE_FILE).SequenceEqual(Constants.START_BYTES))
            //{
            //    LoggingService.Log("Previous exit was unclean.", LogSeverity.Warning);
            //}
            //File.WriteAllBytes(Constants.LAUNCHSTATE_FILE, Constants.START_BYTES);



            _client.UserVoiceStateUpdated += _client_UserVoiceStateUpdated;

            var entries = _client.GetGuild(151508131476799489).GetVoiceChannel(1104947439502360576).ConnectedUsers.ToList();
            var prevBotInstance = entries.Where(x => x.Id == _client.CurrentUser.Id).FirstOrDefault();
            if(prevBotInstance != null)
            {
                var sessionId = prevBotInstance.VoiceSessionId;
                Trace.WriteLine($"old session ID:{sessionId}");                   
            }
        }

        private static Task _client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState prevChannel, SocketVoiceState currentChannel)
        {
            if(user.Id == _client.CurrentUser.Id)
            {
                var voiceState = user as SocketGuildUser;
                var voiceSessionId = voiceState.VoiceSessionId;
                Trace.WriteLine($"new session ID:{voiceSessionId}");
                
            }
            return Task.CompletedTask;
        }

        private static async Task CleanupAndExit()
        {            
            var context = AudioModule.Instance.GetDiscordContext();
            _lavaNode.TryGetPlayer(context.Item1, out var player);
            await _lavaNode.LeaveAsync(player.VoiceChannel);
            var embedLeave = await EmbedHandler.BasicEmbed("🚫 Music", $"I've left.", Discord.Color.Red);
            await _client.GetGuild(context.Item1.Id).GetTextChannel(context.Item2.Id).SendMessageAsync(embed: embedLeave);
            LoggingService.Log($"Music bot has left", Spectre.Console.Color.Gold1, true);
            //File.WriteAllBytes(Constants.LAUNCHSTATE_FILE, Constants.END_BYTES);
            LoggingService.Log($"Finished cleaning up. Bot will now exit", Spectre.Console.Color.Gold1, true);
        }        
    }
}
