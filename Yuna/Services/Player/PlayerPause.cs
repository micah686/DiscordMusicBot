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
    internal class PlayerPause
    {
        internal static async Task<Embed> PauseAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            var player = node.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            try
            {
                if (!(player.PlayerState is PlayerState.Playing))
                {
                    await player.PauseAsync();
                    return await EmbedHandler.BasicEmbed("", "⚠️ There is nothing to pause.", Color.Red);
                }

                await player.PauseAsync();
                return await EmbedHandler.BasicEmbed("", $"⏸️ **Paused:** {player.Track.Title}", Color.Default);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
