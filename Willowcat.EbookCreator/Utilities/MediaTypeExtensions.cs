using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Utilities
{
    public static class MediaTypeExtensions
    {
        #region Methods...
        public static string ConvertToString(this MediaType type)
        {
            string Result = null;
            switch (type)
            {
                case MediaType.Unknown:
                    Result = "";
                    break;

                case MediaType.HtmlXml:
                    Result = "application/xhtml+xml";
                    break;

                case MediaType.NavXml:
                    Result = "application/x-dtbncx+xml";
                    break;

                case MediaType.ImagePng:
                    Result = "image/png";
                    break;
            }
            return Result;
        }

        #endregion Methods...
    }
}
