using Prism.Commands;
using System;
using System.Linq;
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

        private bool _IsVisible = true;
        private EpubDisplayModel _DisplayModel = null;
        #endregion Member Variables...

        #region Properties...

        #region AdditionalTags
        public EpubTagItemsViewModel AdditionalTags { get; private set; }
        #endregion AdditionalTags

        #region AddToCalibreCommand
        public ICommand AddToCalibreCommand { get; private set; }
        #endregion AddToCalibreCommand

        #region CharacterTags
        public EpubTagItemsViewModel CharacterTags { get; private set; }
        #endregion CharacterTags

        #region DisplayModel
        public EpubDisplayModel DisplayModel
        {
            get => _DisplayModel;
            set
            {
                _DisplayModel = value;
                OnPropertyChanged();
                InitializeTagViews();
            }
        }
        #endregion DisplayModel

        #region ExcludeTagCommand
        public ICommand ExcludeTagCommand { get; private set; }
        #endregion ExcludeTagCommand

        #region FandomTags
        public EpubTagItemsViewModel FandomTags { get; private set; }
        #endregion FandomTags

        #region FilterByAuthorCommand
        public ICommand FilterByAuthorCommand { get; private set; }
        #endregion FilterByAuthorCommand

        #region IncludeTagCommand
        public ICommand IncludeTagCommand { get; private set; }
        #endregion IncludeTagCommand

        #region IsVisible
        public bool IsVisible
        {
            get => _IsVisible;
            set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion IsVisible

        #region OverflowTags
        public EpubTagItemsViewModel OverflowTags { get; private set; }
        #endregion OverflowTags

        #region ProcessTags
        public EpubTagItemsViewModel ProcessTags { get; private set; }
        #endregion ProcessTags

        #region RelationshipTags
        public EpubTagItemsViewModel RelationshipTags { get; private set; }
        #endregion RelationshipTags

        #region RequestSeriesMergeCommand
        public ICommand RequestSeriesMergeCommand { get; private set; }
        #endregion RequestSeriesMergeCommand

        #region WarningTags
        public EpubTagItemsViewModel WarningTags { get; private set; }
        #endregion WarningTags

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

            FandomTags = new EpubTagItemsViewModel();
            WarningTags = new EpubTagItemsViewModel();
            CharacterTags = new EpubTagItemsViewModel();
            RelationshipTags = new EpubTagItemsViewModel();
            AdditionalTags = new EpubTagItemsViewModel();
            ProcessTags = new EpubTagItemsViewModel();
            OverflowTags = new EpubTagItemsViewModel();
            InitializeTagViews();

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

        #region InitializeTagViews
        private void InitializeTagViews()
        {
            FandomTags.SetTags(_DisplayModel?.FandomTags);
            WarningTags.SetTags(_DisplayModel?.WarningTags);
            CharacterTags.SetTags(_DisplayModel?.CharacterTags);
            RelationshipTags.SetTags(_DisplayModel?.RelationshipTags);
            AdditionalTags.SetTags(_DisplayModel?.AdditionalTags);
            ProcessTags.SetTags(_DisplayModel?.ProcessTags?.Select(tag => tag.ToDisplayName()));
            OverflowTags.SetTags(_DisplayModel?.OverflowTags);
        }
        #endregion InitializeTagViews

        #endregion Methods...
    }
}
