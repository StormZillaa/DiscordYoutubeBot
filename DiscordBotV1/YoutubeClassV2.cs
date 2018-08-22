using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using System.Threading.Tasks;
using NYoutubeDL;
using YoutubeSearch;
using System.Linq;
using System.IO;

namespace DiscordBot
{
    public class YoutubeClassV2
    {



        public string Search(string query, IMessageChannel channel)
        {
            int queryPages = 1;
            int queryResults = 1;
            var items = new VideoSearch();
            int x = 0;
            string url = null;
            foreach(var item in items.SearchQuery(query,queryPages))
            {
                channel.SendMessageAsync(item.Url.ToString());
                x++;
                url = item.Url.ToString();
                if (x == queryResults)
                {
                    break;
                }
            }
            return url;
        }

        public Task download(String url, IGuild guild)
        {

            var youtubeDl = new YoutubeDL();

            Console.WriteLine("Writing download location");

            youtubeDl.Options.FilesystemOptions.Output = @"C:\Users\david\source\repos\ConsoleApp2\ConsoleApp2\bin\Release\netcoreapp2.0\songs\" + guild.Id.ToString() + ".mp4";
            youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
            youtubeDl.VideoUrl = url;

            youtubeDl.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            youtubeDl.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);

            Console.WriteLine("Begining download...");

            youtubeDl.Download();

            return Task.CompletedTask;
        }

    }
}
