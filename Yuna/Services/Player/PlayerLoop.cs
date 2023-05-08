using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Yuna.Handlers;
using Yuna.Modules;

namespace Yuna.Services.Player
{
    internal class PlayerLoop
    {
        internal static async Task<Embed> LoopAsync(LavaNode node, IGuild guild)
        {
            var player = node.GetPlayer(guild);
            if(player.PlayerState == Victoria.Enums.PlayerState.Playing || player.PlayerState == Victoria.Enums.PlayerState.Paused)
            {
                AudioModule.LoopEnabled = !AudioModule.LoopEnabled;
                return await EmbedHandler.BasicEmbed("", $"{(AudioModule.LoopEnabled ? "🔂 Loop enabled." : "❌ Loop disabled")}", Color.Green);
            }
            else
            {
                return await EmbedHandler.ErrorEmbed($"⚠️ Must be playing or paused to change loop state");
            }            
        }
    }
}
