using System.Threading.Tasks;
using Willowcat.Common.UI.ViewModel;
using Willowcat.Common.Utilities;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubSearchViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly EbookFileService _EbookFileService;
        private readonly SettingsModel _Settings = null;

        private EpubListViewModel _EpubListViewModel = null;

        #endregion Member Variables...

        #region Properties...

        #region EpubListViewModel
        public EpubListViewModel EpubListViewModel { get; private set; }
        #endregion EpubListViewModel

        #region FilterViewModel
        public FilterViewModel FilterViewModel { get; private set; }
        #endregion FilterViewModel

        #endregion Properties...

        #region Constructors...

        #region EpubSearchViewModel
        public EpubSearchViewModel(EbookFileService ebookFileService, SettingsModel settings)
        {
            _Settings = settings;
            _EbookFileService = ebookFileService;

            FilterViewModel = new FilterViewModel(_EbookFileService);
            FilterViewModel.FilterUpdated += FilterViewModel_FilterUpdated;

            EpubListViewModel = new EpubListViewModel(_EbookFileService, settings);
        }
        #endregion EpubSearchViewModel

        #endregion Constructors...

        #region Methods...

        #region Event Handlers...

        #region FilterViewModel_FilterUpdated
        private async void FilterViewModel_FilterUpdated(object sender, Events.FilterUpdatedEventArgs e)
        {
            if (FilterViewModel == sender)
            {
                await ApplyFilterAsync();
            }
        }
        #endregion FilterViewModel_FilterUpdated

        #endregion Event Handlers...

        #region ApplyFilterAsync
        public async Task ApplyFilterAsync()
        {
            if (FilterViewModel.FilterModel != null && EpubListViewModel != null)
            {
                try
                {
                    FilterViewModel.SearchTaskStatus = TaskProgressType.Running;
                    //EpubListViewModel.Books.Clear();
                    await Task.Delay(5000);
                }
                finally
                {
                    FilterViewModel.SearchTaskStatus = TaskProgressType.Finished;
                }
            }            
        }
        #endregion ApplyFilterAsync

        #region LoadAsync
        public async Task LoadAsync()
        {
            await FilterViewModel.LoadAsync();
            await ApplyFilterAsync();
        }
        #endregion LoadAsync

        #endregion Methods...
    }
}
