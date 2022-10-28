using Newtonsoft.Json;
using NorthWestNodes.ReleaseFeeder.Daemon.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NorthWestNodes.ReleaseFeeder.Daemon.Sinks
{
    public class SlackWebhookSink : BaseSink
    {
        public override async Task Dispatch(params string[] args)
        {
            // 0 = name
            // 1 = url
            // 2 = title/version

            if (args == null)
            {
                // panic
            }

            if (args.Length != 3)
            {
                // panic
            }

            string slackWebhookUrl = Environment.GetEnvironmentVariable("slackWebhookUrl");
            if (string.IsNullOrEmpty(slackWebhookUrl))
            {
                // panic
            }

            string name = args[0].Trim();
            string url = args[1].Trim();
            string title = args[2].Trim();

            using (HttpClient httpClient = new HttpClient())
            {
                SlackMessageObject githubMessageObject = new SlackMessageObject($"New release published: `{name} - {title}` at {url}");
                string githubMessageJson = JsonConvert.SerializeObject(githubMessageObject);

                StringContent stringContent = new StringContent(githubMessageJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(slackWebhookUrl, stringContent);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Slack Webhook Sink: http status code {response.StatusCode} received while posting Slack message: {response.ReasonPhrase}");
                }
            }
        }
    }
}
