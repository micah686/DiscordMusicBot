using Discord;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerShuffle
    {
        internal static async Task<Embed> ShuffleAsync(LavaNode node, IGuild guild)
        {
            node.TryGetPlayer(guild, out var player);            
            try
            {
                if (player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                {
                    if(player.Vueue.Count <2) return await EmbedHandler.ErrorEmbed("⚠️ Cannot shuffle with less than 2 tracks.");
                    player.Vueue.Shuffle();
                    return await EmbedHandler.BasicEmbed("", $"🔀 Shuffled {player.Vueue.Count} tracks", Color.Default);
                }
                return await EmbedHandler.ErrorEmbed("⚠️ Must be playing or paused to shuffle tracks.");
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }            
        }
    }
}
