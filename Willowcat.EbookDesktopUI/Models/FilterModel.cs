using System;
using System.Collections.Generic;
using System.Linq;

namespace Willowcat.EbookDesktopUI.Models
{
    public class FilterModel
    {
        public string Author { get; set; }
        public HashSet<string> ExcludedTags { get; set; } = new HashSet<string>();
        public HashSet<string> IncludedTags { get; set; } = new HashSet<string>();
        public List<string> Fandoms { get; set; } = new List<string>();

        internal bool IsMatch(EpubDisplayModel pub)
        {
            if (!string.IsNullOrEmpty(Author))
            {
                if (pub.Author != Author) return false;
            }

            IEnumerable<string> allPubTags = pub.AdditionalTags
                .Union(pub.CharacterTags)
                .Union(pub.RelationshipTags)
                .Union(pub.WarningTags)
                .Union(pub.FandomTags);

            if (ExcludedTags != null && ExcludedTags.Any())
            {
                if (HasMatch(allPubTags, ExcludedTags, matchAll: false)) return false;
            }

            if (IncludedTags != null && IncludedTags.Any())
            {
                if (HasMatch(allPubTags, IncludedTags, matchAll: true)) return false;
            }

            if (Fandoms != null && Fandoms.Any())
            {
                if (HasMatch(pub.FandomTags, Fandoms, matchAll: true)) return false;
            }

            return true;
        }

        private bool HasMatch(IEnumerable<string> pubTags, IEnumerable<string> filterTags, bool matchAll)
        {
            if (matchAll)
            {
                return filterTags.All(filterTag => pubTags.Any(tag => filterTag == tag));
            }
            else
            {
                return filterTags.Any(filterTag => pubTags.Any(tag => filterTag == tag));
            }
        }
    }
}
