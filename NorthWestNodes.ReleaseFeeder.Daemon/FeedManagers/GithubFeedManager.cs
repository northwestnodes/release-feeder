using NorthWestNodes.ReleaseFeeder.Daemon.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWestNodes.ReleaseFeeder.Daemon.FeedManagers
{
    public class GithubFeedManager : FeedManager
    {
        public GithubFeedManager() : base("rss.github")
        {
        }

        public override void CreateFeed(string data)
        {
            string name = null;
            string url = null;

            if (data.Contains(';'))
            {
                string[] dataElements = data.Split(';');
                name = dataElements[0];
                url = dataElements[1].ToLower().Trim();
            }
            else
            {
                name = "Unspecified";
                url = data.ToLower().Trim();
            }
            

            GithubRssFeed feed = new GithubRssFeed(name, url);
            feeds.Add(feed);
        }
    }
}
