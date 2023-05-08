using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerMove
    {
        internal static async Task<Embed> MoveAsync(LavaNode node, IGuild guild, ushort index)
        {
            try
            {
                var player = node.GetPlayer(guild);
                var queue = player.Queue.ToList();
                var trackToMove = queue.ElementAt(index);
                queue.RemoveAt(index);
                player.Queue.Clear();
                player.Queue.Enqueue(trackToMove);
                foreach (var track in queue)
                {
                    player.Queue.Enqueue(track);
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
