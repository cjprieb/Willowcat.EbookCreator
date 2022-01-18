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
                    EpubListViewModel.Books.Clear();
                    //await Task.Delay(5000);
                    var filteredWorks = await _EbookFileService.GetFilteredResultsAsync(FilterViewModel.FilterModel);
                    foreach (var item in filteredWorks)
                    {
                        var bookViewModel = new EpubItemViewModel(_EbookFileService, FilterViewModel, item, _Settings);
                        bookViewModel.SeriesMergeRequested += BookViewModel_SeriesMergeRequested;
                        EpubListViewModel.Books.Add(bookViewModel);
                    }
                    EpubListViewModel.SelectedEpubItemViewModel = EpubListViewModel.Books.FirstOrDefault();
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
        }
        #endregion LoadAsync

        #region Report
        public void Report(LoadProgressModel value)
        {
            WorksProcessedCount = value.CurrentCount;
            TotalWorks = value.TotalCount;
            if (value.CurrentCount == value.TotalCount)
            {
                ShowProgressBar = false;
            }
        }
        #endregion Report

        #endregion Methods...
    }
}
