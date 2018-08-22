using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class MiscCommands : ModuleBase
    {

 

        [Command("echo"), Summary("Echos text")]
        public async Task Echo(string text)
        {
            await ReplyAsync(text);
        }

    }
}
