using System.Collections.Generic;

namespace Willowcat.EbookDesktopUI.Models
{
    public class FilterModel
    {
        public string Author { get; set; }
        public HashSet<string> ExcludedTags { get; set; } = new HashSet<string>();
        public HashSet<string> IncludedTags { get; set; } = new HashSet<string>();
        public List<string> Fandoms { get; set; } = new List<string>();
    }
}
