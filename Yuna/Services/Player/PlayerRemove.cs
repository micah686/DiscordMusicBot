using Discord;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerRemove
    {
        internal static async Task<Embed> RemoveAsync(LavaNode node, IGuild guild, IVoiceState voiceState, ushort index)
        {
            node.TryGetPlayer(guild, out var player);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            if(index > player.Vueue.Count) return await EmbedHandler.ErrorEmbed("⚠️ Index not in range of playlist entries");

            try
            {
                var trackToRemove = player.Vueue.RemoveAt(index);
                await LogService.LogInfoAsync("MUSIC", $"Removed {trackToRemove.Title} from queue");
                return await EmbedHandler.BasicEmbed("", $"❌ Removed \"{trackToRemove.Title}\" from queue.", Color.Default);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
    }
}
