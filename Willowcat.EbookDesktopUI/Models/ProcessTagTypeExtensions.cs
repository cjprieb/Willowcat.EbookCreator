namespace Willowcat.EbookDesktopUI.Models
{
    public static class ProcessTagTypeExtensions
    {
        #region Member Variables...
        public const string ProcessTagPrefix = "process.";
        #endregion Member Variables...

        #region Methods...

        #region Parse
        internal static ProcessTagType? Parse(string tag)
        {
            ProcessTagType? result = null;
            if (tag == "in calibre")
            {
                result = ProcessTagType.InCalibre;
            }
            else if (tag == "add as series")
            {
                result = ProcessTagType.CombineAsSeries;
            }
            else if (tag == "combine")
            {
                result = ProcessTagType.CombineAsShortStories;
            }
            else if (tag == "include as bookmark")
            {
                result = ProcessTagType.IncludeAsBookmark;
            }
            else if (tag == "skip")
            {
                result = ProcessTagType.Skip;
            }
            else if (tag == "maybe")
            {
                result = ProcessTagType.Maybe;
            }
            return result;
        }
        #endregion Parse

        #region ToDisplayName
        public static string ToDisplayName(this ProcessTagType processTagType)
        {
            switch (processTagType)
            {
                case ProcessTagType.All: return null;
                case ProcessTagType.None: return null;
                case ProcessTagType.InCalibre: return "in calibre";
                case ProcessTagType.CombineAsSeries: return "add as series";
                case ProcessTagType.CombineAsShortStories: return "combine";
                case ProcessTagType.IncludeAsBookmark: return "include as bookmark";
                case ProcessTagType.Skip: return "skip";
                case ProcessTagType.Maybe: return "maybe";
            }
            return null;
        }
        #endregion ToDisplayName

        #region ToTagName
        public static string ToTagName(this ProcessTagType processTagType)
        {
            switch (processTagType)
            {
                case ProcessTagType.All: return null;
                case ProcessTagType.None: return null;
                case ProcessTagType.InCalibre: return "process.in calibre";
                case ProcessTagType.CombineAsSeries: return "process.add as series";
                case ProcessTagType.CombineAsShortStories: return "process.combine";
                case ProcessTagType.IncludeAsBookmark: return "process.include as bookmark";
                case ProcessTagType.Skip: return "process.skip";
                case ProcessTagType.Maybe: return "process.maybe";
            }
            return null;
        }
        #endregion ToTagName

        #endregion Methods...
    }
}
