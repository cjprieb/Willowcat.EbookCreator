using Prism.Commands;
using System;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Events;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubItemViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly FilterViewModel _FilterViewModel = null;
        private readonly EbookFileService _EbookFileService = null;
        private readonly SettingsModel _Settings = null;

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

        #region AddToCalibreCommand
        public ICommand AddToCalibreCommand { get; private set; }
        #endregion AddToCalibreCommand

        #region ExcludeTagCommand
        public ICommand ExcludeTagCommand { get; private set; }
        #endregion ExcludeTagCommand

        #region FilterByAuthorCommand
        public ICommand FilterByAuthorCommand { get; private set; }
        #endregion FilterByAuthorCommand

        #region IncludeTagCommand
        public ICommand IncludeTagCommand { get; private set; }
        #endregion IncludeTagCommand

        #region RequestSeriesMergeCommand
        public ICommand RequestSeriesMergeCommand { get; private set; }
        #endregion RequestSeriesMergeCommand

        #endregion Properties...

        #region Event Handlers...
        public event EventHandler<SeriesMergeEventArgs> SeriesMergeRequested;
        #endregion Event Handlers...

        #region Constructors...

        #region EpubListItemViewModel
        public EpubItemViewModel(EbookFileService ebookFileService, FilterViewModel filterViewModel, EpubDisplayModel item, SettingsModel settings)
        {
            _EbookFileService = ebookFileService;
            _FilterViewModel = filterViewModel;
            _DisplayModel = item;
            _Settings = settings;

            AddToCalibreCommand = new DelegateCommand(ExecuteAddToCalibre);
            ExcludeTagCommand = new DelegateCommand<string>(ExecuteExcludeTagFromFilter);
            IncludeTagCommand = new DelegateCommand<string>(ExecuteIncludeTagInFilter);
            FilterByAuthorCommand = new DelegateCommand(ExecuteFilterByAuthor);
            RequestSeriesMergeCommand = new DelegateCommand<EpubSeriesModel>(ExecuteRequestSeriesMerge);
        }
        #endregion EpubListItemViewModel

        #endregion Constructors...

        #region Methods...

        #region ExecuteAddToCalibre
        private async void ExecuteAddToCalibre()
        {
            if (!string.IsNullOrEmpty(_Settings.MoveToCalibreDirectory))
            {
                await _EbookFileService.MarkAddToCalibreAsync(DisplayModel);
                OnPropertyChanged(nameof(DisplayModel));
            }
        }
        #endregion ExecuteAddToCalibre

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

        #region ExecuteRequestSeriesMerge
        private async void ExecuteRequestSeriesMerge(EpubSeriesModel seriesModel)
        {
            await _EbookFileService.AddProcessTagAsync(DisplayModel, ProcessTagType.CombineAsSeries);
            OnPropertyChanged(nameof(DisplayModel));
            SeriesMergeRequested?.Invoke(this, new SeriesMergeEventArgs(_DisplayModel, seriesModel));
        }
        #endregion ExecuteRequestSeriesMerge

        #endregion Methods...
    }
}
