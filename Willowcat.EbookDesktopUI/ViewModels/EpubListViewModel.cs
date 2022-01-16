using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubListViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly EbookFileService _EbookFileService;
        private readonly SettingsModel _Settings = null;

        private EpubItemViewModel _SelectedEpubItemViewModel = null;
        #endregion Member Variables...

        #region Properties...

        #region Books
        public ObservableCollection<EpubItemViewModel> Books { get; set; } = new ObservableCollection<EpubItemViewModel>();
        #endregion Books

        #region SelectedEpubItemViewModel
        public EpubItemViewModel SelectedEpubItemViewModel
        {
            get => _SelectedEpubItemViewModel;
            set
            {
                _SelectedEpubItemViewModel = value;
                OnPropertyChanged();
            }
        }
        #endregion SelectedEpubItemViewModel

        #endregion Properties...

        #region Constructors...

        #region EpubListViewModel
        public EpubListViewModel(EbookFileService ebookFileService, SettingsModel settings)
        {
            _Settings = settings;
            _EbookFileService = ebookFileService;
        }
        #endregion EpubListViewModel

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
