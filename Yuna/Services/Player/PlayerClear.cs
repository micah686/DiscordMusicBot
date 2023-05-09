using Discord;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerClear
    {
        internal static async Task<Embed> ClearAsync(LavaNode node, IGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            if (!node.HasPlayer(guild))
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            node.TryGetPlayer(guild, out var player);
            player.Vueue.Clear();

            return await EmbedHandler.BasicEmbed("", "Queue cleared.", Color.Green);
        }
    }
}
