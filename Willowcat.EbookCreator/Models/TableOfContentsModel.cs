using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookCreator.Models
{
    public class TableOfContentsModel
    {
        #region Properties...

        public List<FileItemModel> ChapterFiles { get; private set; } = new List<FileItemModel>();
        public List<TableOfContentsLinkModel> Entries { get; private set; } = new List<TableOfContentsLinkModel>();
        public List<FileItemModel> OtherFiles { get; private set; } = new List<FileItemModel>();
        public string CoverFileName { get; set; }

        #endregion Properties...
    }
}
 