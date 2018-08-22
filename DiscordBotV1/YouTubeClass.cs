using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using Discord;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace DiscordBot
{
    public class YouTubeClass
    {
        public static int videocount;

        public YouTubeClass()
        {
            videocount = 0;
        }

        public int getVideoCount()
        {
            return videocount;
        }

        public Task ConvertWithUrl (string url, string name)
        {
            Console.WriteLine("Starting download");
            var source = @"C:\Users\david\source\repos\ConsoleApp2\ConsoleApp2\bin\Release\netcoreapp2.0\songs";
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(url);
            File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

            Console.WriteLine("Saving files");
            var inputFile = new MediaFile { Filename = source + vid.FullName };
            var outputFile = new MediaFile { Filename = $"{source + name}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }
            return Task.CompletedTask;
        }

    }
}
