namespace Willowcat.EbookCreator.Utilities
{
    public static class StringExtensions
    {
        #region Methods...

        public static string GetXmlIdFromFilePath(this string filePath)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            return fileName.Replace(" ", "_");
        }

        #endregion Methods...
    }
}
