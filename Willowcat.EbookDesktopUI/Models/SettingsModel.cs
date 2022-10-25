using System;

namespace Willowcat.EbookDesktopUI.Models
{
    public class SettingsModel
    {
        #region Properties...

        #region BaseDirectory
        /// <summary>
        /// The directory to create the merged book in
        /// </summary>
        public string BaseMergeDirectory { get; set; }
        #endregion BaseDirectory

        #region BaseCatalogDirectory
        /// <summary>
        /// The directory to search for epub books in
        /// </summary>
        public string BaseCatalogDirectory { get; set; }
        #endregion BaseCatalogDirectory

        #region MoveToCalibreDirectory
        /// <summary>
        /// The directory to copy epub files to when selecting the 
        /// "move to calibre" option. It won't actually add to the
        /// calibre database.
        /// </summary>
        public string MoveToCalibreDirectory { get; set; }
        #endregion MoveToCalibreDirectory

        #region WordsReadPerMinute
        /// <summary>
        /// Used to set a metadata element with an 
        /// estimate of how long it will take to read the book
        /// </summary>
        public int? WordsReadPerMinute { get; set; }
        #endregion WordsReadPerMinute

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region Clone
        public SettingsModel Clone()
        {
            return new SettingsModel() 
            { 
                BaseMergeDirectory = BaseMergeDirectory,
                MoveToCalibreDirectory = MoveToCalibreDirectory,
                WordsReadPerMinute  = WordsReadPerMinute,
                BaseCatalogDirectory = BaseCatalogDirectory
            };
        }
        #endregion Clone

        #region CopyFrom
        public void CopyFrom(SettingsModel source)
        {
            BaseMergeDirectory = source.BaseMergeDirectory;
            MoveToCalibreDirectory = source.MoveToCalibreDirectory;
            WordsReadPerMinute = source.WordsReadPerMinute;
            BaseCatalogDirectory = source.BaseCatalogDirectory;
        }
        #endregion CopyFrom

        #region LoadFromProperties
        public void LoadFromProperties()
        {
            BaseMergeDirectory = Properties.Settings.Default.MergeBooksBaseDirectory;
            if (string.IsNullOrEmpty(BaseMergeDirectory))
            {
                BaseMergeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            MoveToCalibreDirectory = Properties.Settings.Default.MoveToCalibreDirectory;
            if (string.IsNullOrEmpty(MoveToCalibreDirectory))
            {
                MoveToCalibreDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            BaseCatalogDirectory = Properties.Settings.Default.BaseCatalogDirectory;
            if (string.IsNullOrEmpty(BaseCatalogDirectory))
            {
                BaseCatalogDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (Properties.Settings.Default.WordsPerMinute > 0)
            {
                WordsReadPerMinute = Properties.Settings.Default.WordsPerMinute;
            }
        }
        #endregion LoadFromProperties

        #region Save
        public void Save()
        {
            Properties.Settings.Default.MergeBooksBaseDirectory = BaseMergeDirectory;
            Properties.Settings.Default.MoveToCalibreDirectory = MoveToCalibreDirectory;
            Properties.Settings.Default.BaseCatalogDirectory = BaseCatalogDirectory;
            if (WordsReadPerMinute.HasValue)
            {
                Properties.Settings.Default.WordsPerMinute = WordsReadPerMinute.Value;
            }
            Properties.Settings.Default.Save();
        }
        #endregion Save

        #endregion Methods...
    }
}
