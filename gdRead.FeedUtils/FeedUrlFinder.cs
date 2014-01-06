using System;
using System.ServiceModel.Syndication;
using System.Xml;
using HtmlAgilityPack;

namespace gdRead.FeedUtils
{
    public class FeedUrlFinder
    {
        public static string FindFeedUrl(string url)
        {
            if (!url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                url = "http://" + url;

            //Check if URL is already a valid feed
            try
            {
                using (var feedXml = XmlReader.Create(url,new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse }))
                {
                    SyndicationFeed.Load(feedXml);
                    feedXml.Close();
                }
                //No error so the url must be a valid feed
                return url;
            }
            catch
            {
                try
                {
                    var htmlLoader = new HtmlWeb();
                    var htmlDoc = htmlLoader.Load(url);
                    var rssLinks = htmlDoc.DocumentNode.SelectNodes("/html/head/link[@type='application/rss+xml']/@href");
                    if(rssLinks==null)
                        rssLinks = htmlDoc.DocumentNode.SelectNodes("/head/link[@type='application/rss+xml']/@href");
                    //For now we take the first feed
                    //TODO work out if multiple feeds which one to use. IE work out which is the real feed and which is comments
                    if (rssLinks != null || rssLinks.Count > 0)
                    {
                        return rssLinks[0].Attributes["href"].Value;
                    }
                    else return "";
                }
                catch
                {
                    return "";
                }
            }            
        }
    }
}
