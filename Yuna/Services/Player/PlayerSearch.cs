using Discord;
using System.Threading.Tasks;
using System;
using Yuna.Handlers;
using Discord.WebSocket;
using Victoria.Responses.Search;
using System.Text;
using System.Linq;
using Yuna.Modules;
using System.Collections.Generic;

namespace Yuna.Services.Player
{
    internal class PlayerSearch
    {
        private static readonly Dictionary<int, string> _numberLookup = new Dictionary<int, string>()
        { {1, "one" }, {2,"two" }, {3,"three" }, {4,"four" }, {5,"five" }, {6,"six" }, {7,"seven" }, {8,"eight" }, {9,"nine" }, {10,"keycap_ten" } };
        internal static IEmote[] EmojiList { get; private set; }
        internal enum EmojiStates { One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten }
        internal static LavaTrack[] LavaTrackList { get; private set; }

        internal static async Task<Embed> SearchAsync(LavaNode node, SocketGuildUser user, IGuild guild, IVoiceState voice, ITextChannel channel, string query)
        {
            if (node.HasPlayer(guild))
            {
                node.TryGetPlayer(guild, out var player);
                var searchResults = await node.SearchAsync(SearchType.YouTube, query);
                var pls = searchResults.Tracks.Take(10);
                var descriptionBuilder = new StringBuilder();

                descriptionBuilder.AppendLine("Search returned:");
                List<Emoji> emojis = new List<Emoji>();
                List<LavaTrack> tracks = new List<LavaTrack>();
                int counter = 1;

                try
                {
                    foreach (var track in pls)
                    {
                        descriptionBuilder.AppendLine($"{counter}: {track.Title}");
                        emojis.Add(Emoji.Parse($":{_numberLookup[counter]}:"));
                        tracks.Add(track);
                        counter++;
                    }
                    LavaTrackList= tracks.ToArray();
                    EmojiList = emojis.ToArray();
                }
                catch (Exception ex)
                {

                    throw;
                }

                
                return await EmbedHandler.BasicEmbed("🎶 Playlist", descriptionBuilder.ToString(), Color.Default);

            }
            return await EmbedHandler.BasicEmbed("", $"Searched ", Color.Green);
        }


        
        


        internal static async Task AddReactions(IUserMessage message)
        {
            try
            {

                foreach (var em in EmojiList)
                {
                    await message.AddReactionAsync(em);
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
    }
}
