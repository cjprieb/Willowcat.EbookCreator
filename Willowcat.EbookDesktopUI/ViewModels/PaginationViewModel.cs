using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
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

        #region AllPages
        public ObservableCollection<PaginationPageViewModel> AllPages { get; private set; } = new ObservableCollection<PaginationPageViewModel>();
        #endregion AllPages

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
                UpdatePageViewModels();
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
            private set
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

        #region PageViewModel_OnLoadPage
        private void PageViewModel_OnLoadPage(object sender, LoadPageEventArgs e)
        {
            ExecuteRequestPage(e.Page);
        }
        #endregion PageViewModel_OnLoadPage

        #region SetTotalItems
        public void SetTotalItems(int totalItems)
        {
            TotalItems = totalItems;
            foreach (var oldModel in AllPages)
            {
                oldModel.OnLoadPage -= PageViewModel_OnLoadPage;
            }

            AllPages.Clear();

            for (int i = 1; i <= TotalPages; i++)
            {
                var pageViewModel = new PaginationPageViewModel(i);
                pageViewModel.OnLoadPage += PageViewModel_OnLoadPage;
                AllPages.Add(pageViewModel);
            }
        }
        #endregion SetTotalItems

        #region UpdatePageViewModels
        private void UpdatePageViewModels()
        {
            foreach (var viewModel in AllPages)
            {
                viewModel.IsCurrentPage = (viewModel.Page == CurrentPage);
            }
        }
        #endregion UpdatePageViewModels

        #endregion Methods...
    }

    public class PaginationPageViewModel : ViewModelBase
    {
        #region Member Variables...
        private bool _IsCurrentPage = false;
        #endregion Member Variables...

        #region Properties...

        #region IsCurrentPage
        public bool IsCurrentPage
        {
            get => _IsCurrentPage;
            set
            {
                _IsCurrentPage = value;
                OnPropertyChanged();
            }
        }
        #endregion IsCurrentPage

        #region RequestPageCommand
        public ICommand RequestPageCommand { get; private set; }
        #endregion RequestPageCommand

        #region Page
        public int Page { get; private set; }
        #endregion Page

        #endregion Properties...

        #region Events...
        public event EventHandler<LoadPageEventArgs> OnLoadPage;
        #endregion Events...

        #region Constructors...

        #region PaginationPageViewModel
        public PaginationPageViewModel(int page)
        {
            Page = page;
            RequestPageCommand = new DelegateCommand(ExecuteLoadPage);
        }
        #endregion PaginationPageViewModel

        #endregion Constructors...

        #region Methods...

        #region ExecuteLoadPage
        private void ExecuteLoadPage()
        {
            OnLoadPage?.Invoke(this, new LoadPageEventArgs(Page));
        }
        #endregion ExecuteLoadPage

        #endregion Methods...
    }
}
