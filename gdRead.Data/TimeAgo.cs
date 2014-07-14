using System;

namespace gdRead.Data
{
    public static class TimeAgo
    {
        public static string ToTimeAgo(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return string.Format("{0} seconds ago", timeSpan.Seconds);

            if (timeSpan <= TimeSpan.FromMinutes(60))
                return timeSpan.Minutes > 1 ? String.Format("{0} minutes ago", timeSpan.Minutes) : "a minute ago";

            if (timeSpan <= TimeSpan.FromHours(24))
                return timeSpan.Hours > 1 ? String.Format("{0} hours ago", timeSpan.Hours) : "an hour ago";

            if (timeSpan <= TimeSpan.FromDays(30))
                return timeSpan.Days > 1 ? String.Format("{0} days ago", timeSpan.Days) : "yesterday";

            if (timeSpan <= TimeSpan.FromDays(365))
                return timeSpan.Days > 30 ? String.Format("{0} months ago", timeSpan.Days / 30) : "a month ago";

            return timeSpan.Days > 365 ? String.Format("{0} years ago", timeSpan.Days / 365) : "a year ago";
        }
    }
}