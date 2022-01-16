using System.Diagnostics;
using System.IO;

namespace Willowcat.EbookDesktopUI.Common
{
    public class PathExtensions
    {
        #region ExploreToDirectory
        public static void ExploreToDirectory(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && Directory.Exists(filePath))
            {
                Process.Start("explorer.exe", filePath);
            }
        }
        #endregion ExploreToDirectory

        #region ExploreToFile
        public static void ExploreToFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                Process.Start("explorer.exe", filePath);
            }
        }
        #endregion ExploreToFile
    }
}
