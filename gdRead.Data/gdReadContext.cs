using System.Data.Entity;
using gdRead.Data.Models;

namespace gdRead.Data
{
    public class gdReadContext : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}

