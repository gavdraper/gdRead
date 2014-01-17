using System;

namespace gdRead.Web.Models
{
    public class Dashboard
    {
        public int UserCount { get; set; }
        public int FeedCount { get; set; }
        public int PostCount { get; set; }
        public DateTime LastFetch { get; set; }
    }
}