using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Configuration;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using System.Diagnostics;
using PusherServer;

namespace tweet_listener_webjob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static string[] COMMA = { "," };
        static void Main()
        {
            var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            var TWITTERAPPACCESSTOKEN = ConfigurationManager.AppSettings["TWITTERAPPACCESSTOKEN"];
            var TWITTERAPPACCESSTOKENSECRET = ConfigurationManager.AppSettings["TWITTERAPPACCESSTOKENSECRET"];
            var TWITTERAPPAPIKEY = ConfigurationManager.AppSettings["TWITTERAPPAPIKEY"];
            var TWITTERAPPAPISECRET = ConfigurationManager.AppSettings["TWITTERAPPAPISECRET"];
            var trackersKeywords = ConfigurationManager.AppSettings["TrackerKeywords"];
            var pusherAppId = ConfigurationManager.AppSettings["PusherAppId"];
            var pusherAppKey = ConfigurationManager.AppSettings["PusherAppId"];
            var pusherAppSecret = ConfigurationManager.AppSettings["PusherAppId"];
            var pusherChannel = ConfigurationManager.AppSettings["PusherAppId"];
            var pusherEvent = ConfigurationManager.AppSettings["PusherAppId"];
            var pusherConfig = new PusherConfig {
                 AppId = pusherAppId,
                 AppKey = pusherAppKey,
                 AppSecret = pusherAppSecret,
                 Channel = pusherChannel,
                 Event = pusherEvent
            };
            Auth.SetUserCredentials(TWITTERAPPAPIKEY, TWITTERAPPAPISECRET, TWITTERAPPACCESSTOKEN, TWITTERAPPACCESSTOKENSECRET);
            Auth.ApplicationCredentials = new TwitterCredentials(TWITTERAPPAPIKEY, TWITTERAPPAPISECRET, TWITTERAPPACCESSTOKEN, TWITTERAPPACCESSTOKENSECRET);
            Program p = new Program();
            p.Stream_FilteredStreamExample(trackersKeywords, pusherConfig);
            host.RunAndBlock();
        }

        private void Stream_FilteredStreamExample(string trackersKeywords, PusherConfig pusherConfig)
        {
            var trackersKeywordsList = trackersKeywords.Split(COMMA,StringSplitOptions.RemoveEmptyEntries).ToList();
            for (; ; )
            {
                try
                {
                    var stream = Stream.CreateFilteredStream();
                    foreach (var trackersKeyword in trackersKeywordsList)
                        stream.AddTrack(trackersKeyword);
                    var tweetCount = 0;
                    var timer = Stopwatch.StartNew();

                    // Generate credentials that we want to use
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\tTweet to {0}", string.Join(",", trackersKeywordsList.ToArray()));

                    stream.MatchingTweetReceived += (sender, args) =>
                    {
                        tweetCount++;
                        var tweet = args.Tweet;

                        // Push to Dashboard.

                        if (timer.ElapsedMilliseconds > 1000)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n{0}: {1} {2}", tweet.Id, tweet.Language.ToString(), tweet.Text);
                            Console.ForegroundColor = ConsoleColor.White;
                            if (tweet.Coordinates != null)
                                Console.WriteLine("\tLocation: {0}, {1}", tweet.Coordinates.Longitude, tweet.Coordinates.Latitude);
                            timer.Restart();
                            Console.WriteLine("\tTweets/sec: {0}", tweetCount);
                            tweetCount = 0;
                            Console.WriteLine("\tPushing to Dashboard...");
                            var pusher = new Pusher(pusherConfig.AppId, pusherConfig.AppKey, pusherConfig.AppSecret, new PusherOptions() { Encrypted = true });
                            var result = pusher.Trigger(pusherConfig.Channel, pusherConfig.Event, tweet);
                        }
                    };

                    stream.StartStreamMatchingAllConditions();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exception: {0}", ex.Message);
                }
            }
        }

    }
}
