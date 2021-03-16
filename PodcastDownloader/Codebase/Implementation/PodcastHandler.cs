using Global;
using PodcastDBAccess.Implementation;
using PodcastDBAccess.Interfaces;
using PodcastDBAccess.Models;
using PodcastDownloader.Codebase.Interfaces;
using PodcastDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UIAccess.Codebase.Implementation;
using UIAccess.Codebase.Interfaces;
using UIAccess.Models;
using static Global.GlobalConstants;
using DBAccessContants = PodcastDBAccess.Codebase.Constants;

namespace PodcastDownloader.Codebase.Implementation
{
    public class PodcastHandler : IPodcastHandler
    {
        //Variables
        private IPodcastRepo PodcastDB = null;
        
        //Properties
        public IUIController UIHandler { get; private set; }
        public IProgressBar ProgressUI { get; private set; }

        private const uint MAX_DOWNLOAD_COUNT = 50;
        private const uint LATEST_EPISODE_COUNT = 15;
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Constructor
        /// </summary>
        public PodcastHandler()
        {
            PodcastDB = new PodcastRepo();
            UIHandler = new UIController();
            ProgressUI = new ProgressBar();
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Process the option selected by the user
        /// </summary>
        /// <param name="SelectedOption"></param>
        /// <returns></returns>
        public ProcessStatus? ProcessOption(string SelectedOption)
        {
            ProcessStatus Status = ProcessStatus.Continue;

            //Parse the option
            string ParsedOption = ParsedRawUserSelection(SelectedOption);

            //Get the model for the input
            OptionModel SelectedModel = UIHandler.AllOptions
                .Where(x => x.Label.Equals(ParsedOption))
                .SingleOrDefault();

            //Process the input
            if (SelectedModel == null)
            {
                Status = ProcessStatus.Continue;
            }
            else
            {
                switch (SelectedModel.Option)
                {
                    case UserSelectionOptions.help:
                        Status = ProcessStatus.Continue;
                        break;
                    case UserSelectionOptions.subscriptions:
                        //Display a list of all the podcasts
                        DisplayAllPodcasts();
                        break;
                    case UserSelectionOptions.add_sub:
                        //Add the subscription
                        string Abbreviation = null;
                        AddSubcription(out Abbreviation, out Status);

                        //If the status is to continue, update the podcast episodes
                        if (Status == ProcessStatus.Continue)
                        {
                            UpdateSubscriptionEpisodeList(Abbreviation, out Status);

                            //Print message to user
                            if (Status == ProcessStatus.Continue)
                            {
                                Console.WriteLine("Adding subscription \"{0}\" was a success!", Abbreviation);
                            }
                            else
                            {
                                Console.Error.WriteLine("Adding subscription \"{0}\" was a failure.", Abbreviation);
                            }
                        }
                        break;
                    //case UserSelectionOptions.delete_sub:
                    //    break;
                    case UserSelectionOptions.download:
                        //Download the latest set of podcasts
                        DownloadEpisodes(out Status);

                        //If the status is to continue, update the podcast episodes
                        if (Status == ProcessStatus.Continue)
                        {
                            Console.WriteLine("\nDownload complete!");
                        }
                        else
                        {
                            Console.Error.WriteLine("\nERROR: An error occurred while downloading podcast episodes.");
                        }
                        break;
                    case UserSelectionOptions.update_sub:
                        {
                            Console.Write("Enter RSS abbreviation to update (max " + Subscription.ABBREVIATION_MAX_LENGTH + " characters):> ");
                            string Abbrev = Console.ReadLine();

                            UpdateSubscriptionEpisodeList(Abbrev, out Status);

                            //Print message to user
                            if (Status == ProcessStatus.Continue)
                            {
                                Console.WriteLine("Updating subscription \"{0}\" was a success!", Abbrev);
                            }
                            else
                            {
                                Console.Error.WriteLine("Updating subscription \"{0}\" was a failure.", Abbrev);
                            }
                        }
                        break;
                    case UserSelectionOptions.update_all_subs:
                        {
                            //Get all podcasts
                            List<PodcastListItem> AllPods = GetPodcastList();
                            foreach(var aProduct in AllPods)
                            {
                                UpdateSubscriptionEpisodeList(aProduct.abbreviation, out Status);

                                //Print message to user
                                if (Status == ProcessStatus.Continue)
                                {
                                    Console.WriteLine("Updating subscription \"{0}\" was a success!", aProduct.abbreviation);
                                }
                                else
                                {
                                    Console.Error.WriteLine("Updating subscription \"{0}\" was a failure.", aProduct.abbreviation);
                                }
                            }
                        }
                        break;
                    case UserSelectionOptions.exit:
                        Status = ProcessStatus.Exit;
                        break;
                }
            }
            return Status;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        #region Private methods
        /// <summary>
        /// Parse the raw user's selection
        /// </summary>
        /// <param name="SelectedOption"></param>
        /// <returns></returns>
        private string ParsedRawUserSelection(string SelectedOption)
        {
            //Parse the option
            string ParsedOption = (SelectedOption ?? String.Empty).Trim();

            //Check if the first two characters are OPTION_DELIM
            if (ParsedOption.IndexOf(UIHandler.OPTION_DELIM) == 0)
            {
                ParsedOption = ParsedOption.Substring(UIHandler.OPTION_DELIM.Length);
            }
            ParsedOption = ParsedOption.ToLower();

            //Return the parsed data
            return ParsedOption;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get the data needed for a new subscription
        /// </summary>
        /// <returns></returns>
        private Subscription GetNewSubscription()
        {
            Console.Write("Enter new RSS name:> ");
            string DisplayName = Console.ReadLine();

            //Console.Write("Enter website url providing RSS:> ");
            Console.Write("Enter podcast's website url:> ");
            string WebsiteUrl = Console.ReadLine();

            Console.Write("Enter new RSS abbreviation (max " + Subscription.ABBREVIATION_MAX_LENGTH + " characters):> ");
            string Abbreviation = Console.ReadLine();

            Console.Write("Enter new RSS Url:> ");
            string RssUrl = Console.ReadLine();

            //Add an extra line so that any following messages are a little more readable
            Console.WriteLine("");

            //Set the subscription model
            Subscription SubModel = new Subscription
            {
                display_name = DisplayName,
                url = WebsiteUrl,
                abbreviation = Abbreviation,
                rss_url = RssUrl
            };
            return SubModel;
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Episode Download
        /// <summary>
        /// Download the latest set of podcasts
        /// </summary>
        /// <param name="Status"></param>
        private void DownloadEpisodes(out ProcessStatus Status)
        {
            Console.Write("Enter RSS abbreviation for download:> ");
            string Abbreviation = Console.ReadLine();

            Console.Write("Enter full directory path to save episodes:> ");
            string DirectoryPath = Console.ReadLine();

            //Add an extra line so that any following messages are a little more readable
            Console.WriteLine("");

            //Download episodes
            bool Success = DownloadPodcastEpisodes(Abbreviation, DirectoryPath);
            Status = Success ? ProcessStatus.Continue : ProcessStatus.InvalidSubscription;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Download podcast episodes
        /// </summary>
        /// <param name="SubscriptionAbbrev"></param>
        /// <param name="OutputDirectory"></param>
        /// <returns></returns>
        private bool DownloadPodcastEpisodes(string SubscriptionAbbrev, string OutputDirectory)
        {
            //Check that the directory actually exists
            if (!System.IO.Directory.Exists(OutputDirectory))
            {
                return false;
            }

            bool Success = true;

            //Get the podcast data to download
            var PodcastsList = PodcastDB.GetPodcastsToDownload(SubscriptionAbbrev, MAX_DOWNLOAD_COUNT, LATEST_EPISODE_COUNT);
            if (PodcastsList != null && 0 < PodcastsList.Count)
            {
                //Display message to the console
                ProgressUI.DisplayMessage("Starting Episode download:", true);

                //Show default progress bar
                ProgressUI.ShowProgressBar((uint)0, (uint)PodcastsList.Count, true);

                //Loop through each podcast, and download
                for (int i = 0; i < PodcastsList.Count; i++)
                {
                    var aPodcast = PodcastsList[i];
                    bool DownloadSuccess = DownloadPod(aPodcast,
                        OutputDirectory,
                        new MetaData { Album = SubscriptionAbbrev.ToUpper().Trim() }
                    );
                    Success |= DownloadSuccess;
                    if (DownloadSuccess)
                    {
                        //If the donwload was a success, update the stats
                        PodcastDB.IncrementEpisodeDownloadCount(aPodcast.id);
                    }

                    //Update progress bar
                    ProgressUI.ShowProgressBar((uint)(i + 1), (uint)PodcastsList.Count, true);
                }
            }

            //Return the flag
            return Success;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Download a podcast file
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="OutputDirectory"></param>
        /// <returns></returns>
        private bool DownloadPod(PodcastDownload aPodcast, string OutputDirectory, MetaData SetMetaData)
        {
            bool Success = false;
            if (aPodcast != null)
            {
                //Remove any query params
                string Url = aPodcast.url;
                if (Url.Contains("?"))
                {
                    Url = Url.Substring(0, Url.IndexOf("?")).TrimEnd(' ', '?');
                }

                //Get the title, and turn into into a filename. Set the file name to this value
                string TitleToFilename = (aPodcast.title ?? String.Empty).Trim();
                TitleToFilename = Regex.Replace(TitleToFilename, @"[^a-zA-Z0-9\s]+", "", RegexOptions.IgnoreCase);
                TitleToFilename = TitleToFilename.Replace(" ", "-").Replace("--", "-").ToLower();
                TitleToFilename += System.IO.Path.GetExtension(Url);

                //Create the full destination path
                string DestinationPath = System.IO.Path.Combine(OutputDirectory, TitleToFilename);

                //If the destination file already exists, delete
                if (System.IO.File.Exists(DestinationPath))
                {
                    System.IO.File.Delete(DestinationPath);
                }

                //Download the file
                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(Url, DestinationPath);
                    }
                }
                catch(Exception e)
                {
                    string EMessage = String.Format("\nERROR: Unable to download \"{0}\"", Url);
                    Console.Error.WriteLine(EMessage);

                    StringBuilder ErrMsg = new StringBuilder()
                        .AppendLine(EMessage.Trim())
                        .AppendLine(e.Message)
                        .AppendLine(e.StackTrace);

                    //Store error
                    string ErrorLog = System.IO.Path.Combine(OutputDirectory, "Error.log");
                    if(System.IO.File.Exists(ErrorLog))
                    {
                        System.IO.File.AppendAllText(ErrorLog, ErrMsg.ToString());
                    }
                    else
                    {
                        System.IO.File.WriteAllText(ErrorLog, ErrMsg.ToString());
                    }

                    //If there is any file that was created, make sure to remove it
                    if(System.IO.File.Exists(DestinationPath))
                    {
                        System.IO.File.Delete(DestinationPath);
                    }
                }

                //If the file exists, set the flag
                if (System.IO.File.Exists(DestinationPath))
                {
                    Success = true;

                    //Adjust file's metadata as needed
                    if (SetMetaData != null)
                    {
                        TagLib.File TagFileObj = TagLib.File.Create(DestinationPath);

                        //Set the album
                        if (!string.IsNullOrEmpty(SetMetaData.Album) && !string.IsNullOrWhiteSpace(SetMetaData.Album))
                        {
                            TagFileObj.Tag.Album = SetMetaData.Album.Trim();
                        }

                        //Save the changes
                        TagFileObj.Save();
                    }
                }
            }
            return Success;
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Update Subscription
        /// <summary>
        /// Update a subscription's episode list
        /// </summary>
        /// <param name="SubscriptionAbbreviation"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        private void UpdateSubscriptionEpisodeList(string SubscriptionAbbreviation, out ProcessStatus Status)
        {
            //Try to update the episode list
            var SuccessAdd = UpdateSubscriptionList(SubscriptionAbbreviation);
            switch (SuccessAdd)
            {
                case UpdateSubscriptionFlags.InvalidAbbreviation:
                    Console.WriteLine("ERROR: " + UIOptions.UpdateFailInvalidAbbrev);
                    Status = ProcessStatus.InvalidSubscription;
                    break;
                case UpdateSubscriptionFlags.Failure:
                    Console.WriteLine("ERROR: " + UIOptions.SubscriptionUpdateFailure);
                    Status = ProcessStatus.InvalidSubscription;
                    break;
                case UpdateSubscriptionFlags.Success:
                    Status = ProcessStatus.Continue;
                    break;
                default:
                    Status = ProcessStatus.InvalidSubscription;
                    break;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Update a subscription's episode list
        /// </summary>
        /// <param name="SubscriptionAbbreviation"></param>
        /// <returns></returns>
        private UpdateSubscriptionFlags? UpdateSubscriptionList(string SubscriptionAbbreviation)
        {
            //Validate the abbreviation
            if (SubscriptionAbbreviation.IsEmptyNullWhitespace())
            {
                return UpdateSubscriptionFlags.InvalidAbbreviation;
            }

            //Get the subscription data for the abbreviation provided
            var SubData = PodcastDB.GetSubscriptionByAbbreviation(SubscriptionAbbreviation);
            if (SubData != null)
            {
                UpdateSubscriptionFlags UpdateFlag = UpdateSubscriptionFlags.Failure;

                //Download the rss file into an xelement obj
                //XElement EpisodeRss = XDocument.Load(SubData.RSS_Url).Root;
                XElement EpisodeRss = null;
                using (var client = new WebClient())
                {
                    string RssDataStr = client.DownloadString(SubData.RSS_Url);
                    EpisodeRss = XDocument.Parse(RssDataStr).Root;
                }
                if(EpisodeRss == null)
                {
                    return UpdateSubscriptionFlags.Failure;
                }

                //Loop through the podcast episodes, and insert into the DB as needed
                List<EpisodeModel> EpisodeItems = EpisodeRss
                    .Descendants("item")
                    .Select(x => new EpisodeModel
                    {
                        Title = x.Element("title").Value,
                        Description = x.Element("description").Value,
                        Url = x.Element("enclosure").Attribute("url").Value,
                        Publication_Date = DateTime.Parse(x.Element("pubDate").Value),
                        UniqueID = x.Element("guid").Value
                    })
                    .ToList();
                if (EpisodeItems != null && 0 < EpisodeItems.Count)
                {
                    //Update the episode list for this podcast
                    bool UpdateSuccess = PodcastDB.UpdateSubscription(SubscriptionAbbreviation, EpisodeItems);
                    UpdateFlag = UpdateSuccess
                        ? UpdateSubscriptionFlags.Success
                        : UpdateSubscriptionFlags.Failure;
                }

                //Return the update flag
                return UpdateFlag;
            }
            else
            {
                //No subscription to update
                return UpdateSubscriptionFlags.InvalidAbbreviation;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Add Subscription
        /// <summary>
        /// Add a new subscription
        /// </summary>
        /// <param name="Status"></param>
        private void AddSubcription(out string Abbreviation, out ProcessStatus Status)
        {
            //Get the data needed for a new subscription
            Subscription NewSub = GetNewSubscription();

            //Check if the subscription model is valid
            if (NewSub.IsValid())
            {
                var SuccessAdd = ProcessNewSubscription(NewSub);
                switch (SuccessAdd)
                {
                    case AddSubscriptionFlags.Success:
                        Status = ProcessStatus.Continue;
                        break;
                    case AddSubscriptionFlags.Failure:
                        Console.WriteLine("ERROR: " + UIOptions.FailureAddSubscription + $"\t\"{NewSub.display_name}\"");
                        Status = ProcessStatus.InvalidSubscription;
                        break;
                    case AddSubscriptionFlags.AlreadyExists:
                        Console.WriteLine("ERROR: " + UIOptions.SubscriptionExists);
                        Status = ProcessStatus.Continue;
                        break;
                    case AddSubscriptionFlags.InvalidSubscriptionData:
                        Console.WriteLine("ERROR: " + UIOptions.InvalidSubscription);
                        Status = ProcessStatus.InvalidSubscription;
                        break;
                    case AddSubscriptionFlags.InvalidURL:
                        Console.WriteLine("ERROR: " + UIOptions.InvalidURL);
                        Status = ProcessStatus.InvalidSubscription;
                        break;
                    default:
                        Status = ProcessStatus.Continue;
                        break;
                }

                Abbreviation = NewSub.abbreviation;
            }
            else
            {
                //Invalid or incomplete data provided. Prompt use
                Console.WriteLine("ERROR: " + UIOptions.InvalidSubscription);
                Status = ProcessStatus.InvalidSubscription;
                Abbreviation = null;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Add a new subscription.
        /// </summary>
        /// <param name="SubscriptionData"></param>
        /// <returns></returns>
        private AddSubscriptionFlags? ProcessNewSubscription(Subscription SubscriptionData)
        {
            AddSubscriptionFlags? StatusFlag = null;

            //Validate the subscription data
            if (!SubscriptionData.IsValid())
            {
                return AddSubscriptionFlags.InvalidSubscriptionData;
            }

            //Validate the urls in the subscription data
            {
                Uri ParseURL = null;

                //Parse the website url
                if (!Uri.TryCreate(SubscriptionData.url, UriKind.Absolute, out ParseURL))
                {
                    return AddSubscriptionFlags.InvalidURL;
                }

                //Parse the rss url
                if (!Uri.TryCreate(SubscriptionData.rss_url, UriKind.Absolute, out ParseURL))
                {
                    return AddSubscriptionFlags.InvalidURL;
                }
            }

            //Attempt to add the new subscription.
            //Stored procedure will check if the subscription already exists by validating the rss url
            var AddResult = PodcastDB.AddSubscription(new PodcastModel
            {
                abbreviation = SubscriptionData.abbreviation,
                display_name = SubscriptionData.display_name,
                rss_url = SubscriptionData.rss_url,
                url = SubscriptionData.url
            });

            //Set the result, and return 
            switch (AddResult)
            {
                case DBAccessContants.AddSubscriptionResult.Success:
                    StatusFlag = AddSubscriptionFlags.Success;
                    break;
                case DBAccessContants.AddSubscriptionResult.Failure:
                    StatusFlag = AddSubscriptionFlags.Failure;
                    break;
                case DBAccessContants.AddSubscriptionResult.AlreadyExists:
                    StatusFlag = AddSubscriptionFlags.AlreadyExists;
                    break;
            }
            return StatusFlag;
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Podcast list
        /// <summary>
        /// Return the list of all podcasts
        /// </summary>
        /// <returns></returns>
        private List<PodcastListItem> GetPodcastList()
        {
            var AllPods = PodcastDB.GetAllPodcastList();

            //Set the list and return
            List<PodcastListItem> PodcastList = null;
            if (AllPods != null && 0 < AllPods.Count)
            {
                PodcastList = AllPods
                    .Select(x => new PodcastListItem
                    {
                        abbreviation = x.abbreviation,
                        dateAdded = x.dateAdded,
                        display_name = x.display_name,
                        rss_url = x.rss_url,
                        url = x.url
                    })
                    .ToList();
            }
            return PodcastList;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Display all podcasts
        /// </summary>
        private void DisplayAllPodcasts()
        {
            List<PodcastListItem> AllPods = GetPodcastList();

            //Get the list for the UI
            AllPods = AllPods.OrderBy(x => x.display_name.SortValue()).ToList();

            //Display the list
            var DisplayList = AllPods.Select(x => new Podcast
                {
                    DisplayName = x.display_name,
                    Abbreviation = x.abbreviation,
                    DateAdded = x.dateAdded
                })
                .ToList();
            string Message = UIHandler.DisplayAllPodcasts(DisplayList);
            Console.WriteLine(Message);
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
