using System;
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
        const string TWITTERAPPACCESSTOKEN = "2533077650-zDmGWzt67pg5QwF4XcPFuUnuumkqro4VfDUkQOA";
        const string TWITTERAPPACCESSTOKENSECRET = "EbndY8ZyPqHsca8zOacWDGG4HNJdOefsQTDGTaH51ekZO";
        const string TWITTERAPPAPIKEY = "BDTfudG6Oag6BKfb18AQK8k5e";
        const string TWITTERAPPAPISECRET = "puHeOlNZFFqVj86hdVbQYyOSLfJyerJ1zSEfLLGRp5bGi6Tt4F";

        
        static void Main(string[] args)
        {
            Auth.SetUserCredentials(TWITTERAPPAPIKEY, TWITTERAPPAPISECRET, TWITTERAPPACCESSTOKEN, TWITTERAPPACCESSTOKENSECRET);
            Auth.ApplicationCredentials = new TwitterCredentials(TWITTERAPPAPIKEY, TWITTERAPPAPISECRET, TWITTERAPPACCESSTOKEN, TWITTERAPPACCESSTOKENSECRET);
            Program p = new Program();
            p.Stream_FilteredStreamExample();
        }

      
        private void Stream_FilteredStreamExample()
        {
          
            for (; ; )
            {
                try
                {
                    var stream = Stream.CreateFilteredStream();
                    //stream.AddLocation(Tweetinvi.Geo.GenerateLocation(-180, -90, 180, 90));
                    stream.AddTrack("#fhfoofighterstwitterdemo");
                    stream.AddTrack("#fhlivetwitter");
                    stream.AddTrack("#fhtrueiq");
                    stream.AddTrack("#trueiq");
                    stream.AddTrack("#foofighters");

                    var tweetCount = 0;
                    var timer = Stopwatch.StartNew();

                    // Generate credentials that we want to use
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\tTweet to #fhfoofighterstwitterdemo, #fhlivetwitter, #fhtrueiq,  #trueiq, #foofighters");

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
                            var pusher = new Pusher("146361", "8acc66b3c701002c5458", "f4ff5cff6f23beea51ef", new PusherOptions() { Encrypted = true });
                            var result = pusher.Trigger("trueiq-channel-1", "twitter-event", tweet);
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
