using PodcastDBAccess.Models;
using System.Collections.Generic;
using static PodcastDBAccess.Codebase.Constants;

namespace PodcastDBAccess.Interfaces
{
    public interface IPodcastRepo
    {
        /// <summary>
        /// Add a subscription. Return status flag
        /// </summary>
        /// <param name="SubscriptionData"></param>
        /// <returns></returns>
        AddSubscriptionResult AddSubscription(PodcastModel SubscriptionData);
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get the subscription data for a given abbreviation
        /// </summary>
        /// <param name="Abbreviation"></param>
        /// <returns></returns>
        SubscriptionModel GetSubscriptionByAbbreviation(string Abbreviation);
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Update the episode list for a subscription's abbreviation
        /// </summary>
        /// <param name="SubscriptionAbbrev"></param>
        /// <param name="EpisodeList"></param>
        /// <param name="UpdateProgress"></param>
        bool UpdateSubscription(string SubscriptionAbbrev, List<EpisodeModel> EpisodeList);
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get a list of all podcasts
        /// </summary>
        /// <returns></returns>
        List<PodcastModel> GetAllPodcastList();
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get a list of podcasts to download
        /// </summary>
        /// <param name="Abbreviation"></param>
        /// <param name="MaxDownloadCount"></param>
        /// <param name="LatestEpisodeCount"></param>
        /// <returns></returns>
        List<PodcastDownload> GetPodcastsToDownload(string Abbreviation, uint MaxDownloadCount, uint LatestEpisodeCount);
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Increment an episode's download count
        /// </summary>
        /// <param name="EpisodeID"></param>
        void IncrementEpisodeDownloadCount(int? EpisodeID);
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
