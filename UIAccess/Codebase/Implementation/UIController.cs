using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UIAccess.Codebase.Interfaces;
using UIAccess.Models;

namespace UIAccess.Codebase.Implementation
{
    public class UIController : IUIController
    {
        //Properties
        public List<OptionModel> AllOptions { get; private set; }
        public string OPTION_DELIM { get { return "-"; } }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Constructor 
        /// </summary>
        public UIController()
        {
            AllOptions = new OptionModel[]
            {
                UIOptions.HELP,
                UIOptions.SUBSCRIPTIONS,
                UIOptions.ADD_SUBSCRIPTION,
                //UIOptions.DELETE_SUBSCRIPTION,
                UIOptions.DOWNLOAD,
                UIOptions.UPDATE_SUBSCRIPTION,
                UIOptions.UPDATE_SUBSCRIPTION_ALL,
                UIOptions.EXIT
            }
            .OrderBy(x => Regex.Replace(x.Label, "[^a-zA-Z0-9]+", string.Empty, RegexOptions.IgnoreCase).ToLower())
            .ToList();
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        #region Public methods
        /// <summary>
        /// Display all possible options available to the user
        /// </summary>
        /// <returns></returns>
        public string DisplayOptions()
        {
            //Create a message to the user of all possible options
            StringBuilder Message = new StringBuilder();
            for (int i = 0; i < AllOptions.Count; i++)
            {
                //Build the message
                var aOption = AllOptions[i];
                Message
                    .Append(OPTION_DELIM).AppendLine(aOption.Label)
                    .Append("\t").AppendLine(aOption.Description);
            }
            return Message.ToString();
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Header message for help options
        /// </summary>
        /// <returns></returns>
        public string HelpHeader()
        {
            int CharLength = 40;
            string Label = "HELP OPTIONS";

            int PadLeft = (CharLength - Label.Length) / 2;

            StringBuilder Header = new StringBuilder()
                .AppendLine("".PadRight(CharLength, '#'))
                .AppendLine(Label.PadLeft(Label.Length + PadLeft, ' '))
                .AppendLine("".PadRight(CharLength, '#'));
            return Header.ToString();
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Display all podcasts
        /// </summary>
        /// <param name="PodcastList"></param>
        /// <returns></returns>
        public string DisplayAllPodcasts(List<Podcast> PodcastList)
        {
            StringBuilder Message = new StringBuilder();

            //Set the header
            Message
                .AppendLine("\n\n\tDisplayName\tAbbreviation\tDateAdded")
                .AppendLine("".PadRight(60, '-'));

            //Display the list of podcasts
            for(int i=0; i< PodcastList.Count; i++)
            {
                var aPodcast = PodcastList[i];
                Message
                    .Append(i + 1)
                    .Append(".")
                    .Append("\t")
                    .Append(aPodcast.DisplayName)
                    .Append("\t")
                    .Append(aPodcast.Abbreviation)
                    .Append("\t")
                    .AppendLine(aPodcast.DateAdded.HasValue
                        ? aPodcast.DateAdded.Value.ToString("MM/dd/yyyy")
                        : "");
            }
            return Message.ToString();
        }
        //-----------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
