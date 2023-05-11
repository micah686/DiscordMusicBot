using Discord;
using System.Threading.Tasks;
using Yuna.Handlers;
using Yuna.Helpers;

namespace Yuna.Services.Player
{
    internal class PlayerSeek
    {
        internal static async Task<Embed> SeekAsync(LavaNode node, IGuild guild, string seekTime)
        {
            if (!node.HasPlayer(guild))
            {
                return await EmbedHandler.ErrorEmbed(Constants.PLAYER_NOT_FOUND);
            }

            node.TryGetPlayer(guild, out var player);

            if (player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                return await EmbedHandler.ErrorEmbed("⚠️ Must be playing or paused to seek");
            }

            var pos = player.Track.Position;
            var len = player.Track.Duration;

            if (seekTime == null && player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                return await EmbedHandler.BasicEmbed("",$"Current Position: {pos.ToTimecode()}/{len.ToTimecode()}", Color.Green);
            }

            int seperators = seekTime.Split(":").Length;

            var seek = seperators switch
            {
                3 => seekTime.ToTimeSpan(),
                2 => ("00:" + seekTime).ToTimeSpan(),
                1 => ("00:00:" + seekTime).ToTimeSpan(),
                _ => default,
            };

            if (len.TotalMilliseconds - seek.TotalMilliseconds < 0)
            {
                return await EmbedHandler.BasicEmbed("", $"You can only seek up to {len.ToTimecode()}", Color.Green);
                
            }
            else
            {
                await player.SeekAsync(seek);
                return await EmbedHandler.BasicEmbed("", $"Seeked to `{seek.ToTimecode()}`.", Color.Green);
            }
        }
    }
}
