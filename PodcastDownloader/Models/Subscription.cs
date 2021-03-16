using Global;

namespace PodcastDownloader.Models
{
    public class Subscription
    {
        public string display_name { get; set; }
        public string url { get; set; }
        public string abbreviation { get; set; }
        public string rss_url { get; set; }

        //Getters
        public static int ABBREVIATION_MAX_LENGTH = 10;
        //-----------------------------------------------------------------------------------------------------------------------------

        public bool IsValid()
        {
            bool IsValid = !display_name.IsEmptyNullWhitespace()
                && !url.IsEmptyNullWhitespace()
                && !abbreviation.IsEmptyNullWhitespace()
                && !rss_url.IsEmptyNullWhitespace()

                //Check the abbreviation length
                && abbreviation.Length <= ABBREVIATION_MAX_LENGTH;
            return IsValid;
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
