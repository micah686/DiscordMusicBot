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

                
                return await EmbedHandler.BasicEmbed("🎶 Search", descriptionBuilder.ToString(), Color.Default);

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

                if (!EmojiList.Contains(reaction.Emote))
                {
                    await Task.CompletedTask;
                    return;
                }

                _ = Task.Run(async () =>
                {
                    EmojiStates currentState = (EmojiStates)Array.IndexOf(EmojiList, reaction.Emote);

                    if (reaction.UserId == ConfigManager.BotConfig.BotUserId)
                    {
                        await Task.CompletedTask;
                        return;
                    }
                    await msg.RemoveReactionAsync(reaction.Emote, reaction.User.Value, options: new RequestOptions { RetryMode = RetryMode.RetryRatelimit });

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

                    if (!(player.PlayerState is PlayerState.Playing or PlayerState.Paused))
                    {
                        return;
                    }

                    switch (currentState)
                    {
                        case EmojiStates.One:
                            await player.PlayAsync(LavaTrackList[0]);
                            break;
                        case EmojiStates.Two:
                            await player.PlayAsync(LavaTrackList[1]);
                            break;
                        case EmojiStates.Three:
                            await player.PlayAsync(LavaTrackList[2]);
                            break;
                        case EmojiStates.Four:
                            await player.PlayAsync(LavaTrackList[3]);
                            break;
                        case EmojiStates.Five:
                            await player.PlayAsync(LavaTrackList[4]);
                            break;
                        case EmojiStates.Six:
                            await player.PlayAsync(LavaTrackList[5]);
                            break;
                        case EmojiStates.Seven:
                            await player.PlayAsync(LavaTrackList[6]);
                            break;
                        case EmojiStates.Eight:
                            await player.PlayAsync(LavaTrackList[7]);
                            break;
                        case EmojiStates.Nine:
                            await player.PlayAsync(LavaTrackList[8]);
                            break;
                        case EmojiStates.Ten:
                            await player.PlayAsync(LavaTrackList[9]);
                            break;
                        default:
                            break;
                    }
                });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
