using PodcastDownloader.Codebase.Implementation;
using PodcastDownloader.Codebase.Interfaces;
using System;
using static Global.GlobalConstants;

namespace PodcastDownloader
{
    public class PodcastBootstrap
    {
        static void Main(string[] args)
        {
            IPodcastHandler DownloadPodcast = new PodcastHandler();

            //Display the default options
            Console.Write(DownloadPodcast.UIHandler.DisplayOptions() + "\n");

            //Read in the user's selection
            ProcessStatus? Status = null;
            do
            {
                Console.Write("Please input selection:> ");

                //Read in the option
                string UserSelection = Console.ReadLine();

                //Process the request
                Status = DownloadPodcast.ProcessOption(UserSelection);
                switch (Status)
                {
                    case ProcessStatus.Continue:
                        //Show the help menu again
                        Console.WriteLine("\n\n");
                        Console.Write(DownloadPodcast.UIHandler.HelpHeader());
                        Console.WriteLine(DownloadPodcast.UIHandler.DisplayOptions());
                        break;
                    case ProcessStatus.Exit:
                        break;
                }
            }
            while (!(Status.HasValue && Status.Value == ProcessStatus.Exit));
        }
    }
}
