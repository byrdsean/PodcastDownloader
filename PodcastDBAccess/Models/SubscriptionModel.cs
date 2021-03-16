using System;

namespace PodcastDBAccess.Models
{
    public class SubscriptionModel
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string Abbreviation { get; set; }
        public DateTime? DateAdded { get; set; }
        public string RSS_Url { get; set; }
    }
}
