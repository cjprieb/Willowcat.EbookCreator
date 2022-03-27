using CsQuery;
using Ionic.Zip;
using System.IO;
using System.Linq;
using System.Text;

namespace Willowcat.EbookCreator.Utilities
{
    public class EPubWordCountCalculator
    {
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
                foreach (ZipEntry e in zip.Where(entry => entry.FileName.EndsWith("html") || entry.FileName.EndsWith("htm")))
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

    }
}
