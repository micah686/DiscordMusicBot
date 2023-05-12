using Discord;
using Spectre.Console;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;
using Color = Discord.Color;

namespace Yuna.Services.Player
{
    internal class PlayerPause
    {
        internal static async Task<Embed> PauseAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            node.TryGetPlayer(guild, out var player);
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
                return await EmbedHandler.BasicEmbed("", $"⏸️ **Paused:** {player.Track.Title.EscapeMarkup()}", Color.Default);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
