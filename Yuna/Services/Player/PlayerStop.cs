using Discord;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerStop
    {
        internal static async Task<Embed> StopAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            node.TryGetPlayer(guild, out var player);

            if (player is null)
                return await EmbedHandler.ErrorEmbed(Constants.PLAYER_NOT_FOUND);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    //clear any endless, loop, repeat,... states here
                    player.Vueue.Clear();
                    await player.StopAsync();
                }
                LoggingService.Log($"Music bot has stopped playback", Spectre.Console.Color.Gold1, true);
                return await EmbedHandler.BasicEmbed("", "⏹️ I have stopped playback & the playlist has been cleared.", Color.Default);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
