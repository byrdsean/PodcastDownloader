namespace PodcastDBAccess.Codebase
{
    public static class Constants
    {
        /// <summary>
        /// Status flags for adding subscriptions
        /// </summary>
        public enum AddSubscriptionResult
        {
            Success = 0,
            Failure = 1,
            AlreadyExists = 2
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
