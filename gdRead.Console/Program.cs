using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using gdRead.Data;
using gdRead.Data.Models;

namespace gdRead.Console
{
    class Program
    {
        private static void addDemoSubs()
        {            
            if(ctx.Feeds.Count()==0)
            {

                ctx.Feeds.Add(new Feed()
                {
                    Title = "Gav Blog",
                    Url = "http://www.gavindraper.com"
                });
                ctx.SaveChanges();
            }
        }

        private static string buildFeedUrl(string url)
        {
            if (url.EndsWith("feed/", StringComparison.CurrentCultureIgnoreCase) || url.EndsWith("feed", StringComparison.CurrentCultureIgnoreCase))
                return url;
            if (!url.EndsWith("/")) 
                url += "/";
            url += "feed/";            
            return url;
        }

        private static DateTime getLastPublishDate(Feed subscription)
        {
            var ctx = new gdReadContext();
            var lastPost = (from post in ctx.Posts orderby post.PublishDate descending select post).FirstOrDefault();
            if (lastPost == null)
                return DateTime.MinValue;
            return lastPost.PublishDate;
        }

        static gdReadContext ctx = new gdReadContext();

        static void Main(string[] args)
        {
            addDemoSubs();
            
            foreach (var feed in ctx.Feeds.ToList())
            {
                SyndicationFeed rssFeed;
                using (var feedXml = XmlReader.Create(buildFeedUrl(feed.Url), new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
                {
                    rssFeed = SyndicationFeed.Load(feedXml);
                    feedXml.Close();
                }

                var lastPublishDate = getLastPublishDate(feed);

                foreach (var post in rssFeed.Items.Where(x=>x.PublishDate.UtcDateTime >= lastPublishDate))
                {

                    var _post = new Post();
                    _post.Url = post.Links.FirstOrDefault().Uri.ToString();
                    _post.Name = post.Title.Text;
                    _post.Summary = post.Summary.Text;

                    //get the encoded content or content
                    if (post.Content != null)
                        _post.Content = post.Content.ToString();
                    else
                    {
                        foreach (var ext in post.ElementExtensions)
                        {
                            var ele = ext.GetObject<XElement>();
                            if (ele.Name.LocalName == "encoded" && ele.Name.Namespace.ToString().Contains("content"))
                            {
                                _post.Content = ele.Value;
                                break;
                            }
                        }
                    }

                    _post.Feed = feed;
                    _post.PublishDate = post.PublishDate.UtcDateTime;

                    
                    ctx.Posts.Add(_post);
                    ctx.SaveChanges();
                }
            }

        }
    }
}
