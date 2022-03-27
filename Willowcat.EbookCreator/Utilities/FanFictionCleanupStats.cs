using CsQuery;
using System.Linq;
using System.Text.RegularExpressions;

namespace Willowcat.EbookCreator.Utilities
{
    public class FanFictionCleanupStats
    {
        public bool IsMultiworkBook { get; set; }
        public bool IsComplete { get; set; } = true;
        public bool IsFanfiction { get; set; }
        public string WorkId { get; private set; }

        public void SetFromFirstPage(string html)
        {
            CQ dom = html;

            if (!IsFanfiction)
            {
                CheckLinks(dom);
            }

            if (IsFanfiction)
            {
                CheckStats(dom);
            }
        }

        private void CheckLinks(CQ dom)
        {
            var allLinks = dom["a"];
            if (allLinks.Any())
            {
                int count = 0;
                foreach (var link in allLinks)
                {
                    var url = link.Attributes["href"];
                    if (count == 0 && url == "http://archiveofourown.org/")
                    {
                        IsFanfiction = true;
                    }
                    else if (count == 1 && IsFanfiction)
                    {
                        Match match = Regex.Match(url, @"http://archiveofourown.org/works/(\d+)");
                        if (match.Success)
                        {
                            WorkId = match.Groups[1].Value;
                        }
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
        }

        private void CheckStats(CQ dom)
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
