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

        #endregion Member Variables...

        #region Properties...

        #region Books
        public ObservableCollection<EpubListItemViewModel> Books { get; set; } = new ObservableCollection<EpubListItemViewModel>();
        #endregion Books

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
