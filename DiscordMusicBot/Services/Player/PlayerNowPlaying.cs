using Discord;
using Spectre.Console;
using System;
using System.Threading.Tasks;
using Yuna.Handlers;
using Color = Discord.Color;

namespace Yuna.Services.Player
{
    internal class PlayerNowPlaying
    {
        internal static async Task<Embed> NowPlayingAsync(LavaNode node, IGuild guild, IVoiceState voiceState)
        {
            node.TryGetPlayer(guild, out var player);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }
            try
            {
                if (player.PlayerState is PlayerState.Playing)
                    return await EmbedHandler.BasicEmbed("🎵 Music", $"Now Playing: \"{player.Track.Title.EscapeMarkup()}\"\nAuthor: {player.Track.Author}\nUrl: {player.Track.Url}", Color.Default);
                else
                    return await EmbedHandler.ErrorEmbed("⚠️ Nothing Else Is Queued.");
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
