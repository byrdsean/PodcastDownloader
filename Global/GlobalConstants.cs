namespace Global
{
    public class GlobalConstants
    {
        /// <summary>
        /// The different process status flags
        /// </summary>
        public enum ProcessStatus
        {
            Continue
            , InvalidSubscription
            , Exit
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Status flags for adding subscriptions
        /// </summary>
        public enum AddSubscriptionFlags
        {
            Success = 0,
            Failure = 1,
            AlreadyExists = 2,
            InvalidURL = 3,
            InvalidSubscriptionData = 4
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Status flags for updating a subscription's episode list
        /// </summary>
        public enum UpdateSubscriptionFlags
        {
            Success = 0,
            Failure = 1,
            InvalidAbbreviation = 2
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Different type of options the user can select
        /// </summary>
        public enum UserSelectionOptions
        {
            help,
            subscriptions,
            add_sub,
            delete_sub,
            download,
            update_sub,
            update_all_subs,
            exit
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
