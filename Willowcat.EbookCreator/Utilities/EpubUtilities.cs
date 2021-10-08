using CsQuery;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Willowcat.EbookCreator.Utilities
{
    public class EpubUtilities
    {
        #region Member Variables...

        private const string opfNamespaceUrl = "http://www.idpf.org/2007/opf";
        private static readonly XNamespace _OpfNamespace = opfNamespaceUrl;

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

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
