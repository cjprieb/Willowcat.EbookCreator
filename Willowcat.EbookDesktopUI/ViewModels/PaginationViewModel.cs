using Prism.Commands;
using System;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Events;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class PaginationViewModel : ViewModelBase
    {
        #region Member Variables...

        private int _CurrentPage = 1;
        private int _ItemsPerPage = 10;
        private int _TotalItems = 0;

        #endregion Member Variables...

        #region Properties...

        #region CurrentPage
        public int CurrentPage
        {
            get => _CurrentPage;
            set
            {
                _CurrentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NextPage));
                OnPropertyChanged(nameof(PreviousPage));
                RequestPageCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion CurrentPage

        #region ItemsPerPage
        public int ItemsPerPage
        {
            get => _ItemsPerPage;
            set
            {
                if (_ItemsPerPage != value)
                {
                    _ItemsPerPage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPages));
                }
            }
        }
        #endregion ItemsPerPage

        #region NextPage
        public int? NextPage => CurrentPage < TotalPages ? CurrentPage + 1 : (int?)null;
        #endregion NextPage

        #region PreviousPage
        public int? PreviousPage => CurrentPage > 1 ? CurrentPage - 1 : (int?)null;
        #endregion PreviousPage

        #region RequestPageCommand
        public DelegateCommand<int?> RequestPageCommand { get; private set; }
        #endregion RequestPageCommand

        #region TotalItems
        public int TotalItems
        {
            get => _TotalItems;
            set
            {
                if (_TotalItems != value)
                {
                    _TotalItems = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPages));
                }
            }
        }
        #endregion TotalItems

        #region TotalPages
        public int TotalPages
        {
            get => (TotalItems / ItemsPerPage) + 1;
        }
        #endregion TotalPages

        #endregion Properties...

        #region Events...
        public event EventHandler<PageRequestedEventArgs> OnPageRequested;
        #endregion Events...

        #region Constructors...

        #region PaginationViewModel
        public PaginationViewModel()
        {
            RequestPageCommand = new DelegateCommand<int?>(ExecuteRequestPage, CanRequestPage);
        }
        #endregion PaginationViewModel

        #endregion Constructors...

        #region Methods...

        #region CanRequestPage
        private bool CanRequestPage(int? page)
        {
            return page.HasValue;
        }
        #endregion CanRequestPage

        #region ExecuteRequestPage
        private void ExecuteRequestPage(int? page)
        {
            if (page.HasValue)
            {
                CurrentPage = page.Value;
                int itemsToSkip = ItemsPerPage * (CurrentPage - 1);
                OnPageRequested?.Invoke(this, new PageRequestedEventArgs(itemsToSkip, ItemsPerPage));
            }
        }
        #endregion ExecuteRequestPage

        #endregion Methods...
    }
}
