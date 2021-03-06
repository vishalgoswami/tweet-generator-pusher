﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using PusherServer;


namespace tweet_generator_console
{
    class Program
    {
        private static string[] COMMA = { "," };

        static void Main(string[] args)
        {
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
            var pusherConfig = new PusherConfig
            {
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
        }


        private void Stream_FilteredStreamExample(string trackersKeywords, PusherConfig pusherConfig)
        {
            var trackersKeywordsList = trackersKeywords.Split(COMMA, StringSplitOptions.RemoveEmptyEntries).ToList();
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
