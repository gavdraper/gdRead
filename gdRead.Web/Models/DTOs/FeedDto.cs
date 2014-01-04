using gdRead.Data.Models;

namespace gdRead.Web.Models.DTOs
{
    public class FeedDto : Feed
    {
        public bool UnreadItems { get; set; }
    }
}