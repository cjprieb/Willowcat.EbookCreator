using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookCreator.Models
{
    public class BookModel
    {
        #region Properties...

        #region Bibliography
        public BibliographyModel Bibliography { get; set; }
        #endregion Bibliography

        #region TableOfContents
        public TableOfContentsModel TableOfContents { get; set; }
        #endregion TableOfContents

        #endregion Properties...
    }
}
