using System;
using System.Linq;
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
                    feed.Title = rssFeed.Title != null ? rssFeed.Title.Text : "No Title";
                    feedRepository.UpdateFeed(feed);
                }

                var lastPublishDate = getLastPublishDate(feed);

                foreach (var feedPost in rssFeed.Items.Where(x => x.PublishDate.UtcDateTime > lastPublishDate))
                {
                    var post = new Post()
                    {
                        Url = feedPost.Links.FirstOrDefault() == null ? "" : feedPost.Links.First().Uri.ToString(),
                        Name = feedPost.Title.Text,
                        Summary = feedPost.Summary != null ? feedPost.Summary.Text : "",
                        Content = feedPost.Summary != null ? feedPost.Summary.Text : "",
                        FeedId = feed.Id,
                        PublishDate = feedPost.PublishDate.UtcDateTime,
                        Read = false
                    };

                    //get the encoded content or content
                    if (feedPost.Content != null)
                        post.Content = feedPost.Content.ToString();
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
            catch(Exception ex)
            {
                
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
