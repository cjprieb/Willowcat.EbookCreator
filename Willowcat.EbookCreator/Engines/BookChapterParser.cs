using CsQuery;
using System.Linq;

namespace Willowcat.EbookCreator.Engines
{
    public class BookChapterParser
    {
        private string _FilePath = null;
        private CQ _Document = null;

        public BookChapterParser(string filePath)
        {
            _FilePath = filePath;
            _Document = CQ.CreateDocumentFromFile(filePath);
        }

        public string GetChapterTitle()
        {
            string title = null;
            var heading = _Document[".toc-heading"];

            if (heading == null || !heading.Any())
            {
                heading = _Document[".heading"];
            }

            if (heading != null && heading.Any())
            {
                title = heading.Text();
            }

            return title;
        }
    }
}
