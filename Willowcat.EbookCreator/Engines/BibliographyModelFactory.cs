using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Engines
{
    public class BibliographyModelFactory
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region ExtractAdditionalMetadata
        public virtual IBibliographyModel ExtractAdditionalMetadata(ExtractedEpubFilesModel ebook, IBibliographyModel model)
            => model;
        #endregion ExtractAdditionalMetadata

        #region CreateMergedBibliographyModel
        public virtual MergedBibliographyModel CreateMergedBibliographyModel(IBibliographyModel original)
            => new MergedBibliographyModel(original);
        #endregion CreateMergedBibliographyModel

        #endregion Methods...
    }
}
