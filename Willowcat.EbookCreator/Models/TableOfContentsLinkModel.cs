using System.Collections.Generic;

namespace Willowcat.EbookCreator.Models
{
    public class TableOfContentsLinkModel
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...
        public List<TableOfContentsLinkModel> ChildEntries { get; private set; } = new List<TableOfContentsLinkModel>();

        public FileItemModel FileItem { get; set; }

        public string Name { get; set; }

        #endregion Properties...

        #region Constructors...

        public TableOfContentsLinkModel(string name, FileItemModel fileItem)
        {
            Name = name;
            FileItem = fileItem;
        }

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
