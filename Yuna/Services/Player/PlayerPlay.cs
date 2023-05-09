using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuna.Handlers;
using Victoria.Responses.Search;

namespace Yuna.Services.Player
{
    internal class PlayerPlay
    {
        internal static async Task<Embed> PlayAsync(LavaNode node, SocketGuildUser user, IGuild guild, IVoiceState voice, ITextChannel channel, string query)
        {
            if (user.VoiceChannel == null)
            {
                return await EmbedHandler.ErrorEmbed("You Must First Join a Voice Channel.");
            }

            //Check the guild has a player available.
            if (!node.HasPlayer(guild))
            {
                return await EmbedHandler.BasicEmbed("Music", "I'm not connected to a voice channel.", Color.DarkRed);
            }

            try
            {
                node.TryGetPlayer(guild, out var player);

                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                await node.SearchAsync(Victoria.Responses.Search.SearchType.Direct,query)
                    : await node.SearchAsync(Victoria.Responses.Search.SearchType.YouTube,query);
                
                if (search.Status == SearchStatus.NoMatches)
                {
                    return await EmbedHandler.ErrorEmbed($"⚠️ I wasn't able to find anything for \"{query}\".");
                }
                //else if (search.LoadStatus == LoadStatus.PlaylistLoaded)
                //{
                //    for (int trackNumber = 0; trackNumber < search.Tracks.Count; trackNumber++)
                //    {
                //        track = search.Tracks.ElementAt(trackNumber);
                //        if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                //        {
                //            player.Vueue.Enqueue(track);
                //        }
                //        else
                //        {
                //            if (trackNumber == 0)
                //            {
                //                await player.PlayAsync(track);
                //            }
                //            else
                //            {
                //                player.Vueue.Enqueue(track);
                //            }
                //        }
                //    }
                //    return await EmbedHandler.BasicEmbed("🎵 Music", $"The playlist \"{search.Playlist.Name}\" has been successfully added to the queue.", Color.Green);
                //}
                else
                {
                    var track = search.Tracks.FirstOrDefault();

                    if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                    {
                        player.Vueue.Enqueue(track);
                        await LogService.LogInfoAsync("MUSIC", $"\"{track.Title}\" has been added to the music queue.");
                        return await EmbedHandler.BasicEmbed("🎵 Music", $"\"{track.Title}\" has been added to the music queue.", Color.Green);
                    }

                    await player.PlayAsync(track);
                    await LogService.LogInfoAsync("MUSIC", $"Yuna Now Playing: {track.Title}\nUrl: {track.Url}");
                    return await EmbedHandler.BasicEmbed("🎵 Music", $"Now Playing: \"{track.Title}\"\nAuthor: {track.Author}\nUrl: {track.Url}", Color.Green);
                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
