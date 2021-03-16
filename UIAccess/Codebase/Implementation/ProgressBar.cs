using System;
using UIAccess.Codebase.Interfaces;

namespace UIAccess.Codebase.Implementation
{
    public class ProgressBar : IProgressBar
    {
        //Variables
        private const int MAX_BLOCKS = 50;
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Display any messages
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="DisplayHR"></param>
        public void DisplayMessage(string Message, bool DisplayHR)
        {
            if (!string.IsNullOrEmpty(Message) && !string.IsNullOrWhiteSpace(Message))
            {
                Console.WriteLine(Message);

                //Display the horizontal rule if needed
                if (DisplayHR)
                {
                    Console.WriteLine("".PadRight(10, '-'));
                }
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Display the progress bar
        /// </summary>
        /// <param name="StepsCompleted"></param>
        /// <param name="MaxSteps"></param>
        /// <param name="ShowPercentComplete"></param>
        public void ShowProgressBar(uint StepsCompleted, uint MaxSteps, bool ShowPercentComplete)
        {
            if (0 <= StepsCompleted && 0 < MaxSteps)
            {
                //Calculate the percentage complete
                double PercentComplete = (StepsCompleted / (MaxSteps * 1.0)) * 100;

                //Make sure "PercentComplete" is non-negative, and not greater than 100
                if (PercentComplete < 0)
                {
                    PercentComplete = 0;
                }
                else if (100 < PercentComplete)
                {
                    PercentComplete = 100;
                }

                //Determine how many blocks need to represent the completion value
                int CompleteBlocks = Convert.ToInt32(Math.Floor(MAX_BLOCKS * (PercentComplete / 100)));

                //Remember the original background color so we can reset it later
                var OriginalColor = Console.BackgroundColor;

                //Print out the bar
                Console.Write("\r[");
                Console.BackgroundColor = ConsoleColor.Blue;
                //Console.BackgroundColor = ConsoleColor.White;
                Console.Write("".PadRight(CompleteBlocks, ' '));

                //Print out the remaining spaces
                Console.BackgroundColor = OriginalColor;
                Console.Write("".PadRight((MAX_BLOCKS - CompleteBlocks), '.'));
                Console.Write("]");

                //Print out the complete value if needed
                if (ShowPercentComplete)
                {
                    string PercentStr = PercentComplete.ToString("00.0");
                    if(PercentStr[0] == '0')
                    {
                        PercentStr.Remove(0, 1);
                        PercentStr.Insert(0, " ");
                    }
                    Console.Write(" {0}% Complete", PercentStr);
                }
            }
            else
            {
                Console.Error.WriteLine("ERROR: Invalid data used to display progress bar. Aborting progress bar.");
            }
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
