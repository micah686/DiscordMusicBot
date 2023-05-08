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
    internal class PlayerShuffle
    {
        internal static async Task<Embed> ShuffleAsync(LavaNode node, IGuild guild)
        {
            var player = node.GetPlayer(guild);            
            try
            {
                if (player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                {
                    if(player.Queue.Count <2) return await EmbedHandler.ErrorEmbed("⚠️ Cannot shuffle with less than 2 tracks.");
                    player.Queue.Shuffle();
                    return await EmbedHandler.BasicEmbed("", $"🔀 Shuffled {player.Queue.Count} tracks", Color.Default);
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
