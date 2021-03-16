using UIAccess.Codebase.Interfaces;
using static Global.GlobalConstants;

namespace PodcastDownloader.Codebase.Interfaces
{
    public interface IPodcastHandler
    {
        //Properties
        IUIController UIHandler { get; }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Process the option selected by the user
        /// </summary>
        /// <param name="SelectedOption"></param>
        /// <returns></returns>
        ProcessStatus? ProcessOption(string SelectedOption);
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
