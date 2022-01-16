using Prism.Commands;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubListItemViewModel : ViewModelBase
    {
        #region Member Variables...
        private EpubDisplayModel _DisplayModel;
        #endregion Member Variables...

        #region Properties...

        #region DisplayModel
        public EpubDisplayModel DisplayModel
        {
            get => _DisplayModel;
            set
            {
                _DisplayModel = value;
                OnPropertyChanged();
            }
        }
        #endregion DisplayModel

        #endregion Properties...

        #region Constructors...

        #region EpubListItemViewModel
        public EpubListItemViewModel(EpubDisplayModel item)
        {
            _DisplayModel = item;
        }
        #endregion EpubListItemViewModel

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
