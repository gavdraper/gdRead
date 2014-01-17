using System;

namespace gdRead.Data.Models
{
    public class Feed
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime? LastChecked { get; set; }
    }
}
