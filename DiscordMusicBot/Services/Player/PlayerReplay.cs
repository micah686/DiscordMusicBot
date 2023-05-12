using Discord;
using Spectre.Console;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;
using Color = Discord.Color;

namespace Yuna.Services.Player
{
    internal class PlayerReplay
    {
        internal static async Task<Embed> ReplayAsync(LavaNode node, IGuild guild)
        {
            node.TryGetPlayer(guild, out var player);
            if (player.PlayerState != PlayerState.Playing)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ No song playing");
            }
            await player.SeekAsync(TimeSpan.Zero);
            return await EmbedHandler.BasicEmbed("", $"Replayed {player.Track.Title.EscapeMarkup()}", Color.Green);            
        }
    }
}
