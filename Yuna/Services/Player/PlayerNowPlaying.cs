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
    internal class PlayerNowPlaying
    {
        internal static async Task<Embed> NowPlayingAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            var player = node.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }
            try
            {
                if (player.PlayerState is PlayerState.Playing)
                    return await EmbedHandler.BasicEmbed("🎵 Music", $"Now Playing: \"{player.Track.Title}\"\nAuthor: {player.Track.Author}\nUrl: {player.Track.Url}", Color.Default);
                else
                    return await EmbedHandler.ErrorEmbed("⚠️ Nothing Else Is Queued.");
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
