using CsQuery;
using System;
using System.Linq;
using System.Text;

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

        public string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            var blockquotes = _Document["blockquote"];
            var addedSummary = false;
            var addedNotes = false;
            foreach (var block in blockquotes)
            {
                var previousElement = block.PreviousElementSibling;
                if (previousElement.InnerText == "Summary")
                {
                    description.AppendLine($"<p>Summary</p>");
                    description.AppendLine($"<blockquote>{block.InnerText}</blockquote>");
                    //description.Append(previousElement.OuterHTML).Append(block.OuterHTML);
                    addedSummary = true;
                }
                else if (previousElement.InnerText == "Notes")
                {
                    description.AppendLine($"<p>Notes</p>");
                    description.AppendLine($"<blockquote>{block.InnerText}</blockquote>");
                    //description.Append(previousElement.OuterHTML).Append(block.OuterHTML);
                    addedNotes = true;
                }

                if (addedSummary && addedNotes) break;
            }
            return description.ToString();
        }
    }
}
