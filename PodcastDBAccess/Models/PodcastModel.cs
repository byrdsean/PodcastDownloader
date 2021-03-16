using System;

namespace PodcastDBAccess.Models
{
    public class PodcastModel
    {
        public string display_name { get; set; }
        public string url { get; set; }
        public string abbreviation { get; set; }
        public string rss_url { get; set; }
        public DateTime? dateAdded { get; set; }
    }
}
