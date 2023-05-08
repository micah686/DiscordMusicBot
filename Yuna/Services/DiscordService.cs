using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;
using Victoria;
using Yuna.Helpers;
using Yuna.Data;
using Yuna.Modules;

namespace Yuna.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly ServiceProvider _services;
        private readonly LavaNode _lavaNode;
        private readonly LavaLinkAudioService _audioService;
        private readonly ConfigData _globalData;


        public DiscordService()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commandHandler = _services.GetRequiredService<CommandHandler>();
            _lavaNode = _services.GetRequiredService<LavaNode>();
            _globalData = _services.GetRequiredService<ConfigData>();
            _audioService = _services.GetRequiredService<LavaLinkAudioService>();

            SubscribeLavaLinkEvents();
            SubscribeDiscordEvents();
        }

        // Initialize the Discord Client.
        public async Task InitializeAsync()
        {
            await InitializeGlobalDataAsync();

            await _client.LoginAsync(TokenType.Bot, SecureStore.Decrypt(SecureStore._token));
            await _client.StartAsync();

            await _commandHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        // Hook Any Client Events Up Here.
        private void SubscribeLavaLinkEvents()
        {
            _lavaNode.OnLog += LogAsync;
            _lavaNode.OnTrackEnded += _audioService.TrackEnded;
        }

        private void SubscribeDiscordEvents()
        {
            _client.Ready += ReadyAsync;
            _client.Log += LogAsync;
        }

        private async Task InitializeGlobalDataAsync()
        {
            await _globalData.InitializeAsync();
        }

        // Used when the Client Fires the ReadyEvent.
        private async Task ReadyAsync()
        {
            try
            {
                if (!_lavaNode.IsConnected)
                {
                    await _lavaNode.ConnectAsync();
                }
                await _client.SetActivityAsync(new Game ("", ActivityType.Listening));
            }
            catch (Exception ex)
            {
                await LogService.LogInfoAsync(ex.Source, ex.Message);
            }

        }

        // Used whenever we want to log something to the Console. 
        private async Task LogAsync(LogMessage logMessage)
        {
            await LogService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        // Configure our Services for Dependency Injection.
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddLavaNode(x =>
                {
                    x.SelfDeaf = false;
                    x.Authorization = ConfigData.Config.LavalinkPassword;
                    x.Hostname = ConfigData.Config.LavalinkHost;
                    x.Port = ConfigData.Config.LavalinkPort;
                    x.LogSeverity = LogSeverity.Debug;
                })
                .AddSingleton(new LavaConfig())
                .AddSingleton<LavaLinkAudioService>()
                .AddSingleton<ConfigData>()
                .AddSingleton<AudioModule>()
                .BuildServiceProvider();
        }

    }
}
