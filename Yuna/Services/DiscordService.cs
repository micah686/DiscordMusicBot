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
       

        //// Initialize the Discord Client.
        //public async Task InitializeAsync()
        //{
        //    await InitializeGlobalDataAsync();

        //    await _client.LoginAsync(TokenType.Bot, SecureStore.Decrypt(SecureStore._token));
        //    await _client.StartAsync();

        //    await _commandHandler.InitializeAsync();

        //    await Task.Delay(-1);
        //}

        

        //// Used when the Client Fires the ReadyEvent.
        //private async Task ReadyAsync()
        //{
        //    try
        //    {
        //        if (!_lavaNode.IsConnected)
        //        {
        //            await _lavaNode.ConnectAsync();
        //        }
        //        await _client.SetActivityAsync(new Game ("", ActivityType.Listening));
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogService.LogInfoAsync(ex.Source, ex.Message);
        //    }

        //}


    }
}
