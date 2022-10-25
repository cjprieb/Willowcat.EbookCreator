using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreator.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            var options = ParseArguments(args);
            string output = null;
            if (options == null || options.ShowHelp || options.EbookCommand == EbookCommandType.Help)
            {
                output = Properties.Resources.help;
            }
            else
            {
                try
                {
                    output = RunCommand(options, output);
                }
                catch (FileNotFoundException ex)
                {
                    System.Console.Error.WriteLine($"file not found: {ex.FileName}");
                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine(ex.Message);
                    System.Console.Error.WriteLine(ex.StackTrace);
                }
            }
            System.Console.WriteLine(output ?? string.Empty);
        }

        private static string RunCommand(CommandLineOptions options, string output)
        {
            switch (options.EbookCommand)
            {
                case EbookCommandType.ReadTime:
                    var timeToReadBook = EpubUtilities.CalculateTimeToReadBook(options.EBookPath, options.ReadTimeOptions.WordsPerMinute);
                    output = Format(timeToReadBook, options.ReadTimeOptions);
                    break;

                case EbookCommandType.Identifier:
                    Dictionary<string, string> identifiers = EpubUtilities.GetIdentifiers(options.EBookPath);
                    if (identifiers != null && identifiers.Count > 0)
                    {
                        output = string.Join(",", identifiers.Select(x => $"{x.Key}:{x.Value}"));
                    }
                    break;

                case EbookCommandType.Cleanup:
                    string calibrePath = @"D:\Users\Crystal\Sync\eBooks\Calibre\";
                    EpubUtilities.Cleanup(calibrePath);
                    output = "Clean Up Finished";
                    break;
            }

            return output;
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

        public static string Format(TimeSpan? timeToReadBook, ReadTimeOptions options)
        {
            string timeString = string.Empty;
            if (timeToReadBook.HasValue && timeToReadBook.Value.TotalMinutes > 0)
            {
                TimeSpan timeSpan = timeToReadBook.Value;
                timeString = timeSpan.ToString(options.TimeFormat);

                if (options.ShowLengthIndicator)
                {
                    int totalIndicators = 0;

                    if (timeSpan.Hours >= 1)
                    {
                        totalIndicators = timeSpan.Hours * (60 / options.MinutesPerIndicator);
                    }

                    if (timeSpan.Minutes >= 1)
                    {
                        totalIndicators += (int)Math.Round((decimal)timeSpan.Minutes / options.MinutesPerIndicator, 0);
                    }


                    if (timeString.Length > 0)
                    {
                        string indicatorString = new string(options.IndicatorCharactor, totalIndicators == 0 ? 1 : totalIndicators);
                        timeString = $"{indicatorString} ({timeString})";
                    }
                }
            }
            return timeString;
        }
    }

    public class CommandLineOptions
    {
        public EbookCommandType EbookCommand { get; set; }
        public string EBookPath { get; set; }
        public ReadTimeOptions ReadTimeOptions { get; set; }
        public bool ShowHelp { get; set; }

        public CommandLineOptions(string command, Dictionary<string, string> argumentDictionary)
        {
            if (command.ToLower() == "readtime")
            {
                EbookCommand = EbookCommandType.ReadTime;
                ReadTimeOptions = new ReadTimeOptions(argumentDictionary);
            }
            else if (command.ToLower() == "identifier")
            {
                EbookCommand = EbookCommandType.Identifier;
            }
            else if (command.ToLower() == "cleanup")
            {
                EbookCommand = EbookCommandType.Cleanup;
            }

            if (argumentDictionary.ContainsKey("-h") || argumentDictionary.ContainsKey("--help"))
            {
                if (EbookCommand == EbookCommandType.None)
                {
                    EbookCommand = EbookCommandType.Help;
                }
                ShowHelp = true;
            }

            if (argumentDictionary.ContainsKey("-f"))
            {
                EBookPath = argumentDictionary["-f"];
            }
        }
    }

    public enum EbookCommandType
    {
        None,
        ReadTime,
        Identifier,
        Cleanup,
        Help
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
