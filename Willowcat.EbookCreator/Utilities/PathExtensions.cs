using System.IO;

namespace Willowcat.EbookCreator.Utilities
{
    public static class PathExtensions
    {
        #region Methods...

        #region AddIndexToPath
        public static string AddIndexToFileName(string outputFilePath, int seriesIndex)
        {
            string oldName = Path.GetFileName(outputFilePath);
            string extension = Path.GetExtension(outputFilePath);
            string newName = $"{Path.GetFileNameWithoutExtension(outputFilePath)}_{seriesIndex}{extension}";
            return Path.Combine(Path.GetDirectoryName(outputFilePath), newName);
        }
        #endregion AddIndexToPath

        #region FormatAsEPubFileName
        public static string FormatAsEPubFileName(int workIndex, string workTitle)
        {
            string escapedTitle = workTitle
                .Replace(":", "")
                .Replace("?", "")
                .Replace(",", "");

            return $"{workIndex:D2}-{escapedTitle.Trim()}.epub";
        }
        #endregion FormatAsEPubFileName

        #region GetXmlIdFromFilePath
        public static string GetXmlIdFromFilePath(this string filePath)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            return fileName.Replace(" ", "_");
        }
        #endregion GetXmlIdFromFilePath

        #endregion Methods...
    }
}
