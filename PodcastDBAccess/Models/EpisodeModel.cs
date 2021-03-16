using System;

namespace PodcastDBAccess.Models
{
    public class EpisodeModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime? Publication_Date { get; set; }
        public string UniqueID { get; set; }
    }
}
