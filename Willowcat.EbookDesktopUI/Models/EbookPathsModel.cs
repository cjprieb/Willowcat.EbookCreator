using System.IO;
using Willowcat.EbookCreator.EPub;

namespace Willowcat.EbookDesktopUI.Models
{
    public class EbookPathsModel
    {
        #region Member Variables...

        private readonly string _EBookDirectory = "";
        private readonly string _BookName = "";

        #endregion Member Variables...

        #region Properties...

        #region EpubFilePath
        public string EpubFilePath => Path.Combine(_EBookDirectory, _BookName, $"{_BookName}.epub");
        #endregion EpubFilePath

        #region InputDirectory
        public string InputDirectory => Path.Combine(_EBookDirectory, _BookName, "original");
        #endregion InputDirectory

        #region OutputDirectory
        public string OutputDirectory => Path.Combine(_EBookDirectory, _BookName, "epub files");
        #endregion OutputDirectory 

        #endregion Properties...

        #region Constructors...

        #region EbookPathsModel
        public EbookPathsModel(string baseDirectory, string bookName)
        {
            _EBookDirectory = baseDirectory;
            _BookName = bookName;
        }
        #endregion EbookPathsModel

        #endregion Constructors...

        #region Methods...

        #region ToFilePaths
        public EpubFilePaths ToFilePaths()
        {
            return new EpubFilePaths()
            {
                SourceDirectory = InputDirectory,
                StagingDirectory = OutputDirectory,
                EpubFilePath = EpubFilePath
            };
        }
        #endregion ToFilePaths

        #endregion Methods...
    }
}
