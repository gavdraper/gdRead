using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using gdRead.Data.Models;
using gdRead.Data.Repositories;

namespace gdRead.FeedUtils
{
    public class Fetcher
    {

        private readonly string _conStr;
        public Fetcher(string conStr)
        {
            _conStr = conStr;
        }

        private DateTime getLastPublishDate(Feed feed)
        {
            var postRepository = new PostRepository(_conStr);
            return postRepository.GetLastPostDateInFeed(feed.Id);
        }

        public void FetchFeed(int feedId)
        {
            try
            {
                var feedRepository = new FeedRepository(_conStr);
                var postRepository = new PostRepository(_conStr);
                var feed = feedRepository.GetFeedById(feedId);
                if (feed.LastChecked != null && feed.LastChecked.Value > DateTime.Now.AddMinutes(-10))
                    return;

                SyndicationFeed rssFeed;
                using (
                    var feedXml = XmlReader.Create(FeedUrlFinder.FindFeedUrl(feed.Url),
                        new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse}))
                {
                    rssFeed = SyndicationFeed.Load(feedXml);
                    feedXml.Close();
                }

                if (string.IsNullOrEmpty(feed.Title))
                {
                    if(rssFeed.Title != null)
                        feed.Title = rssFeed.Title.Text;
                    if (feed.Title == "")
                    {
                        if (rssFeed.Authors.Count > 0 && !string.IsNullOrEmpty(rssFeed.Authors[0].Name))
                        {
                            feed.Title = rssFeed.Authors[0].Name;
                        }
                        else
                            feed.Title = feed.Url.ToLower()
                                .Replace("http://", "")
                                .Replace("https://", "")
                                .Replace("www", "");
                    }


                    feedRepository.UpdateFeed(feed);
                }

                var lastPublishDate = getLastPublishDate(feed);

                foreach (var feedPost in rssFeed.Items.Where(x => (x.PublishDate.UtcDateTime == DateTime.MinValue ? x.LastUpdatedTime.UtcDateTime : x.PublishDate.UtcDateTime ) > lastPublishDate))
                {
                    var post = new Post()
                    {
                        Url = feedPost.Links.FirstOrDefault() == null ? "" : feedPost.Links.First().Uri.ToString(),
                        Name = feedPost.Title.Text,
                        Summary = feedPost.Summary != null ? feedPost.Summary.Text : "",
                        Content = feedPost.Summary != null ? feedPost.Summary.Text : "",
                        FeedId = feed.Id,
                        PublishDate = feedPost.PublishDate.UtcDateTime == DateTime.MinValue ? feedPost.LastUpdatedTime.UtcDateTime : feedPost.PublishDate.UtcDateTime,
                        Read = false
                    };

                    //get the encoded content or content
                    if (feedPost.Content != null)
                        post.Content = ((TextSyndicationContent)feedPost.Content).Text;
                    else
                    {
                        foreach (var ext in feedPost.ElementExtensions)
                        {
                            var ele = ext.GetObject<XElement>();
                            if (ele.Name.LocalName == "encoded" && ele.Name.Namespace.ToString().Contains("content"))
                            {
                                post.Content = ele.Value;
                                break;
                            }
                        }
                    }
                    postRepository.AddPost(post);
                }
                feedRepository.UpdateLastchecked(feed.Id);
            }
            catch
            {
                //TODO INSERT LOGGING CODE
            }
        }

        public void FetchAllFeeds()
        {
            var feedRepository = new FeedRepository(_conStr);
            var feeds = feedRepository.GetAllFeeds();
            Parallel.ForEach(feeds, feed => FetchFeed(feed.Id));
        }
    }
}
