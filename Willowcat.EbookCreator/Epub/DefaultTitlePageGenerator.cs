using CsQuery;
using Willowcat.EbookCreator.Models;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreator.Epub
{
    public class DefaultTitlePageGenerator : ITitlePageGenerator
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region CreateTitlePageFile
        public FileItemModel CreateTitlePageFile(BibliographyModel bibliography)
        {
            CQ template = Properties.Resources.TitlePageTemplate;
            template["#title"].SetInnerTextOfFirst(bibliography.Title);
            template["#creator"].SetInnerTextOfFirst(bibliography.Creator);
            template["#publisher"].SetInnerTextOfFirst(bibliography.Publisher);
            template["#publishedDate"].SetInnerTextOfFirst(bibliography.PublishedDate.ToString("yyyy"));

            return new FileItemModel("titlePage.xhtml", MediaType.HtmlXml)
            {
                FileContents = template.Render()
            };
        }
        #endregion CreateTitlePageFile

        #endregion Methods...
    }
}
