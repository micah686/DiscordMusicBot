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
using Victoria.Node;
using Yuna.Managers;
using Microsoft.Extensions.Primitives;

namespace Yuna.Services.Player
{
    internal class PlayerSearch
    {
        private static readonly Dictionary<int, string> _numberLookup = new Dictionary<int, string>()
        { {1, "one" }, {2,"two" }, {3,"three" }, {4,"four" }, {5,"five" }, {6,"six" }, {7,"seven" }, {8,"eight" }, {9,"nine" }, {10,"keycap_ten" } };
        internal static IEmote[] _emojiList { get; private set; }
        internal static LavaTrack[] _lavaTrackList { get; private set; }

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

                foreach (var track in pls)
                {
                    descriptionBuilder.AppendLine($"{counter}: {track.Title}");
                    emojis.Add(Emoji.Parse($":{_numberLookup[counter]}:"));
                    tracks.Add(track);
                    counter++;
                }
                _lavaTrackList = tracks.ToArray();
                _emojiList = emojis.ToArray();


                return await EmbedHandler.BasicEmbed("🎶 Search", descriptionBuilder.ToString(), Color.Default);

            }
            return await EmbedHandler.BasicEmbed("", $"Searched ", Color.Green);
        }
                
        internal static async Task AddReactions(IUserMessage message)
        {
            foreach (var em in _emojiList)
            {
                await message.AddReactionAsync(em);
            }
        }

        public static async Task ClientReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction reaction, LavaNode node)
        {
            try
            {
                var channel = await messageChannel.GetOrDownloadAsync();

                if (channel is not IGuildChannel guildChannel)
                {
                    return;
                }
                var msg = await message.GetOrDownloadAsync();

                if (!_emojiList.Contains(reaction.Emote))
                {
                    await Task.CompletedTask;
                    return;
                }

                _ = Task.Run(async () =>
                {
                    var currentState = Array.IndexOf(_emojiList, reaction.Emote);

                    if (reaction.UserId == ConfigManager.BotConfig.BotUserId)
                    {
                        await Task.CompletedTask;
                        return;
                    }
                    try
                    {
                        await msg.RemoveReactionAsync(reaction.Emote, reaction.User.Value, options: new RequestOptions { RetryMode = RetryMode.RetryRatelimit });
                    }
                    catch (Exception)
                    {
                        return;                       
                    }
                    

                    try
                    {
                        if (!node.HasPlayer(guildChannel.Guild))
                        {
                            return;
                        }
                    }
                    catch
                    {
                        var error = await EmbedHandler.ErrorEmbed($"Couldn't find Server/Channel");
                        await channel.SendMessageAsync(embed: error);
                    }

                    node.TryGetPlayer(guildChannel.Guild, out var player);


                    
                    if(currentState >= 0 && currentState <= 9)
                    {
                        await player.PlayAsync(_lavaTrackList[currentState - 1]);
                    }

                    
                });
            }
            catch (Exception)
            {

                return;
            }

        }

    }
}
