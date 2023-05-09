using Discord;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Services.Player
{
    internal class PlayerSkip
    {
        internal static async Task<Embed> SkipAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            node.TryGetPlayer(guild, out var player);
            var сurrenttrack = player.Track;
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }

            try
            {
                if (player.Vueue.Count < 1) return await EmbedHandler.ErrorEmbed($"⚠️ Unable To skip a track as there is only one or no songs currently playing.");
                try
                {
                    await player.SkipAsync();
                    await LogService.LogInfoAsync("MUSIC", $"Skipped: \"{сurrenttrack.Title}\"");
                    return await EmbedHandler.BasicEmbed("", $"⏭️ Skipped: \"{сurrenttrack.Title}\".", Color.Default);
                }
                catch (Exception ex)
                {
                    return await EmbedHandler.ErrorEmbed(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
