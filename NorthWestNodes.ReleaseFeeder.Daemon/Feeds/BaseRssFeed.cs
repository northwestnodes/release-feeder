using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Net.Http;
using System.IO;

namespace NorthWestNodes.ReleaseFeeder.Daemon.Feeds
{
    public abstract class BaseRssFeed : IFeed
    {
        private bool isRunning = false;
        public string Url { get; private set; }
        public string Name { get; private set; }

        protected BaseRssFeed(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }

        public async Task Execute()
        {
            isRunning = true;

            using (HttpClient httpClient = new HttpClient())
            {
                string xml = await httpClient.GetStringAsync(Url);
                using (StringReader sr = new StringReader(xml))
                {
                    XmlReader reader = XmlReader.Create(sr);
                    SyndicationFeed feed = SyndicationFeed.Load(reader);

                    await Parse(feed);
                }
            }

            isRunning = false;
        }

        public abstract Task Parse(SyndicationFeed feed);

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}
