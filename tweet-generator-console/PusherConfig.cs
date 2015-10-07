using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tweet_generator_console
{
    public class PusherConfig
    {
        public string AppId { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string Channel { get; set; }
        public string Event { get; set; }
    }
}
