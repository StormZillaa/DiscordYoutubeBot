using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class AudioQueue
    {
        private static List<string> urls;

        public bool stop;

        public AudioQueue()
        {
            urls = new List<string>();
            stop = false;
        }

        public Task addUrl(string url)
        {
            int tot = urls.Count;
            urls.Add(url);

            return Task.CompletedTask;
        }

        public Task removeUrl()
        {
            urls.RemoveAt(1);

            return Task.CompletedTask;
        }

        public string getNextSong()
        {
            return urls[0];
        }

        public int getLength()
        {
            return urls.Count;
        }
        public Task ClearQueue()
        {
            urls.RemoveRange(0, urls.Count);
            return Task.CompletedTask;
        }
        public Task stopQueue()
        {
            stop = true;
            return Task.CompletedTask;
        }
        public Task startQueue()
        {
            stop = false;
            return Task.CompletedTask;
        }
    }
}
