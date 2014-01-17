using System;

namespace gdRead.Data.Models
{
    public class Dashboard
    {
        public int UserCount { get; set; }
        public int FeedCount { get; set; }
        public int PostCount { get; set; }
        public int SubscriptionCount { get; set; }
        public DateTime LastFetch { get; set; }
    }
}