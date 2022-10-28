# NorthWestNodes.ReleaseFeeder
Checks a multitude of data sources for new releases, currently: Github RSS feeds. Work in progress: Docker Hub

## Requirements
* Cross-platform (Linux, Windows, MacOS)
* .NET Core 6
* Designed to run in a serverless environment as a background daemon/service

## ENV vars
* `feedsUrl` - a URL to a JSON file containing feeds, see feeds.json for an example
* `intervalInSeconds` - interval to check for new releases in number of seconds
* `slackWebhookUrl` - URL to your Slack webhook

## Other notes
Code is ugly. Could be better. Logging is suboptimal. Could be better.