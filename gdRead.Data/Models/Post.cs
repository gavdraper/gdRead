using System;

namespace gdRead.Data.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int FeedId { get; set; }
        public string Name { get; set; }
        public string  Url { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public bool Read { get; set; }        
        public DateTime DateFetched { get;set; }

        public Post()
        {
            DateFetched = DateTime.Now;
        }
    }
}
