using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookCreator.Models
{
    public class TableOfContentsModel
    {
        #region Properties...

        /// <summary>
        /// The html files that will be read. Add them to the list in the preferred reading order.
        /// </summary>
        public List<FileItemModel> ChapterFiles { get; private set; } = new List<FileItemModel>();

        /// <summary>
        /// These entries are used to build the Table of Contents file. Usually one per chapter file.
        /// These entries can also be nested.
        /// </summary>
        public List<TableOfContentsLinkModel> Entries { get; private set; } = new List<TableOfContentsLinkModel>();

        /// <summary>
        /// Other files that are referenced in the epub, such as images or stylesheets
        /// </summary>
        public List<FileItemModel> OtherFiles { get; private set; } = new List<FileItemModel>();

        /// <summary>
        /// The name of the file to use as the cover
        /// </summary>
        public string CoverFileName { get; set; }

        #endregion Properties...
    }
}
 