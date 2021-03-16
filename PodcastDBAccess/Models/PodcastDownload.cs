using System;

namespace PodcastDBAccess.Models
{
    public class PodcastDownload
    {
        public int? id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public DateTime? publication_date { get; set; }
    }
}
