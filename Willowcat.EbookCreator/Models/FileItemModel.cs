using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreator.Models
{
    public class FileItemModel
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        public string Id { get; set; }

        public MediaType MediaType { get; set; }

        public string MediaTypeString => MediaType.ConvertToString();

        public string RelativeFilePath { get; set; }

        public string FileContents { get; set; }

        #endregion Properties...

        #region Constructors...

        public FileItemModel(string filePath, MediaType type)
        {
            Id = filePath.GetXmlIdFromFilePath();
            RelativeFilePath = filePath;
            MediaType = type;
        }

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
