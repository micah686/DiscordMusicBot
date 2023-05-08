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
    internal class PlayerPlaylist
    {
        internal static async Task<Embed> PlaylistAsync(LavaNode node, IGuild guild, IVoiceState voiceState, ushort page)
        {
            var descriptionBuilder = new StringBuilder();
            var player = node.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    if (player.Queue.Count < 1 && player.Track != null)
                    {
                        return await EmbedHandler.BasicEmbed($"🎶 Now Playing: {player.Track.Title}", $"Nothing else Is queued.", Color.Default);
                    }
                    else
                    {
                        var skip = Math.Max(0, (page * 10) - 10);
                        var trackList = player.Queue.Skip(skip).Take(10);
                        foreach (LavaTrack track in trackList)
                        {
                            skip++;
                            descriptionBuilder.Append($"{skip}: {track.Title}\n");
                        }

                        int totalPages = player.Queue.Count % 10 >0 ? (player.Queue.Count /10) +1 : player.Queue.Count /10;
                        descriptionBuilder.Append($"Showing {trackList.Count()} out of {player.Queue.Count}, page {page}/ {totalPages}");

                        return await EmbedHandler.BasicEmbed("🎶 Playlist", $"Now Playing: [{player.Track.Title}]({player.Track.Url}) \n{descriptionBuilder}", Color.Default);
                    }
                }
                else
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ Player doesn't seem to be playing anything right now.");
                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
