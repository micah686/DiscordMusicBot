using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerRemove
    {
        internal static async Task<Embed> RemoveAsync(LavaNode node, IGuild guild, IVoiceState voiceState, ushort index)
        {
            var player = node.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            if(index > player.Queue.Count) return await EmbedHandler.ErrorEmbed("⚠️ Index not in range of playlist entries");

            try
            {
                var trackToRemove = player.Queue.RemoveAt(index);
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
