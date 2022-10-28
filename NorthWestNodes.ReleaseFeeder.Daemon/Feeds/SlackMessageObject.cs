using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWestNodes.ReleaseFeeder.Daemon.Feeds
{
    public class SlackMessageObject
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        public SlackMessageObject()
        {
        }

        public SlackMessageObject(string text)
        {
            this.Text = text;
        }
    }
}
