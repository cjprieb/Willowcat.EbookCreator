using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Member Variables...
        private SettingsModel _EditedSettings = new SettingsModel();
        private SettingsModel _SettingsModel = null;
        #endregion Member Variables...

        #region Properties...

        #region BaseDirectory
        public string BaseDirectory
        {
            get => _EditedSettings.BaseDirectory;
            set
            {
                _EditedSettings.BaseDirectory = value;
                OnPropertyChanged();
            }
        }
        #endregion BaseDirectory

        #region WordsReadPerMinute
        public string WordsReadPerMinute
        {
            get => _EditedSettings.WordsReadPerMinute.HasValue ? _EditedSettings.WordsReadPerMinute.Value.ToString() : string.Empty;
            set
            {
                if (int.TryParse(value, out int wordsPerMinute))
                {
                    _EditedSettings.WordsReadPerMinute = wordsPerMinute;
                }
                else
                {
                    _EditedSettings.WordsReadPerMinute = null;
                }
                OnPropertyChanged();
            }
        }
        #endregion WordsReadPerMinute

        #endregion Properties...

        #region Constructors...

        #region SettingsViewModel
        public SettingsViewModel(SettingsModel settings)
        {
            _SettingsModel = settings;
            _EditedSettings = settings.Clone();
        }
        #endregion SettingsViewModel

        #endregion Constructors...

        #region Methods...

        #region OnSaveExcecute
        public void OnSaveExcecute()
        {
            _SettingsModel.CopyFrom(_EditedSettings);
            _SettingsModel.Save();
        }
        #endregion OnSaveExcecute

        #endregion Methods...
    }
}
