using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

namespace DiscordBot
{
   public class AudioClass
    {
            public readonly AudioQueue queue = new AudioQueue();
            private readonly YoutubeClassV2 yt = new YoutubeClassV2();    

            private ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
            private ConcurrentDictionary<ulong, AudioOutStream> AudioStreams = new ConcurrentDictionary<ulong, AudioOutStream>();

            public async Task JoinAudio(IGuild guild, IVoiceChannel target)
            {
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                Console.WriteLine("Something went wrong");
                return;
            }
            if (target.Guild.Id != guild.Id)
                {
                    Console.WriteLine("Guilds do not match.");
                    return;
                }

                var audioClient = await target.ConnectAsync();

                if (ConnectedChannels.TryAdd(guild.Id, audioClient))
                {
                    // If you add a method to log happenings from this service,
                    // you can uncomment these commented lines to make use of that.
                    Console.WriteLine($"Connected to voice on {guild.Name}.");
                }
            }

            public async Task LeaveAudio(IGuild guild)
            {
            if (ConnectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }

            public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
            {
                // Your task: Get a full path to the file if the value of 'path' is only a filename.
                if (!File.Exists(path))
                {
                    await channel.SendMessageAsync("File does not exist.");
                    return;
                }
            if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                var ffmpeg = CreateProcess(path);
                var output = ffmpeg.StandardOutput.BaseStream;
                var stream = client.CreatePCMStream(AudioApplication.Mixed);
                await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); 
                await stream.FlushAsync();
                AudioStreams.TryAdd(guild.Id, stream);
                
            }
        }

        public async Task StopAudio(IGuild guild)
        {
            if(AudioStreams.TryGetValue(guild.Id, out AudioOutStream stream))
            {
                stream.Clear();
                await queue.stopQueue();
            }
        }

        public async Task SkipAudio(IGuild guild, IMessageChannel channel, string path, IVoiceChannel voice)
        {
            await StopAudio(guild);
            await queue.removeUrl();
            await queue.startQueue();
            await PlayQueue(guild, channel, path, voice);
        }

            private Process CreateProcess(string path)
            {
                return Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                });
            }

            public async Task PlayQueue(IGuild guild, IMessageChannel channel, string path, IVoiceChannel voice)
            {
                  while (queue.getLength() >= 0)
                {
                    if(queue.stop == true)
                {
                    break;
                }
                    Console.WriteLine("Playing Next Song");
                    await yt.download(queue.getNextSong(), guild);
                    await SendAudioAsync(guild, channel, path);
                    await queue.removeUrl();
                }
            }

        public string GetPath(IGuild guild)
        {
            return @"C:\Users\david\source\repos\ConsoleApp2\ConsoleApp2\bin\Release\netcoreapp2.0\songs\" + guild.Id.ToString() + ".opus";
        }

        }
    }