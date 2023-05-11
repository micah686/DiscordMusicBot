using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerMove
    {
        internal static async Task<Embed> MoveAsync(LavaNode node, IGuild guild, ushort index)
        {
            try
            {
                node.TryGetPlayer(guild, out var player);
                var queue = player.Vueue.ToList();
                var trackToMove = queue.ElementAt(index);
                queue.RemoveAt(index);
                player.Vueue.Clear();
                player.Vueue.Enqueue(trackToMove);
                foreach (var track in queue)
                {
                    player.Vueue.Enqueue(track);
                }
                return await EmbedHandler.BasicEmbed("", $"{trackToMove} moved to start of queue", Color.Green);
            }
            catch (Exception)
            {

                return await EmbedHandler.ErrorEmbed("⚠️ Failed to move track to top of queue.");
            }
        }
    }
}
