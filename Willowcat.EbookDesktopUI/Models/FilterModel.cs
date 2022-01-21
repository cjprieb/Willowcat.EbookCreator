using System.Collections.Generic;
using System.Linq;
using Willowcat.Common.Utilities;

namespace Willowcat.EbookDesktopUI.Models
{
    public class FilterModel
    {
        public string Author { get; set; }
        public HashSet<string> ExcludedTags { get; set; } = new HashSet<string>();
        public HashSet<string> IncludedTags { get; set; } = new HashSet<string>();
        public List<string> Fandoms { get; set; } = new List<string>();

        public ProcessTagType SelectedProcessTag { get; set; } = ProcessTagType.All;

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

            return true;
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
