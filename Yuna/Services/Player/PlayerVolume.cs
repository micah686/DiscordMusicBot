using Discord;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerVolume
    {
        internal static async Task<Embed> VolumeAsync(LavaNode node, IGuild guild, IVoiceState voiceState, ushort volume)
        {
            node.TryGetPlayer(guild, out var player);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            if (volume > 150 || volume < 0)
            {
                return await EmbedHandler.BasicEmbed("", "⚠️ Volume must be between 0 and 150.", Color.Red);
            }

            try
            {
                await player.SetVolumeAsync(volume);
                await LogService.LogInfoAsync("MUSIC", $"Bot Volume set to: {volume}");
                return await EmbedHandler.BasicEmbed("", $"🔊 Bot Volume set to: {volume}.", Color.Default);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
    }
}
