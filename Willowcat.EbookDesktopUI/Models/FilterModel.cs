using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Willowcat.EbookDesktopUI.Models
{
    public class FilterModel
    {
        public HashSet<string> ExcludedTags { get; set; } = new HashSet<string>();
        public HashSet<string> IncludedTags { get; set; } = new HashSet<string>();
        public List<string> Fandoms { get; set; } = new List<string>();
    }
}
