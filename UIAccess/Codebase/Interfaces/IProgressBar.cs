namespace UIAccess.Codebase.Interfaces
{
    public interface IProgressBar
    {
        /// <summary>
        /// Display any messages
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="DisplayHR"></param>
        void DisplayMessage(string Message, bool DisplayHR);
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Display the progress bar
        /// </summary>
        /// <param name="StepsCompleted"></param>
        /// <param name="MaxSteps"></param>
        /// <param name="ShowPercentComplete"></param>
        void ShowProgressBar(uint StepsCompleted, uint MaxSteps, bool ShowPercentComplete);
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
