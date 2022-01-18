using CsQuery;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Willowcat.EbookCreator.Epub;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Utilities
{
    public class EpubUtilities
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region AddSubjectToContentFile
        public static bool AddSubjectToContentFile(string epubFilePath, string subject)
        {
            EpubZippedFile zipFile = new EpubZippedFile(epubFilePath);
            return zipFile.UpdateContentFile((editor) => editor.AddSubject(subject));
        }
        #endregion AddSubjectToContentFile

        #region AddTimeToReadToEpub
        public static bool AddTimeToReadToEpub(string epubFilePath, TimeSpan timeToRead)
        {
            EpubZippedFile zipFile = new EpubZippedFile(epubFilePath);
            return zipFile.UpdateContentFile((editor) =>
            {
                var timeToReadField = CalibreCustomFields.CreateTimeToReadField(timeToRead);
                var changesMade = false;
                if (editor.Version == "3.0" || editor.Version == "2.0")
                {
                    editor.RemoveCustomFieldValue("#readtime");
                    editor.SetCustomFieldValue(timeToReadField);
                    changesMade = true;
                }
                return changesMade;
            });
        }
        #endregion AddTimeToReadToEpub

        #region CalculateTimeToReadBook
        public static TimeSpan CalculateTimeToReadBook(string epubFilePath, int wordsPerMinute)
        {
            if (wordsPerMinute == 0) throw new DivideByZeroException($"{wordsPerMinute} cannot be 0");
            int minutes = GetWordCount(epubFilePath) / wordsPerMinute;
            int hours = minutes / 60;
            minutes = minutes % 60;
            return new TimeSpan(hours, minutes, 0);
        }
        #endregion CalculateTimeToReadBook

        #region Cleanup
        public static void Cleanup(string calibreLibraryPath)
        {
            Cleanup(GetEBookPaths(calibreLibraryPath));
        }
        #endregion Cleanup

        #region Cleanup
        public static void Cleanup(IEnumerable<string> workPaths)
        {
            List<string> multiworkBooks = new List<string>();
            List<string> incompleteBooks = new List<string>();
            foreach (var epubPath in workPaths)
            {
                string key = Path.GetFileNameWithoutExtension(epubPath);
                Console.WriteLine(key + "...");
                try
                {
                    var cleanupStats = GetCleanupStats(epubPath);
                    if (cleanupStats.IsFanfiction && (cleanupStats.IsMultiworkBook || !cleanupStats.IsComplete))
                    {
                        if (cleanupStats.IsMultiworkBook)
                        {
                            Console.WriteLine("   IsMultiworkBook");
                            multiworkBooks.Add(key);
                        }
                        if (!cleanupStats.IsComplete)
                        {
                            Console.WriteLine("   IsIncomplete");
                            incompleteBooks.Add(key);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error! could not read or parse {epubPath}: {ex.Message}");
                }
            }
            Console.WriteLine("Done.");
            multiworkBooks.Sort(StringComparer.CurrentCultureIgnoreCase);
            incompleteBooks.Sort(StringComparer.CurrentCultureIgnoreCase);

            File.WriteAllLines(@"D:\Users\Crystal\Downloads\multiwork.txt", multiworkBooks);
            File.WriteAllLines(@"D:\Users\Crystal\Downloads\incompleteworks.txt", incompleteBooks);
        }
        #endregion Cleanup

        #region CountWordsInHtml
        public static int CountWordsInHtml(string html)
        {
            CQ dom = html;
            // var body = dom["body"].ElementAt(0);
            var bodyText = dom.Text();
            var foundWord = false;
            var wordCount = 0;
            foreach (var c in bodyText)
            {
                if (char.IsLetterOrDigit(c))
                {
                    foundWord = true;
                }
                else if (c == '-' || c == '\'')
                {
                    // ignore
                }
                else if (foundWord)
                {
                    wordCount++;
                    foundWord = false;
                }
            }

            return wordCount;
        }
        #endregion CountWordsInHtml

        #region CountWordsInStream
        private static int CountWordsInStream(MemoryStream stream)
        {
            return CountWordsInHtml(Encoding.UTF8.GetString(stream.ToArray()));
        }
        #endregion CountWordsInStream

        #region GetEBookPaths
        private static IEnumerable<string> GetEBookPaths(string calibreLibraryPath)
        {
            foreach (string authorDirectory in Directory.EnumerateDirectories(calibreLibraryPath))
            {
                foreach (string workDirectory in Directory.EnumerateDirectories(authorDirectory))
                {
                    foreach (string epubPath in Directory.EnumerateFiles(workDirectory, "*.epub"))
                    {
                        yield return epubPath;
                    }
                }
            }
        }
        #endregion GetEBookPaths

        #region GetWordCount
        public static int GetWordCount(string epubFilePath)
        {
            int count = 0;

            using (ZipFile zip = ZipFile.Read(epubFilePath))
            {
                foreach (ZipEntry e in zip.Where(entry => entry.FileName.EndsWith("html")))
                {
                    using (var stream = new MemoryStream())
                    {
                        e.Extract(stream);
                        count += CountWordsInStream(stream);
                    }
                }
            }
            return count;
        }
        #endregion GetWordCount

        #region GetCleanupStats
        public static CleanupStats GetCleanupStats(string epubFilePath)
        {
            CleanupStats result = new CleanupStats();

            string firstPageHtml = null;
            string htmlFileNameBase = null;
            Regex htmlPattern = new Regex(@"(.+)_split_(\d+).xhtml");

            using (ZipFile zip = ZipFile.Read(epubFilePath))
            {
                foreach (ZipEntry e in zip.Where(entry => entry.FileName.EndsWith("html")))
                {
                    Match match = htmlPattern.Match(e.FileName);
                    if (match.Success)
                    {
                        string baseFileName = match.Groups[1].Value;
                        string index = match.Groups[2].Value;
                        if (index == "000")
                        {
                            using (var stream = new MemoryStream())
                            {
                                e.Extract(stream);
                                firstPageHtml = Encoding.UTF8.GetString(stream.ToArray());
                                result.SetFromFirstPage(firstPageHtml);
                            }

                            if (result.IsFanfiction)
                            {
                                if (htmlFileNameBase == null)
                                {
                                    htmlFileNameBase = baseFileName;
                                }
                                else if (htmlFileNameBase != baseFileName)
                                {
                                    result.IsMultiworkBook = true;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return result;
        }
        #endregion GetCleanupStats

        #region RemoveSubjectFromContentFile
        public static bool RemoveSubjectFromContentFile(string epubFilePath, string subject)
        {
            EpubZippedFile zipFile = new EpubZippedFile(epubFilePath);
            return zipFile.UpdateContentFile((editor) => editor.RemoveSubject(subject));
        }
        #endregion RemoveSubjectFromContentFile

        #endregion Methods...
    }

    public class CleanupStats
    {
        public bool IsMultiworkBook { get; set; }
        public bool IsComplete { get; set; } = true;
        public bool IsFanfiction { get; set; }

        public void SetFromFirstPage(string html)
        {
            CQ dom = html;

            if (!IsFanfiction)
            {
                var firstLink = dom["a"].First();
                if (firstLink.Any())
                {
                    var link = firstLink.Attr("href");
                    if (link == "http://archiveofourown.org/")
                    {
                        IsFanfiction = true;
                    }
                }
            }

            if (IsFanfiction)
            {
                var dataTitles = dom["dt"];
                foreach (var dt in dataTitles)
                {
                    if (dt.InnerText == "Stats:")
                    {
                        var dd = dt.NextElementSibling;
                        ParseStats(dd.InnerText);
                    }
                }
            }
        }

        private void ParseStats(string statsText)
        {
            Regex statsPattern = new Regex(@"Chapters:[\n\r\s]+(\d+)/([\d\?]+)");
            Match match = statsPattern.Match(statsText);
            if (match.Success)
            {
                var chaptersCompleted = match.Groups[1].Value;
                var totalChapters = match.Groups[2].Value;
                if (totalChapters != chaptersCompleted)
                {
                    IsComplete = false;
                }
            }
        }
    }
}
