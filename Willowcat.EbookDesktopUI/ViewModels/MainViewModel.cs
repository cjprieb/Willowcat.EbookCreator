using System.Threading.Tasks;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly EbookFileService _EbookFileService;

        #endregion Member Variables...

        #region Properties...

        #region FilterViewModel
        public FilterViewModel FilterViewModel { get; private set; }
        #endregion FilterViewModel

        #region Settings
        public SettingsModel Settings { get; private set; } = new SettingsModel();
        #endregion Settings

        #endregion Properties...

        #region Constructors...

        #region MainWindowViewModel
        public MainViewModel(EbookFileService ebookFileService)
        {
            _EbookFileService = ebookFileService;
            FilterViewModel = new FilterViewModel(_EbookFileService);
        }
        #endregion MainWindowViewModel

        #endregion Constructors...

        #region Methods...

        #region LoadAsync
        public async Task LoadAsync()
        {
            Settings.LoadFromProperties();
            await FilterViewModel.LoadAsync();
        }
        #endregion LoadAsync

        #endregion Methods...
    }
}
