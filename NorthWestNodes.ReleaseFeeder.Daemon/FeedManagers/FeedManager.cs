using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NorthWestNodes.ReleaseFeeder.Daemon.Feeds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NorthWestNodes.ReleaseFeeder.Daemon.FeedManagers
{
    public abstract class FeedManager
    {
        protected List<IFeed> feeds = new List<IFeed>();
        private bool stopFlagRaised = false;

        public string FeedType { get; private set; }

        protected FeedManager(string feedType)
        {
            this.FeedType = feedType;
        }

        public async Task LoadFeeds()
        {
            string feedsUrl = Environment.GetEnvironmentVariable("feedsUrl");
            string feedsJson = null;
            using (HttpClient httpClient = new HttpClient())
            {
                feedsJson = await httpClient.GetStringAsync(feedsUrl);
            }

            if (string.IsNullOrEmpty(feedsJson))
            {
                Console.WriteLine("feeds JSON is empty");
            }

            JObject feedsConfigurationObj = JObject.Parse(feedsJson);
            if (feedsConfigurationObj == null)
            {
                Console.WriteLine("failed to parse feeds.json");
            }

            string[] pathSteps = FeedType.Split('.');
            if (pathSteps == null)
            {
                Console.WriteLine("path steps is null");
            }
            if (pathSteps.Length == 0)
            {
                Console.WriteLine("path steps is 0");
            }

            JToken tokenToSelect = feedsConfigurationObj;
            foreach (string step in pathSteps)
            {
                tokenToSelect = tokenToSelect[step];
                if (tokenToSelect == null)
                {
                    Console.WriteLine($"{step} is an invalid key for FeedType {FeedType}");
                }
            }

            JArray feedsJArr = tokenToSelect as JArray;
            if (feedsJArr == null)
            {
                Console.WriteLine($"selected token in {FeedType} is not an array");
            }

            foreach (JToken feedToken in feedsJArr)
            {
                string feedData = feedToken.Value<string>();
                CreateFeed(feedData);
            }
        }

        public abstract void CreateFeed(string data);

        public virtual async Task Run()
        {
            while (true)
            {
                if (stopFlagRaised)
                {
                    break;
                }

                foreach (var feed in feeds)
                {
                    if (!feed.IsRunning())
                    {
                        Console.WriteLine($"Executing feed: {feed}");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        feed.Execute();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                }

                int intervalInSeconds = Convert.ToInt32(Environment.GetEnvironmentVariable("intervalInSeconds"));
                await Task.Delay(intervalInSeconds * 1000);
            }
        }

        public void Stop()
        {
            stopFlagRaised = true;
        }
    }
}
