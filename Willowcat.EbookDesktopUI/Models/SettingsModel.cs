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
        public string BaseDirectory { get; set; }
        #endregion BaseDirectory

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
                BaseDirectory = BaseDirectory,
                WordsReadPerMinute  = WordsReadPerMinute
            };
        }
        #endregion Clone

        #region CopyFrom
        public void CopyFrom(SettingsModel source)
        {
            BaseDirectory = source.BaseDirectory;
            WordsReadPerMinute = source.WordsReadPerMinute;
        }
        #endregion CopyFrom

        #region LoadFromProperties
        public void LoadFromProperties()
        {
            BaseDirectory = Properties.Settings.Default.MergeBooksBaseDirectory;
            if (string.IsNullOrEmpty(BaseDirectory))
            {
                BaseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
            Properties.Settings.Default.MergeBooksBaseDirectory = BaseDirectory;
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
