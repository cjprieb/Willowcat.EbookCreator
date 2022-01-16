using Prism.Commands;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubItemViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly FilterViewModel _FilterViewModel = null;

        private EpubDisplayModel _DisplayModel = null;
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

        #region ExcludeTagCommand
        public ICommand ExcludeTagCommand { get; private set; }
        #endregion ExcludeTagCommand

        #region FilterByAuthorCommand
        public ICommand FilterByAuthorCommand { get; private set; }
        #endregion FilterByAuthorCommand

        #region IncludeTagCommand
        public ICommand IncludeTagCommand { get; private set; }
        #endregion IncludeTagCommand

        #endregion Properties...

        #region Constructors...

        #region EpubListItemViewModel
        public EpubItemViewModel(FilterViewModel filterViewModel, EpubDisplayModel item)
        {
            _FilterViewModel = filterViewModel;
            _DisplayModel = item;

            ExcludeTagCommand = new DelegateCommand<string>(ExecuteExcludeTagFromFilter);
            IncludeTagCommand = new DelegateCommand<string>(ExecuteIncludeTagInFilter);
            FilterByAuthorCommand = new DelegateCommand(ExecuteFilterByAuthor);
        }
        #endregion EpubListItemViewModel

        #endregion Constructors...

        #region Methods...

        #region ExecuteFilterByAuthor
        private void ExecuteFilterByAuthor()
        {
            if (_FilterViewModel != null && !string.IsNullOrEmpty(DisplayModel.Author))
            {
                _FilterViewModel.AddAuthorToFilter(DisplayModel.Author);
            }
        }
        #endregion ExecuteFilterByAuthor

        #region ExecuteIncludeTagInFilter
        private void ExecuteIncludeTagInFilter(string tagName)
        {
            if (_FilterViewModel != null && !string.IsNullOrEmpty(tagName))
            {
                _FilterViewModel.IncludeTagInFilter(tagName);
            }
        }
        #endregion ExecuteIncludeTagInFilter

        #region ExecuteExcludeTagFromFilter
        private void ExecuteExcludeTagFromFilter(string tagName)
        {
            if (_FilterViewModel != null && !string.IsNullOrEmpty(tagName))
            {
                _FilterViewModel.ExcludeTagFromFilter(tagName);
            }
        }
        #endregion ExecuteExcludeTagFromFilter

        #endregion Methods...
    }
}
