namespace Willowcat.EbookDesktopUI.Models
{
    public class MergeBooksModel
    {
        #region Properties...

        #region BookTitle
        public string BookTitle
        {
            get => UserSettings.MergeBooksBookTitle;
            set
            {
                UserSettings.MergeBooksBookTitle = value;
                UserSettings.Save();
            }
        }
        #endregion BookTitle

        #region FolderName
        public string FolderName
        {
            get => UserSettings.LastFolderName;
            set
            {
                UserSettings.LastFolderName = value;
                UserSettings.Save();
            }
        }
        #endregion FolderName

        #region IncludeIndexes
        public string IncludeIndexes
        {
            get => UserSettings.MergeBooksIncludeIndexes;
            set
            {
                UserSettings.MergeBooksIncludeIndexes = value;
                UserSettings.Save();
            }
        }
        #endregion IncludeIndexes

        #region SeriesIndex
        public string SeriesIndex
        {
            get => UserSettings.MergeBooksSeriesIndex;
            set
            {
                UserSettings.MergeBooksSeriesIndex = value;
                UserSettings.Save();
            }
        }
        #endregion SeriesIndex

        #region SeriesName
        public string SeriesName
        {
            get => UserSettings.MergeBooksSeriesName;
            set
            {
                UserSettings.MergeBooksSeriesName = value;
                UserSettings.Save();
            }
        }
        #endregion SeriesName

        #region SeriesUrl
        public string SeriesUrl
        {
            get => UserSettings.MergeBooksSeriesUrl;
            set
            {
                UserSettings.MergeBooksSeriesUrl = value;
                UserSettings.Save();
            }
        }
        #endregion SeriesUrl

        #region UserSettings
        internal Properties.Settings UserSettings => Properties.Settings.Default;
        #endregion UserSettings

        #region WorkUrls
        public string WorkUrls
        {
            get => UserSettings.MergeBooksWorkUrls;
            set
            {
                UserSettings.MergeBooksWorkUrls = value;
                UserSettings.Save();
            }
        }
        #endregion WorkUrls

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...
    }
}
