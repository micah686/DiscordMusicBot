using Discord;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerLeave
    {
        internal static async Task<Embed> LeaveAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            node.TryGetPlayer(guild, out var player);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    //clear any endless, loop, repeat,... states here
                    player.Vueue.Clear();
                    await player.StopAsync();
                }
                await node.LeaveAsync(player.VoiceChannel);

                LoggingService.Log($"Music bot has left", Spectre.Console.Color.Gold1, true);
                return await EmbedHandler.BasicEmbed("🚫 Music", $"I've left.", Color.Red);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
