using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Audio;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    public class AudioModule : ModuleBase
    {

        private readonly AudioClass audio;
        private readonly YoutubeClassV2 yt2;
        private readonly AudioQueue queue;

        public AudioModule(AudioClass a, YoutubeClassV2 y, AudioQueue q)
        {
            audio = a;
            yt2 = y;
            queue = q;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task Join([Remainder]IVoiceChannel _channel = null)
        {
            _channel = _channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;

            if(_channel == null)
            {
                await Context.Channel.SendMessageAsync("User must be in a channel...");
                return;
            }
            Console.WriteLine("joining audio");
            await audio.JoinAudio(Context.Guild, _channel);

        }


        [Command("hello"), Summary("Says hello")]
        public async Task Hello()
        {
            await Context.Channel.SendMessageAsync($"Hello {Context.User.Mention}!");
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task Leave()
        {
            IAudioChannel _channel = null;
            _channel = _channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (_channel == null)
            {
                await Context.Channel.SendMessageAsync("Bot must be in a channel to leave...");
                return;
            }

            await audio.LeaveAudio(Context.Guild);

        }

        [Command("search", RunMode = RunMode.Async)]
        public async Task Search([Remainder]string Query)
        {
            string url = yt2.Search(Query, Context.Channel);
            await Context.Channel.SendMessageAsync("Search Complete...");
            await Context.Message.Channel.SendMessageAsync("Queue Length: " + queue.getLength());
            await queue.addUrl(url);

        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play()
        {
            IVoiceChannel _channel = null;
            _channel = _channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;
            await audio.queue.startQueue();
            await audio.PlayQueue(Context.Guild, Context.Channel, audio.GetPath(Context.Guild), _channel);
        }

        [Command("clear")]
        public async Task Clear()
        {
            await queue.ClearQueue();
            await Context.Message.Channel.SendMessageAsync("Queue Cleared...");
        }

        [Command("skip")]
        public async Task Skip()
        {
            IVoiceChannel _channel = null;
            _channel = _channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;
            await audio.SkipAudio(Context.Guild, Context.Message.Channel, audio.GetPath(Context.Guild), _channel);
        }

        [Command("stop")]
        public async Task Stop()
        {
            await audio.StopAudio(Context.Guild);
        }
    }
}
