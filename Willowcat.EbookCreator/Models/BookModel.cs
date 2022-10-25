using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookCreator.Models
{
    public class BookModel
    {
        #region Properties...

        #region Bibliography
        public IBibliographyModel Bibliography { get; set; }
        #endregion Bibliography

        #region TableOfContents
        public TableOfContentsModel TableOfContents { get; set; } = new TableOfContentsModel();
        #endregion TableOfContents

        #region WordsReadPerMinute
        public int? WordsReadPerMinute { get; set; }
        #endregion WordsReadPerMinute

        #endregion Properties...
    }
}
