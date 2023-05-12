using Discord;
using Spectre.Console;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;
using Color = Discord.Color;

namespace Yuna.Services.Player
{
    internal class PlayerResume
    {
        internal static async Task<Embed> ResumeAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            node.TryGetPlayer(guild, out var player);
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
                return await EmbedHandler.BasicEmbed("", $"▶️ **Resumed:** {player.Track.Title.EscapeMarkup()}", Color.Green);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
