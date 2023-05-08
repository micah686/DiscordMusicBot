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
    internal class PlayerReplay
    {
        internal static async Task<Embed> ReplayAsync(LavaNode node, IGuild guild)
        {
            var player = node.GetPlayer(guild);
            if (player.PlayerState != PlayerState.Playing)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ No song playing");
            }
            await player.SeekAsync(TimeSpan.Zero);
            return await EmbedHandler.BasicEmbed("", $"Replayed {player.Track.Title}", Color.Green);            
        }
    }
}
