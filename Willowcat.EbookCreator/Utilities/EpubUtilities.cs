using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            using (EpubZippedFile zipFile = new EpubZippedFile(epubFilePath))
            {
                return zipFile.UpdateContentFile((editor) => editor.AddSubject(subject));
            }
        }
        #endregion AddSubjectToContentFile

        #region AddTimeToReadToEpub
        public static bool AddTimeToReadToEpub(string epubFilePath, TimeSpan? timeToRead)
        {
            if (timeToRead.HasValue)
            {
                using (EpubZippedFile zipFile = new EpubZippedFile(epubFilePath))
                {
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
            }
            return false;
        }
        #endregion AddTimeToReadToEpub

        #region CalculateTimeToReadBook
        public static TimeSpan? CalculateTimeToReadBook(string epubFilePath, int wordsPerMinute)
        {
            if (!File.Exists(epubFilePath)) throw new FileNotFoundException($"File does not exist: {epubFilePath}");

            if (wordsPerMinute == 0) throw new DivideByZeroException($"{wordsPerMinute} cannot be 0");

            int wordCount = EPubWordCountCalculator.GetWordCount(epubFilePath);
            int totalMinutes = wordCount / wordsPerMinute;
            return new TimeSpan(totalMinutes / 60, totalMinutes % 60, 0);
        }
        #endregion CalculateTimeToReadBook

        #region Cleanup
        public static void Cleanup(string calibreLibraryPath)
        {
            EpubFanFictionMetadataService service = new EpubFanFictionMetadataService(calibreLibraryPath);
            service.Cleanup();
        }
        #endregion Cleanup

        #region GetIdentifiers
        public static Dictionary<string, string> GetIdentifiers(string eBookPath)
        {
            EpubFanFictionMetadataService service = new EpubFanFictionMetadataService();
            return service.GetIdentifiers(eBookPath);
        }
        #endregion GetIdentifiers

        #region RemoveSubjectFromContentFile
        public static bool RemoveSubjectFromContentFile(string epubFilePath, string subject)
        {
            using (EpubZippedFile zipFile = new EpubZippedFile(epubFilePath))
            {
                return zipFile.UpdateContentFile((editor) => editor.RemoveSubject(subject));
            }
        }
        #endregion RemoveSubjectFromContentFile

        #region SearchHtmlContent
        public static bool SearchHtmlContentFiles(string epubFilePath, string keyword)
        {
            bool found = false;
            using (EpubZippedFile zipFile = new EpubZippedFile(epubFilePath))
            {
                zipFile.ProcessChapterFiles((stream) =>
                {
                    if (!found)
                    {
                        string html = Encoding.UTF8.GetString(stream.ToArray());
                        found = SearchHtmlFile(html, keyword);
                    }
                });
            }
            return found;
        }
        #endregion SearchHtmlContent

        #region SearchHtmlContent
        public static bool SearchHtmlFile(string html, string keyword)
        {
            bool found = false;
            //CQ dom = html;
            //string text = dom["body"].Text();
            found = html.Contains(keyword, StringComparison.OrdinalIgnoreCase);
            return found;
        }
        #endregion SearchHtmlContent

        #endregion Methods...
    }
}
