using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerLeave
    {
        internal static async Task<Embed> LeaveAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            var player = node.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    //clear any endless, loop, repeat,... states here
                    player.Queue.Clear();
                    await player.StopAsync();
                }
                await node.LeaveAsync(player.VoiceChannel);

                await LogService.LogInfoAsync("MUSIC", $"Yuna has left.");
                return await EmbedHandler.BasicEmbed("🚫 Music", $"I've left.", Color.Red);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
