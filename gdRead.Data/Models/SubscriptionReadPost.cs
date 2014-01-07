namespace gdRead.Data.Models
{
    public class SubscriptionReadPost
    {
        public int Id { get; set; }
        public virtual Subscription Subscription { get; set; }
        public virtual Post Post { get; set; }
    }
}
