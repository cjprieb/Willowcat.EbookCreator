using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookCreator.EPub
{
    public class EpubFilePaths
    {
        #region Properties...

        #region EpubFilePath
        /// <summary>
        /// The path to create the epub file at.
        /// </summary>
        public string EpubFilePath { get; set; }
        #endregion EpubFilePath

        #region SourceDirectory
        /// <summary>
        /// The directory where the original book files are. 
        /// Usually the raw files as downloaded from the internet.
        /// </summary>
        public string SourceDirectory { get; set; }
        #endregion SourceDirectory

        #region StagingDirectory
        /// <summary>
        /// Used for as a staging ground to create the files that
        /// will eventually be zipped into the epub file
        /// </summary>
        public string StagingDirectory { get; set; }
        #endregion StagingDirectory

        #endregion Properties...

    }
}
