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
    internal class PlayerClear
    {
        internal static async Task<Embed> ClearAsync(LavaNode node, IGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            if (!node.HasPlayer(guild))
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            var player = node.GetPlayer(guild);
            player.Queue.Clear();

            return await EmbedHandler.BasicEmbed("", "Queue cleared.", Color.Green);
        }
    }
}
