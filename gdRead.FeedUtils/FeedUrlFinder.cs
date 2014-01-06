using System;

namespace gdRead.FeedUtils
{
    public class FeedUrlFinder
    {
        public static string FindFeedUrl(string url)
        {
            if (url.EndsWith("feed/", StringComparison.CurrentCultureIgnoreCase) ||
                url.EndsWith("feed", StringComparison.CurrentCultureIgnoreCase))
                return url;
            if (!url.EndsWith("/"))
                url += "/";
            url += "feed/";
            return url;
        }
    }
}
