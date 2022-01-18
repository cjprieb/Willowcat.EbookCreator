using System;
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

        private bool _ShowProgressBar = true;
        private int _TotalWorks = 0;
        private int _WorksProcessedCount = 0;
        private string _SelectedTab = null;
        #endregion Member Variables...

        #region Properties...

        #region EpubListViewModel
        public EpubListViewModel EpubListViewModel { get; private set; }
        #endregion EpubListViewModel

        #region FilterViewModel
        public FilterViewModel FilterViewModel { get; private set; }
        #endregion FilterViewModel

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

        #endregion Event Handlers...

        #region ApplyFilterAsync
        public async Task ApplyFilterAsync()
        {
            if (FilterViewModel.FilterModel != null && EpubListViewModel != null)
            {
                try
                {
                    FilterViewModel.SearchTaskStatus = TaskProgressType.Running;

                    await Task.Run(() =>
                    {
                        if (FilterViewModel.FilterModel != null)
                        {
                            foreach (var item in EpubListViewModel.Books)
                            {
                                item.IsVisible = FilterViewModel.FilterModel.IsMatch(item.DisplayModel);
                            }
                        }
                        EpubListViewModel.SelectedEpubItemViewModel = EpubListViewModel.Books.FirstOrDefault();
                    });
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
            if (EpubListViewModel != null)
            {
                try
                {
                    FilterViewModel.SearchTaskStatus = TaskProgressType.Running;

                    EpubListViewModel.Books.Clear();
                    var workLoadingTasks = await Task.Run(() => _EbookFileService.GetAllResultsAsync());

                    var totalBooks = workLoadingTasks.Count();
                    ReportProgress(0, totalBooks);
                    var books = await Task.WhenAll(workLoadingTasks.ToArray());
                    ReportProgress(totalBooks, totalBooks);
                    FilterViewModel.InitializeFandoms(books);
                    int count = 0;
                    foreach (var bookItem in books)
                    {
                        ReportProgress(count, totalBooks);
                        if (bookItem.FandomTags.Any())
                        {
                            var bookViewModel = new EpubItemViewModel(_EbookFileService, FilterViewModel, bookItem, _Settings);
                            bookViewModel.SeriesMergeRequested += BookViewModel_SeriesMergeRequested;
                            EpubListViewModel.Books.Add(bookViewModel);
                        }
                        count++;
                    }
                    ReportProgress(count, totalBooks);
                    EpubListViewModel.SelectedEpubItemViewModel = EpubListViewModel.Books.FirstOrDefault();
                }
                finally
                {
                    FilterViewModel.SearchTaskStatus = TaskProgressType.Finished;
                }
            }
        }
        #endregion LoadAsync

        #region Report
        public void ReportProgress(int? current, int? total)
        {
            if (current.HasValue)
            {
                WorksProcessedCount = current.Value;
            }
            if (total.HasValue)
            {
                TotalWorks = total.Value;
            }
            if (current.HasValue && total.HasValue && current == total)
            {
                ShowProgressBar = false;
            }
        }
        #endregion Report

        #region Report
        public void Report(LoadProgressModel value)
        {
            if (value.IncrementCount.HasValue)
            {
                ReportProgress(WorksProcessedCount++, TotalWorks);
            }
            else
            {
                ReportProgress(value.CurrentCount, value.TotalCount);
            }
        }
        #endregion Report

        #endregion Methods...
    }
}
