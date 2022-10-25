using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Willowcat.EbookCreator.Models
{
    public class ExtractedEpubFilesModel
    {
        public List<string> ChaptersFilePaths { get; set; }
        public string ContentFilePath { get; set; }
        public string OriginalEpubFileName { get; set; }
        public Dictionary<string, string> Stylesheets { get; set; }

        #region GetChapterFiles
        public IEnumerable<FileItemModel> GetChapterFiles()
        {
            return ChaptersFilePaths.Select(x => new FileItemModel(Path.GetFileName(x), MediaType.HtmlXml));
        }
        #endregion GetChapterFiles

        #region GetOtherFiles
        public IEnumerable<string> GetOtherFiles()
        {
            return Stylesheets.Select(kvp => Path.GetFileName(kvp.Value));
        }
        #endregion GetOtherFiles
    }
}