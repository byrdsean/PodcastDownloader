using System.Collections.Generic;
using UIAccess.Models;

namespace UIAccess.Codebase.Interfaces
{
    public interface IUIController
    {
        //Properties
        List<OptionModel> AllOptions { get; }
        string OPTION_DELIM { get; }
        //-----------------------------------------------------------------------------------------------------------------------------

        #region Public methods
        /// <summary>
        /// Display all possible options available to the user
        /// </summary>
        /// <returns></returns>
        string DisplayOptions();
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Header message for help options
        /// </summary>
        /// <returns></returns>
        string HelpHeader();
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Display all podcasts
        /// </summary>
        /// <param name="PodcastList"></param>
        /// <returns></returns>
        string DisplayAllPodcasts(List<Podcast> PodcastList);
        //-----------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
