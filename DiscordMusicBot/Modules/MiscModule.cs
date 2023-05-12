using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordMusicBot.Services.Player;
using System.Threading.Tasks;
using Victoria.Node;
using Yuna.Handlers;
using Yuna.Managers;

namespace Yuna.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService commands;

        public MiscModule(CommandService commandService)
        {
            commands = commandService;
        }

        [Command("help", RunMode = RunMode.Async)]
        [Name("Leave"), Summary("Disconnects the bot from the voice channel.")]
        public async Task Help([Remainder] string command = null) => await ReplyAsync(embed: await PlayerHelp.HelpAsync(commands, command));
    }
}
