using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreator.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            var options = ParseArguments(args);
            if (options == null || options.ShowHelp)
            {
                System.Console.WriteLine(Properties.Resources.help);
            }
            else if (options.ReadTimeOptions != null)
            {
                if (File.Exists(options.EBookPath))
                {
                    var timeToReadBook = EpubUtilities.CalculateTimeToReadBook(options.EBookPath, options.ReadTimeOptions.WordsPerMinute);
                    System.Console.WriteLine(Format(timeToReadBook, options.ReadTimeOptions));
                }
            }
            else if (options.DoCleanup)
            {
                //    string[] calibrePath = new string[]
                //    {
                //        //@"D:\Users\Crystal\Sync\eBooks\Calibre\LanternWisp\Nests and Cages (705)\Nests and Cages - LanternWisp.epub",
                //        //@"D:\Users\Crystal\Sync\eBooks\Calibre\hoye\he's a killer queen, sunflower, gui (688)\he's a killer queen, sunflower, - hoye.epub",
                //        @"D:\Users\Crystal\Sync\eBooks\Calibre\mysterycyclone\Dark Matter (666)\Dark Matter - mysterycyclone.epub",
                //    };
                string calibrePath = @"D:\Users\Crystal\Sync\eBooks\Calibre\";
                EpubUtilities.Cleanup(calibrePath);
            }
        }
        
        private static CommandLineOptions ParseArguments(string[] args)
        {
            Dictionary<string, string> argumentDictionary = new Dictionary<string, string>();
            string previousKey = null;
            string command = null;

            foreach (var arg in args)
            {
                if (command == null)
                {
                    command = arg;
                }
                else if (arg.StartsWith("-"))
                {
                    previousKey = arg;
                    argumentDictionary[arg] = "";
                }
                else if (previousKey == null)
                {
                    argumentDictionary[arg] = "";
                }
                else
                {
                    argumentDictionary[previousKey] = arg;
                    previousKey = null;
                }
            }

            if (!string.IsNullOrEmpty(command))
            {
                return new CommandLineOptions(command, argumentDictionary);
            }
            else
            {
                return null;
            }
        }

        public static string Format(TimeSpan timeToReadBook, ReadTimeOptions options)
        {
            string timeString = timeToReadBook.ToString(options.TimeFormat);

            if (options.ShowLengthIndicator)
            {
                int totalIndicators = 0;

                if (timeToReadBook.Hours >= 1)
                {
                    totalIndicators = timeToReadBook.Hours * (60 / options.MinutesPerIndicator);
                }

                if (timeToReadBook.Minutes >= 1)
                {
                    totalIndicators += (int)Math.Round((decimal)timeToReadBook.Minutes / options.MinutesPerIndicator, 0);
                }


                if (timeString.Length > 0)
                {
                    string indicatorString = new string(options.IndicatorCharactor, totalIndicators == 0 ? 1 : totalIndicators);
                    return $"{indicatorString} ({timeString})";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return timeString;
            }
        }
    }

    public class CommandLineOptions
    {
        public CommandLineOptions(string command, Dictionary<string, string> argumentDictionary)
        {
            if (command.ToLower() == "readtime")
            {
                ReadTimeOptions = new ReadTimeOptions(argumentDictionary);
            }
            else if (command.ToLower() == "cleanup")
            {
                DoCleanup = true;
            }

            if (argumentDictionary.ContainsKey("-h") || argumentDictionary.ContainsKey("--help"))
            {
                ShowHelp = true;
            }

            if (argumentDictionary.ContainsKey("-f"))
            {
                EBookPath = argumentDictionary["-f"];
            }
        }

        public bool DoCleanup { get; set; }
        public string EBookPath { get; set; }
        public ReadTimeOptions ReadTimeOptions { get; set; }
        public bool ShowHelp { get; set; }
    }

    public class ReadTimeOptions
    {
        public string TimeFormat { get; set; } = @"hh\h\ mm\m";
        public bool ShowLengthIndicator { get; set; } = true;
        public char IndicatorCharactor { get; set; } = '*';
        public int MinutesPerIndicator { get; set; } = 30;
        public int WordsPerMinute { get; set; } = 250;

        public ReadTimeOptions(Dictionary<string, string> argumentDictionary)
        {
            if (argumentDictionary.ContainsKey("-t"))
            {
                TimeFormat = argumentDictionary["-t"];
            }

            if (argumentDictionary.ContainsKey("-w"))
            {
                WordsPerMinute = int.Parse(argumentDictionary["-w"]);
            }
        }
    }
}
