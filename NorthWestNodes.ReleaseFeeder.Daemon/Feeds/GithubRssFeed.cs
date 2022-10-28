using Newtonsoft.Json;
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
        private string previousTitle;
        private string previousUrl;

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

            if (string.IsNullOrEmpty(previousTitle))
            {
                previousTitle = latestItem.Title?.Text?.Trim();
                previousUrl = latestItem.Links.FirstOrDefault()?.Uri?.ToString()?.Trim();
                Console.WriteLine($"Github RSS feed {feed.Id}: set version to {previousTitle} at {previousUrl}");
                return;
            }

            string currentTitle = latestItem.Title?.Text?.Trim();
            string currentUrl = latestItem.Links.FirstOrDefault()?.Uri?.ToString()?.Trim();
            
            //Console.WriteLine($"Github RSS feed {feed.Id}: current version is {currentTitle}, previous version is {previousTitle}");

            if (currentTitle != previousTitle && currentUrl != previousUrl)
            {
                previousTitle = currentTitle;
                previousUrl = currentUrl;

                Console.WriteLine($"Github RSS feed {feed.Id}: new version detected, sending notification to Slack webhook");

                using (HttpClient httpClient = new HttpClient())
                {
                    SlackMessageObject githubMessageObject = new SlackMessageObject
                    {
                        Text = $"New release published: `{Name} - {currentTitle}` at {currentUrl}"
                    };

                    string slackWebhookUrl = Environment.GetEnvironmentVariable("slackWebhookUrl");
                    if (string.IsNullOrEmpty(slackWebhookUrl))
                    {
                        Console.WriteLine($"Github RSS feed {feed.Id}: Slack webhook URL is empty");
                    }

                    string githubMessageJson = JsonConvert.SerializeObject(githubMessageObject);
                    StringContent stringContent = new StringContent(githubMessageJson, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(slackWebhookUrl, stringContent);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Github RSS feed {feed.Id}: http status code {response.StatusCode} received while posting Slack message: {response.ReasonPhrase}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Github RSS feed {feed.Id}: version is still the same, {currentTitle} = {previousTitle}");
            }
        }
    }
}
