using NorthWestNodes.ReleaseFeeder.Daemon.FeedManagers;
using System;
using System.Threading.Tasks;

namespace NorthWestNodes.ReleaseFeeder.Daemon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            GithubFeedManager githubFeedManager = new GithubFeedManager();
            await githubFeedManager.LoadFeeds();
            await githubFeedManager.Run();
        }
    }
}