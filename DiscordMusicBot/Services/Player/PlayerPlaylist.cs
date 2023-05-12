using Discord;
using Spectre.Console;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuna.Handlers;
using Color = Discord.Color;

namespace Yuna.Services.Player
{
    internal class PlayerPlaylist
    {
        internal static async Task<Embed> PlaylistAsync(LavaNode node, IGuild guild, IVoiceState voiceState, ushort page)
        {
            var descriptionBuilder = new StringBuilder();
            node.TryGetPlayer(guild, out var player);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    if (player.Vueue.Count < 1 && player.Track != null)
                    {
                        return await EmbedHandler.BasicEmbed($"🎶 Now Playing: {player.Track.Title.EscapeMarkup()}", $"Nothing else Is queued.", Color.Default);
                    }
                    else
                    {
                        var skip = Math.Max(0, (page * 10) - 10);
                        var trackList = player.Vueue.Skip(skip).Take(10);
                        foreach (LavaTrack track in trackList)
                        {
                            skip++;
                            descriptionBuilder.Append($"{skip}: {track.Title}\n");
                        }

                        int totalPages = player.Vueue.Count % 10 >0 ? (player.Vueue.Count /10) +1 : player.Vueue.Count /10;
                        descriptionBuilder.Append($"Showing {trackList.Count()} out of {player.Vueue.Count}, page {page}/ {totalPages}");

                        return await EmbedHandler.BasicEmbed("🎶 Playlist", $"Now Playing: [{player.Track.Title.EscapeMarkup()}]({player.Track.Url}) \n{descriptionBuilder}", Color.Default);
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
