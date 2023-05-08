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
    internal class PlayerJoin
    {        

        internal static async Task<Embed> JoinAsync(LavaNode node,IGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            if (node.HasPlayer(guild))
            {
                var player = node.GetPlayer(guild);
                if (player.VoiceChannel == voiceState.VoiceChannel)
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ I'm already connected to the voice channel!");
                }
                else
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ I'm already connected to a different voice channel!");
                }
            }
            if (voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed(Constants.USER_NOT_IN_VOICE);
            }
            
            try
            {
                await node.JoinAsync(voiceState.VoiceChannel, textChannel);
                return await EmbedHandler.BasicEmbed("", $"✅ Joined  \"{voiceState.VoiceChannel.Name}\".", Color.Green);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
    }
}
