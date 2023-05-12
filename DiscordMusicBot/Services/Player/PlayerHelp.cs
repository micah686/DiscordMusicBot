using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuna;
using Yuna.Handlers;
using Yuna.Managers;

namespace DiscordMusicBot.Services.Player
{
    internal class PlayerHelp
    {
        internal static async Task<Embed> HelpAsync(CommandService commands, string command)
        {
            var cmds = commands.Commands.OrderBy(x => x.Name);

            if (command == null)
            {
                var s = string.Join("\n", cmds.Select(x => $"`{x.Name}`{(x.Aliases.Count - 1 == 0 ? "" : " - [**" + string.Join(", ", x.Aliases.Skip(1)) + "**]")}: {x.Summary}"));
                var embed = await EmbedHandler.BasicEmbed("", s, Color.Green);
                return embed;
            }

            var selectedCmd = cmds.Where(x => x.Name.ToLower() == command || x.Aliases.Contains(command, StringComparer.OrdinalIgnoreCase))?.Select(x => $"`{x.Name}`{(x.Aliases.Count - 1 == 0 ? "" : " - [**" + string.Join(", ", x.Aliases.Skip(1)) + "**]")}: {x.Summary}").FirstOrDefault();

            if (selectedCmd != null)
            {
                var embed = await EmbedHandler.BasicEmbed("",selectedCmd, Color.Green);
                return embed;
            }

            var error = await EmbedHandler.ErrorEmbed($"Command `{command}` is invalid.\nUse `{ConfigManager.BotConfig.Prefix}help` to see all commands");
            return error;
            
        }
    }
}
