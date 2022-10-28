using Newtonsoft.Json;
using NorthWestNodes.ReleaseFeeder.Daemon.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NorthWestNodes.ReleaseFeeder.Daemon.Feeds
{
    public class GithubRssFeed : BaseRssFeed
    {
        private string previousVersion;
        private string previousReleaseUrl;

        public GithubRssFeed(string name, string url) : base(name, url)
        {
        }

        public override async Task Parse(SyndicationFeed feed)
        {
            if (feed.Items.Count() == 0)
            {
                Console.WriteLine($"no items in Github RSS feed {feed.Id}");
            }

            var orderedItems = feed.Items.OrderByDescending(x => x.LastUpdatedTime);
            var latestItem = orderedItems.FirstOrDefault();

            if (string.IsNullOrEmpty(previousVersion))
            {
                previousVersion = latestItem.Title?.Text?.Trim();
                previousReleaseUrl = latestItem.Links.FirstOrDefault()?.Uri?.ToString()?.Trim();
                Console.WriteLine($"Github RSS feed {feed.Id}: set version to {previousVersion} at {previousReleaseUrl}");
                return;
            }

            string currentVersion = latestItem.Title?.Text?.Trim();
            string currentReleaseUrl = latestItem.Links.FirstOrDefault()?.Uri?.ToString()?.Trim();

            if (currentVersion != previousVersion && currentReleaseUrl != previousReleaseUrl)
            {
                previousVersion = currentVersion;
                previousReleaseUrl = currentReleaseUrl;

                Console.WriteLine($"Github RSS feed {feed.Id}: new version detected, sending notifications");

                SlackWebhookSink slackWebhookSink = new SlackWebhookSink();
                await slackWebhookSink.Dispatch(Name, currentReleaseUrl, currentVersion);
            }
            else
            {
                Console.WriteLine($"Github RSS feed {feed.Id}: version is still the same, {currentVersion} = {previousVersion}");
            }
        }
    }
}
