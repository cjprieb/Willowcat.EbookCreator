using System;
using System.Collections.Generic;

namespace Willowcat.EbookCreator.Models
{
    public class ContentsFileModel
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        public CoverModel Cover { get; set; }
        public FileItemModel ContentsFile { get; internal set; }
        public FileItemModel TableOfContentsPage { get; set; }
        public FileItemModel TitlePage { get; set; }
        public List<FileItemModel> ChapterFiles { get; private set; } = new List<FileItemModel>();
        public List<FileItemModel> OtherFiles { get; private set; } = new List<FileItemModel>();

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region GetGeneratedFiles
        internal IEnumerable<FileItemModel> GetGeneratedFiles()
        {
            yield return Cover?.CoverHtmlPage;
            yield return TableOfContentsPage;
            yield return TitlePage;
            yield return ContentsFile;
        }
        #endregion GetGeneratedFiles

        #region GetManifestList
        public IEnumerable<FileItemModel> GetManifestList()
        {
            if (TitlePage != null)
            {
                yield return TitlePage;
            }
            if (TableOfContentsPage != null)
            {
                yield return TableOfContentsPage;
            }
            if (Cover?.CoverHtmlPage != null)
            {
                yield return Cover.CoverHtmlPage;
            }
            foreach (var file in ChapterFiles)
            {
                yield return file;
            }
            if (Cover?.CoverImage != null)
            {
                yield return Cover.CoverImage;
            }
            foreach (var file in OtherFiles)
            {
                yield return file;
            }
        }
        #endregion GetManifestList

        #region GetSpineList
        public IEnumerable<FileItemModel> GetSpineList()
        {
            if (Cover?.CoverHtmlPage != null)
            {
                yield return Cover.CoverHtmlPage;
            }
            foreach (var file in ChapterFiles)
            {
                yield return file;
            };
        }
        #endregion GetSpineList

        #endregion Methods...
    }
}
