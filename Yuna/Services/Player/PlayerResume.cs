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
    internal class PlayerResume
    {
        internal static async Task<Embed> ResumeAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            var player = node.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            try
            {
                if (player.PlayerState is PlayerState.Paused)
                {
                    await player.ResumeAsync();
                }
                return await EmbedHandler.BasicEmbed("", $"▶️ **Resumed:** {player.Track.Title}", Color.Green);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
