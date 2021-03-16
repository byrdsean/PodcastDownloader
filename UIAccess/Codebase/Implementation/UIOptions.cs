using Global;
using UIAccess.Models;

namespace UIAccess.Codebase.Implementation
{
    public static class UIOptions
    {
        #region Options
        public static OptionModel HELP = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.help,
            Description = "Display all help options."
        };
        public static OptionModel SUBSCRIPTIONS = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.subscriptions,
            Description = "Lists all the current subscriptions."
        };
        public static OptionModel ADD_SUBSCRIPTION = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.add_sub,
            Description = "Add a subscription."
        };
        public static OptionModel DELETE_SUBSCRIPTION = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.delete_sub,
            Description = "Delete a subscription."
        };
        public static OptionModel DOWNLOAD = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.download,
            Description = "Downloads some podcast episodes."
            //download(total)(max new)
        };
        public static OptionModel UPDATE_SUBSCRIPTION = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.update_sub,
            Description = "Update the podcast episode list for a single subscription."
        };
        public static OptionModel UPDATE_SUBSCRIPTION_ALL = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.update_all_subs,
            Description = "Update the podcast episode list for all subscriptions."
        };
        public static OptionModel EXIT = new OptionModel
        {
            Option = GlobalConstants.UserSelectionOptions.exit,
            Description = "Exit the program."
        };
        #endregion

        #region Error Messages
        public static string InvalidSubscription = "Invalid or Incomplete data provided. Please provide all RSS feed data.";
        public static string FailureAddSubscription = "Unable to add new subscription.";
        public static string SubscriptionExists = "Subscription already exists.";
        public static string InvalidURL = "The url(s) provided are invalid. Please provide valid url(s).";
        public static string SubscriptionUpdateFailure = "Unable to update a subscription's episode list.";
        public static string UpdateFailInvalidAbbrev = "Unable to update a subscription's episode list. The subscription abbreviation is invalid.";
        #endregion
    }
}
