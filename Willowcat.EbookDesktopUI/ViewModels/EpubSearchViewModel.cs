using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Willowcat.Common.UI.ViewModel;
using Willowcat.Common.Utilities;
using Willowcat.EbookDesktopUI.Events;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubSearchViewModel : ViewModelBase, IProgress<LoadProgressModel>
    {
        #region Member Variables...
        private readonly EbookFileService _EbookFileService;
        private readonly SettingsModel _Settings = null;

        private bool _IsRefreshingList = false;
        private bool _ShowProgressBar = true;
        private int _TotalWorks = 0;
        private int _WorksProcessedCount = 0;
        private List<EpubItemViewModel> _AllEpubDisplayModels = new List<EpubItemViewModel>();
        private List<EpubItemViewModel> _FilteredEpubDisplayModels = new List<EpubItemViewModel>();
        private string _SelectedTab = null;

        private object _LockProgress = new object();
        #endregion Member Variables...

        #region Properties...

        #region EpubListViewModel
        public EpubListViewModel EpubListViewModel { get; private set; }
        #endregion EpubListViewModel

        #region FilterViewModel
        public FilterViewModel FilterViewModel { get; private set; }
        #endregion FilterViewModel

        #region IsRefreshingList
        public bool IsRefreshingList
        {
            get => _IsRefreshingList;
            set
            {
                _IsRefreshingList = value;
                OnPropertyChanged();
            }
        }
        #endregion IsRefreshingList

        #region Pagination
        public PaginationViewModel Pagination { get; private set; } = new PaginationViewModel();
        #endregion Pagination

        #region PercentComplete
        public decimal PercentComplete
        {
            get => TotalWorks != 0 ? ((decimal)WorksProcessedCount / TotalWorks * 100) : 0;
        }
        #endregion PercentComplete

        #region SelectedTab
        public string SelectedTab
        {
            get => _SelectedTab;
            set
            {
                _SelectedTab = value;
                OnPropertyChanged();
            }
        }
        #endregion SelectedTab

        #region ShowProgressBar
        public bool ShowProgressBar
        {
            get => _ShowProgressBar;
            set
            {
                _ShowProgressBar = value;
                OnPropertyChanged();
            }
        }
        #endregion ShowProgressBar

        #region TotalWorks
        public int TotalWorks
        {
            get => _TotalWorks;
            set
            {
                _TotalWorks = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PercentComplete));
            }
        }
        #endregion TotalWorks

        #region WorksProcessedCount
        public int WorksProcessedCount
        {
            get => _WorksProcessedCount;
            set
            {
                _WorksProcessedCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PercentComplete));
            }
        }
        #endregion WorksProcessedCount

        #endregion Properties...

        #region Event Handlers...
        public event EventHandler<SeriesMergeEventArgs> SeriesMergeRequested;
        #endregion Event Handlers...

        #region Constructors...

        #region EpubSearchViewModel
        public EpubSearchViewModel(EbookFileService ebookFileService, SettingsModel settings)
        {
            _Settings = settings;
            _EbookFileService = ebookFileService;
            _EbookFileService.LoadingProgress = this;

            FilterViewModel = new FilterViewModel(_EbookFileService);
            FilterViewModel.FilterUpdated += FilterViewModel_FilterUpdated;

            EpubListViewModel = new EpubListViewModel(_EbookFileService, settings);

            Pagination.OnPageRequested += Pagination_OnPageRequested;
        }
        #endregion EpubSearchViewModel

        #endregion Constructors...

        #region Methods...

        #region Event Handlers...

        #region BookViewModel_SeriesMergeRequested
        private void BookViewModel_SeriesMergeRequested(object sender, SeriesMergeEventArgs e)
        {
            SeriesMergeRequested?.Invoke(this, e);
        }
        #endregion BookViewModel_SeriesMergeRequested

        #region FilterViewModel_FilterUpdated
        private async void FilterViewModel_FilterUpdated(object sender, Events.FilterUpdatedEventArgs e)
        {
            if (FilterViewModel == sender)
            {
                await ApplyFilterAsync();
            }
        }
        #endregion FilterViewModel_FilterUpdated

        #region Pagination_OnPageRequested
        private async void Pagination_OnPageRequested(object sender, PageRequestedEventArgs e)
        {
            await ApplyPaginationAsync(e.ItemsToSkip, e.ItemsToDisplay);
        }
        #endregion Pagination_OnPageRequested

        #endregion Event Handlers...

        #region ApplyFilterAsync
        public async Task ApplyFilterAsync()
        {
            if (FilterViewModel.FilterModel != null && EpubListViewModel != null)
            {
                try
                {
                    FilterViewModel.SearchTaskStatus = TaskProgressType.Running;
                    IsRefreshingList = true;

                    _FilteredEpubDisplayModels = await Task.Run(() =>
                    {
                        var filteredItems = _AllEpubDisplayModels
                            .Where(item => FilterViewModel.FilterModel.IsMatch(item.DisplayModel))
                            .ToList();
                        return filteredItems;
                    });

                    Pagination.SetTotalItems(_FilteredEpubDisplayModels.Count);
                    Pagination.CurrentPage = 1;
                    await ApplyPaginationAsync(0, Pagination.ItemsPerPage);
                }
                finally
                {
                    FilterViewModel.SearchTaskStatus = TaskProgressType.Finished;
                }
            }            
        }
        #endregion ApplyFilterAsync

        #region ApplyPaginationAsync
        public async Task ApplyPaginationAsync(int itemsToSkip, int itemsToDisplay)
        {
            if (FilterViewModel.FilterModel != null && EpubListViewModel != null)
            {
                try
                {
                    IsRefreshingList = true;

                    EpubListViewModel.ApplyPagination(_FilteredEpubDisplayModels, itemsToSkip, itemsToDisplay);
                }
                finally
                {
                    IsRefreshingList = false;
                }
            }
            await Task.FromResult(0);
        }
        #endregion ApplyPaginationAsync

        #region LoadAsync
        public async Task LoadAsync()
        {
            if (EpubListViewModel != null)
            {
                var workLoadingTasks = await Task.Run(() => _EbookFileService.GetAllResultsAsync());

                var totalBooks = workLoadingTasks.Count();
                Report(new LoadProgressModel(0, totalBooks));
                var books = await Task.WhenAll(workLoadingTasks.ToArray());
                Report(new LoadProgressModel(totalBooks, totalBooks));

                FilterViewModel.InitializeFandoms(books);
                foreach (var bookItem in books)
                {
                    if (bookItem.FandomTags.Any())
                    {
                        var bookViewModel = new EpubItemViewModel(_EbookFileService, FilterViewModel, bookItem, _Settings);
                        bookViewModel.SeriesMergeRequested += BookViewModel_SeriesMergeRequested;
                        bookViewModel.IsVisible = true;
                        _AllEpubDisplayModels.Add(bookViewModel);
                    }
                }
                ShowProgressBar = false;

                await ApplyFilterAsync();
            }
        }
        #endregion LoadAsync

        #region Report
        public void Report(LoadProgressModel value)
        {
            lock (_LockProgress)
            {
                int? current = value.IncrementCount.HasValue ? WorksProcessedCount + 1 : value.CurrentCount;
                int? total = value.IncrementCount.HasValue ? null : value.TotalCount;

                if (current.HasValue)
                {
                    WorksProcessedCount = current.Value;
                }

                if (total.HasValue)
                {
                    TotalWorks = total.Value;
                }
            }
        }
        #endregion Report

        #endregion Methods...
    }
}
