using System.Collections.Generic;
using System.Linq;
using Willowcat.Common.Utilities;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookDesktopUI.Models
{
    public enum CompletionOption
    {
        All, Incomplete, Completed
    }

    public class FilterModel
    {
        public bool DoFullTextSearch { get; set; }
        public string Author { get; set; }
        public string Keyword { get; set; }
        public HashSet<string> ExcludedTags { get; set; } = new HashSet<string>();
        public HashSet<string> IncludedTags { get; set; } = new HashSet<string>();
        public HashSet<string> Fandoms { get; set; } = new HashSet<string>();

        public ProcessTagType SelectedProcessTag { get; set; } = ProcessTagType.All;
        public CompletionOption CompletionStatus { get; internal set; }

        internal bool IsMatch(EpubDisplayModel pub)
        {
            if (!string.IsNullOrEmpty(Author))
            {
                if (pub.Author != Author) return false;
            }

            if (SelectedProcessTag == ProcessTagType.None)
            {
                if (pub.ProcessTags?.Any() ?? false) return false;
            }
            else if (SelectedProcessTag != ProcessTagType.All)
            {
                if (!pub.ProcessTags.Any(tag => tag == SelectedProcessTag)) return false;
            }

            IEnumerable<string> allPubTags = pub.AllTags;

            if (ExcludedTags != null && ExcludedTags.Any())
            {
                if (HasMatch(allPubTags, ExcludedTags, matchAll: false)) return false;
            }

            if (IncludedTags != null && IncludedTags.Any())
            {
                if (!HasMatch(allPubTags, IncludedTags, matchAll: true)) return false;
            }

            if (Fandoms != null && Fandoms.Any())
            {
                if (!HasMatch(pub.FandomTags, Fandoms, matchAll: false)) return false;
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                if (Contains(pub.Author, Keyword)) return true;
                if (Contains(pub.Title, Keyword)) return true;
                if (allPubTags.Any(tag => Contains(tag, Keyword))) return true;
                if (Contains(pub.Description, Keyword)) return true;
                if (DoFullTextSearch)
                {
                    return EpubUtilities.SearchHtmlContentFiles(pub.LocalFilePath, Keyword);
                }

                return false;
            }

            if (CompletionStatus != CompletionOption.All)
            {
                bool isPublicationComplete = pub.Statistics?.DateCompleted.HasValue ?? false;
                return (CompletionStatus == CompletionOption.Completed && isPublicationComplete)
                    || (CompletionStatus == CompletionOption.Incomplete && !isPublicationComplete);
            }

            return true;
        }

        private bool Contains(string baseString, string searchString)
        {
            return baseString?.Contains(searchString, System.StringComparison.OrdinalIgnoreCase) ?? false;
        }

        private bool HasMatch(IEnumerable<string> pubTags, IEnumerable<string> filterTags, bool matchAll)
        {
            if (matchAll)
            {
                return filterTags.All(filterTag => pubTags.Any(tag => filterTag.EqualsIgnoreCase(tag)));
            }
            else
            {
                return filterTags.Any(filterTag => pubTags.Any(tag => filterTag.EqualsIgnoreCase(tag)));
            }
        }
    }
}
