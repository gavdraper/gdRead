using System;
using System.Collections.Generic;

namespace gdRead.Data.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int FeedId { get; set; }
    }
}
