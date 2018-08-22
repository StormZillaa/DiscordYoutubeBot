using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DiscordBot
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;
        private AudioClass audioService;
        private YoutubeClassV2 tubes2;
        public AudioQueue queue;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            audioService = new AudioClass();
            tubes2 = new YoutubeClassV2();
            queue = new AudioQueue();
            services = new ServiceCollection().AddSingleton(client).AddSingleton(commands).AddSingleton(audioService).AddSingleton(tubes2).AddSingleton(queue).BuildServiceProvider();

            string token = "NDM3NDM5NzAwODM2Mjg2NDY0.Dix_YQ.uXRjqARsjvHRptzIwcttTUAt0Wo";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await installCommands();
            client.Log += Log;



            await Task.Delay(-1);
        }

        public async Task installCommands()
        {
            client.MessageReceived += HandleCommand;
            // client.UserJoined += NewUser;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task NewUser(SocketGuildUser usr)
        {
            if (usr.Id.ToString() == "185545563247345664")
            {
                await usr.KickAsync();
            }
            await usr.RemoveRolesAsync(usr.Roles);
            var role = usr.Guild.Roles.FirstOrDefault(x => x.Name == "Newbs");
            await (usr as IGuildUser).AddRoleAsync(role);
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasStringPrefix("[]", ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.HasStringPrefix("{}", ref argPos))) return;

            var context = new CommandContext(client, message);

            Console.WriteLine(message.ToString());

            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
            {
                Console.WriteLine(message.ToString());
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

