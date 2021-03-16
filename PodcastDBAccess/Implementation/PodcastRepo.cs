using Dapper;
using PodcastDBAccess.Interfaces;
using PodcastDBAccess.Models;
using PodcastDBAccess.Models.DBResults;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UIAccess.Codebase.Implementation;
using UIAccess.Codebase.Interfaces;
using static PodcastDBAccess.Codebase.Constants;

namespace PodcastDBAccess.Implementation
{
    public class PodcastRepo : IPodcastRepo
    {
        //Variables
        private string PodcastDownloader_Conn = null;
        private IProgressBar UpdateProgress = null;
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Constructor
        /// </summary>
        public PodcastRepo()
        {
            PodcastDownloader_Conn = System.Configuration.ConfigurationManager.ConnectionStrings["PodcastDownloader"].ConnectionString;
            UpdateProgress = new ProgressBar();
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Add a subscription. Return status flag
        /// </summary>
        /// <param name="SubscriptionData"></param>
        /// <returns></returns>
        public AddSubscriptionResult AddSubscription(PodcastModel SubscriptionData)
        {
            AddSubscriptionResult AddResult = AddSubscriptionResult.Failure;
            using (var Conn = new SqlConnection(PodcastDownloader_Conn))
            {
                var Params = new
                {
                    DisplayName = SubscriptionData.display_name,
                    Url = SubscriptionData.url,
                    Abbreviation = SubscriptionData.abbreviation,
                    RSS_Url = SubscriptionData.rss_url
                };
                var Result = Conn.Query<AddSubResult>("usp_AddNewSubscription", Params, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
                if (Result != null)
                {
                    switch(Result.StatusCode)
                    {
                        case 0:
                            AddResult = AddSubscriptionResult.Success;
                            break;
                        case 1:
                            AddResult = AddSubscriptionResult.Failure;
                            break;
                        case 2:
                            AddResult = AddSubscriptionResult.AlreadyExists;
                            break;
                    }
                }
            }
            return AddResult;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get the subscription data for a given abbreviation
        /// </summary>
        /// <param name="Abbreviation"></param>
        /// <returns></returns>
        public SubscriptionModel GetSubscriptionByAbbreviation(string Abbreviation)
        {
            SubscriptionModel SubData = null;
            using (var Conn = new SqlConnection(PodcastDownloader_Conn))
            {
                var Params = new
                {
                    Abbreviation = Abbreviation
                };
                var Result = Conn.Query<SubscriptionModel>("usp_GetSubscriptionByAbbrev", Params, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
                if (Result != null)
                {
                    SubData = Result;
                }
            }
            return SubData;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Update the episode list for a subscription's abbreviation
        /// </summary>
        /// <param name="SubscriptionAbbrev"></param>
        /// <param name="EpisodeList"></param>
        /// <param name="UpdateProgress"></param>
        public bool UpdateSubscription(string SubscriptionAbbrev, List<EpisodeModel> EpisodeList)
        {
            bool UpdateStatus = false;

            //Check if the data is valid
            bool ValidData =
                   !string.IsNullOrEmpty(SubscriptionAbbrev)
                && !string.IsNullOrWhiteSpace(SubscriptionAbbrev)
                && EpisodeList != null
                && 0 < EpisodeList.Count;

            //Update the data
            if (ValidData)
            {
                using (var Conn = new SqlConnection(PodcastDownloader_Conn))
                {
                    //Display message to the console
                    UpdateProgress.DisplayMessage(string.Format("Starting \"{0}\" Episode update:", SubscriptionAbbrev), true);

                    //Insert / update the data
                    int Successes = 0;
                    foreach (var aEpisode in EpisodeList)
                    {
                        //Build the param model
                        var Params = new
                        {
                            title = aEpisode.Title,
                            description = aEpisode.Description,
                            url = aEpisode.Url,
                            publication_date = aEpisode.Publication_Date,
                            uniqueID = aEpisode.UniqueID,
                            abbreviation = SubscriptionAbbrev
                        };

                        //Run the sproc
                        var Result = Conn.Query<UpdateEpisodeResult>("usp_InsertUpdatePodcastEpisode", Params, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
                        if (Result != null && Result.UpdateSuccess.HasValue && Result.UpdateSuccess.Value)
                        {
                            Successes++;
                        }

                        //Update progress bar
                        UpdateProgress.ShowProgressBar((uint)Successes, (uint)EpisodeList.Count, true);
                    }
                    UpdateProgress.DisplayMessage(string.Format("\nFinished \"{0}\"Episode update.\n", SubscriptionAbbrev), false);

                    //Update the flag
                    UpdateStatus = Successes == EpisodeList.Count;
                }
            }

            //Return the status flag
            return UpdateStatus;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get a list of all podcasts
        /// </summary>
        /// <returns></returns>
        public List<PodcastModel> GetAllPodcastList()
        {
            List<PodcastModel> AllPodcasts = null;
            using (var Conn = new SqlConnection(PodcastDownloader_Conn))
            {
                //Run the sproc
                var Results = Conn.Query<PodcastModel>("usp_GetAllPodcasts", null, commandType: System.Data.CommandType.StoredProcedure);
                if (Results != null && 0 < Results.Count())
                {
                    AllPodcasts = Results.ToList();
                }
            }
            return AllPodcasts;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get a list of podcasts to download
        /// </summary>
        /// <param name="Abbreviation"></param>
        /// <param name="MaxDownloadCount"></param>
        /// <param name="LatestEpisodeCount"></param>
        /// <returns></returns>
        public List<PodcastDownload> GetPodcastsToDownload(string Abbreviation, uint MaxDownloadCount, uint LatestEpisodeCount)
        {
            List<PodcastDownload> DownloadList = null;
            using (var Conn = new SqlConnection(PodcastDownloader_Conn))
            {
                //Params
                var Params = new
                {
                    Abbreviation = Abbreviation,
                    MaxReturn = (long)MaxDownloadCount,
                    LatestEpisodes = (long)LatestEpisodeCount
                };

                //Run the sproc
                var Results = Conn.Query<PodcastDownload>("usp_GetPodcastDownloadData", Params, commandType: System.Data.CommandType.StoredProcedure);
                if (Results != null && 0 < Results.Count())
                {
                    DownloadList = Results.ToList();
                }
            }
            return DownloadList;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Increment an episode's download count
        /// </summary>
        /// <param name="EpisodeID"></param>
        public void IncrementEpisodeDownloadCount(int? EpisodeID)
        {
            if(!EpisodeID.HasValue)
            {
                return;
            }

            using (var Conn = new SqlConnection(PodcastDownloader_Conn))
            {
                //Params
                var Params = new
                {
                    EpisodeID = EpisodeID.Value
                };

                //Run the sproc
                Conn.Execute("usp_IncrementEpisodeDownloadCount", Params, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
