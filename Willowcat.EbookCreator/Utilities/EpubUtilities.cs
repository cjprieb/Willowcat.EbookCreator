using CsQuery;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
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

        #region AddOrSetMetadataElementInStream
        public static string AddOrSetMetadataElementInStream(MemoryStream stream, TimeSpan timeToRead)
        {
            var originalData = Encoding.UTF8.GetString(stream.ToArray());
            return AddOrSetMetadataElementToXml(originalData, timeToRead);
        }
        #endregion AddOrSetMetadataElementInStream

        #region AddOrSetMetadataElementInStream
        public static string AddOrSetMetadataElementToXml(string xmlString, TimeSpan timeToRead)
        {
            var timeToReadField = CalibreCustomFields.CreateTimeToReadField(timeToRead);
            var editor = new ContentFileCustomMetadataEditor(xmlString);
            if (editor.Version == "3.0" || editor.Version == "2.0")
            {
                editor.RemoveCustomFieldValue("#readtime");
                editor.SetCustomFieldValue(timeToReadField);
                return editor.BuildXmlString();
            }
            else
            {
                return null;
            }
        }
        #endregion AddOrSetMetadataElementInStream

        #region AddTimeToReadToContentFile
        public static bool AddTimeToReadToContentFile(string contentFilePath, TimeSpan timeToRead)
        {
            string oldOpfFileContents = File.ReadAllText(contentFilePath);
            string newOpfFileContents = AddOrSetMetadataElementToXml(oldOpfFileContents, timeToRead);
            if (!string.IsNullOrEmpty(newOpfFileContents))
            {
                File.WriteAllText(contentFilePath, newOpfFileContents);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion AddTimeToReadToContentFile

        #region AddTimeToReadToEpub
        public static bool AddTimeToReadToEpub(string epubFilePath, TimeSpan timeToRead)
        {
            string newOpfFileContents = null;
            string opfFilePath = null;
            bool fileUpdated = false;

            using (ZipFile zip = ZipFile.Read(epubFilePath))
            {
                var contentFileEntry = zip.FirstOrDefault(entry => entry.FileName.EndsWith(".opf"));
                if (contentFileEntry != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        contentFileEntry.Extract(stream);
                        newOpfFileContents = AddOrSetMetadataElementInStream(stream, timeToRead);
                        if (newOpfFileContents != null)
                        {
                            opfFilePath = contentFileEntry.FileName;
                            zip.UpdateEntry(contentFileEntry.FileName, Encoding.UTF8.GetBytes(newOpfFileContents));
                            zip.Save();
                            fileUpdated = true;
                        }
                    }
                }
            }
            return fileUpdated;
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

        #endregion Methods...
    }
}
