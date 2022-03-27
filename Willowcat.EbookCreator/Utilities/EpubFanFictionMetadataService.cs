using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Willowcat.EbookCreator.Utilities
{
    public class EpubFanFictionMetadataService
    {
        private readonly string _CalibreLibraryPath;

        #region EpubFanFictionMetadataService
        public EpubFanFictionMetadataService(string calibreLibraryPath = null)
        {
            _CalibreLibraryPath = calibreLibraryPath;
        }
        #endregion EpubFanFictionMetadataService

        #region Cleanup
        public void Cleanup()
        {
            if (!string.IsNullOrEmpty(_CalibreLibraryPath))
            {
                CleanupWorks(GetEBookPaths(_CalibreLibraryPath));
            }
        }
        #endregion Cleanup

        #region CleanupWorks
        private void CleanupWorks(IEnumerable<string> workPaths)
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
        #endregion CleanupWorks

        #region GetCleanupStats
        public FanFictionCleanupStats GetCleanupStats(string epubFilePath)
        {
            FanFictionCleanupStats result = new FanFictionCleanupStats();

            string firstPageHtml = null;
            string htmlFileNameBase = null;
            Regex htmlPattern = new Regex(@"(.+)_split_(\d+)\.x?html");

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

        #region GetEBookPaths
        private IEnumerable<string> GetEBookPaths(string calibreLibraryPath)
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

        #region GetIdentifiers
        public Dictionary<string, string> GetIdentifiers(string epubFilePath)
        {
            Dictionary<string, string> identifiers = new Dictionary<string, string>();

            var stats = GetCleanupStats(epubFilePath);
            if (stats != null && stats.IsFanfiction)
            {
                if (!string.IsNullOrEmpty(stats.WorkId)) 
                {
                    identifiers["ao3"] = stats.WorkId;
                }

                //if (!string.IsNullOrEmpty(stats.AuthorAlias))
                //{
                //    identifiers["ao3-user"] = stats.AuthorAlias;
                //}
            }
            return identifiers;
        }
        #endregion GetIdentifiers

    }
}
