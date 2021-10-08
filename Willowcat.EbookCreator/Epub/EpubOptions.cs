namespace Willowcat.EbookCreator.Epub
{
    public class EpubOptions
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        /// <summary>
        /// Set to false to prevent re-downloading the book files
        /// </summary>
        public bool OverwriteOriginalFiles { get; set; } = false;

        /// <summary>
        /// Used to set a metadata element with an 
        /// estimate of how long it will take to read the book
        /// </summary>
        public int? WordsReadPerMinute { get; set; }

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
