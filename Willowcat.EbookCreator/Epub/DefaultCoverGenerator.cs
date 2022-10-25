using System.IO;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public class DefaultCoverGenerator : ICoverGenerator
    {
        #region Methods...

        #region CreateCover
        /// <summary>
        /// Returns null if <paramref name="coverImagePath"/> is not 
        /// a valid path. Otherwise, creates an HTML page that
        /// includes the cover image and returns the result.
        /// </summary>
        /// <param name="bibliography"></param>
        /// <param name="coverImagePath"></param>
        /// <returns></returns>
        public virtual CoverModel CreateCover(IBibliographyModel bibliography, string coverImagePath)
        {
            if (string.IsNullOrEmpty(coverImagePath)) return null;

            string coverHtml = Properties.Resources.CoverTemplate
                .Replace("{{Title}}", bibliography.Title)
                .Replace("{{ImageFileName}}", Path.GetFileName(coverImagePath));

            return new CoverModel()
            {
                CoverImage = new FileItemModel(coverImagePath, MediaType.ImagePng),
                CoverHtmlPage = new FileItemModel("cover-page.html", MediaType.HtmlXml)
                {
                    FileContents = coverHtml
                }
            };
        }
        #endregion CreateCover 

        #endregion Methods...
    }
}
