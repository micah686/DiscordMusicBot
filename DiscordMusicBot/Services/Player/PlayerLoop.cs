using Discord;
using System.Threading.Tasks;
using Yuna.Handlers;
using Yuna.Modules;

namespace Yuna.Services.Player
{
    internal class PlayerLoop
    {
        internal static async Task<Embed> LoopAsync(LavaNode node, IGuild guild)
        {
            node.TryGetPlayer(guild, out var player);
            if(player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
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
