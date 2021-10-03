using CsQuery;

namespace Willowcat.EbookCreator.Utilities
{
    public static class CQExtensions
    {
        #region Methods...
        public static void SetInnerTextOfFirst(this CQ template, string text)
        {
            if (template != null && template.Length > 0)
            {
                template[0].InnerText = text;
            }
        }

        public static CQ AppendParagraph(this CQ template, string innerHtml, bool makeCenter = false)
        {
            var centerStyle = makeCenter ? " style=\"text-align: center;\"" : "";
            return template.Append($"\n<p{centerStyle}>{innerHtml}</p>\n");
        }

        #endregion Methods...
    }
}
