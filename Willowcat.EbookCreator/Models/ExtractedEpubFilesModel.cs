using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Willowcat.EbookCreator.Models
{
    internal class ExtractedEpubFilesModel
    {
        public List<string> ChaptersFilePaths { get; set; }
        public string ContentFilePath { get; set; }
        public string OriginalEpubFileName { get; set; }
        public Dictionary<string, string> Stylesheets { get; set; }

        #region GetChapterFiles
        internal IEnumerable<FileItemModel> GetChapterFiles()
        {
            return ChaptersFilePaths.Select(x => new FileItemModel(Path.GetFileName(x), MediaType.HtmlXml));
        }
        #endregion GetChapterFiles

        #region GetOtherFiles
        internal IEnumerable<FileItemModel> GetOtherFiles()
        {
            return Stylesheets.Select(x => new FileItemModel(Path.GetFileName(x.Value), MediaType.Unknown));
        }
        #endregion GetOtherFiles
    }
}