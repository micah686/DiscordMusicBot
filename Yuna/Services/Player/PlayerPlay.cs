using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuna.Handlers;
using Victoria.Responses.Search;
using Yuna.Modules;

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

                var isUrl = Uri.IsWellFormedUriString(query, UriKind.Absolute);
                var isFile = new Uri(query).IsFile;
                var validLocation = isUrl || isFile;
                var search = validLocation ? await node.SearchAsync(SearchType.Direct, query)
                    : await node.SearchAsync(SearchType.YouTube, query);
                
                if (search.Status == SearchStatus.NoMatches)
                {
                    return await EmbedHandler.ErrorEmbed($"⚠️ I wasn't able to find anything for \"{query}\".");
                }
                else if (search.Status == SearchStatus.PlaylistLoaded)
                {
                    for (int trackNumber = 0; trackNumber < search.Tracks.Count; trackNumber++)
                    {
                        var track = search.Tracks.ElementAt(trackNumber);
                        if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                        {
                            player.Vueue.Enqueue(track);
                        }
                        else
                        {
                            if (trackNumber == 0)
                            {
                                await player.PlayAsync(track);
                            }
                            else
                            {
                                player.Vueue.Enqueue(track);
                            }
                        }
                    }
                    return await EmbedHandler.BasicEmbed("🎵 Music", $"The playlist \"{search.Playlist.Name}\" has been successfully added to the queue.", Color.Green);
                }
                else
                {
                    var track = search.Tracks.FirstOrDefault();
                    AudioModule.CurrentTrack = track;

                    if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                    {
                        player.Vueue.Enqueue(track);
                        LoggingService.Log($"\"{track.Title}\" has been added to the music queue", Spectre.Console.Color.Gold1, true);
                        return await EmbedHandler.BasicEmbed("🎵 Music", $"\"{track.Title}\" has been added to the music queue.", Color.Green);
                    }

                    await player.PlayAsync(track);
                    LoggingService.Log($"Now Playing: {track.Title}\nUrl: {track.Url}", Spectre.Console.Color.Gold1, true);
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
