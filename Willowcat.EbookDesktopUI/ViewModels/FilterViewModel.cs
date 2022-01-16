﻿using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModel;
using Willowcat.Common.Utilities;
using Willowcat.EbookDesktopUI.Events;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class FilterViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly EbookFileService _EbookFileService;

        private FilterModel _FilterModel;
        private string _Author = null;
        private TaskProgressType _SearchTaskStatus = TaskProgressType.None;
        #endregion Member Variables...

        #region Properties...

        #region ApplyFilterCommand
        public ICommand ApplyFilterCommand { get; private set; }
        #endregion ApplyFilterCommand

        #region Author
        public string Author
        {
            get => _Author;
            set
            {
                _Author = value;
                OnPropertyChanged();
            }
        }
        #endregion Author

        #region CanApplyFilterCommand
        public bool CanApplyFilterCommand
        {
            get => (SearchTaskStatus != TaskProgressType.Running);
        }
        #endregion CanApplyFilterCommand

        #region ClearAuthorCommand
        public ICommand ClearAuthorCommand { get; private set; }
        #endregion ClearAuthorCommand

        #region ExcludedTagsViewModel
        public TagFilterListViewModel ExcludedTagsViewModel { get; private set; } = new TagFilterListViewModel("Exclude:");
        #endregion ExcludedTagsViewModel

        #region IncludedTagsViewModel
        public TagFilterListViewModel IncludedTagsViewModel { get; private set; } = new TagFilterListViewModel("Include:");
        #endregion IncludedTagsViewModel

        #region Fandoms
        public ObservableCollection<TagViewModel> Fandoms { get; set; } = new ObservableCollection<TagViewModel>();
        #endregion Fandoms

        #region FilterModel
        public FilterModel FilterModel
        {
            get => _FilterModel;
            set
            {
                _FilterModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilterString));
            }
        }
        #endregion FilterModel

        #region FilterString
        public string FilterString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                if (FilterModel != null)
                {
                    builder.Append("Excluded: ").Append(string.Join(", ", FilterModel.ExcludedTags));
                    builder.Append("; Included: ").Append(string.Join(", ", FilterModel.IncludedTags));
                    builder.Append("; Fandoms: ").Append(string.Join(", ", FilterModel.Fandoms));
                }
                return builder.ToString();
            }
        }
        #endregion FilterString

        #region IsSearching
        public bool IsSearching
        {
            get => (SearchTaskStatus == TaskProgressType.Running);
        }
        #endregion IsSearching

        #region SearchTaskStatus
        public TaskProgressType SearchTaskStatus
        {
            get => _SearchTaskStatus;
            set
            {
                _SearchTaskStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanApplyFilterCommand));
                OnPropertyChanged(nameof(IsSearching));
            }
        }
        #endregion SearchTaskStatus

        #endregion Properties...

        #region Events...
        public event EventHandler<FilterUpdatedEventArgs> FilterUpdated;
        #endregion Events...

        #region Constructors...

        #region FilterViewModel
        public FilterViewModel(EbookFileService ebookFileService)
        {
            _EbookFileService = ebookFileService;

            ApplyFilterCommand = new DelegateCommand(ExecuteApplyFilter);
            ClearAuthorCommand = new DelegateCommand(ExecuteClearAuthor);
        }
        #endregion FilterViewModel

        #endregion Constructors...

        #region Methods...

        #region AddAuthorToFilter
        public void AddAuthorToFilter(string author)
        {
            Author = author;
            ExecuteApplyFilter();
        }
        #endregion AddAuthorToFilter

        #region ExecuteApplyFilter
        private void ExecuteApplyFilter()
        {
            var filterModel = new FilterModel();
            filterModel.Author = Author;
            filterModel.ExcludedTags.AddAll(ExcludedTagsViewModel.SelectedTags.Select(tag => tag.Name));
            filterModel.IncludedTags.AddAll(IncludedTagsViewModel.SelectedTags.Select(tag => tag.Name));
            filterModel.Fandoms.AddRange(Fandoms.Where(tag => tag.IsSelected).Select(tag => tag.Name));
            FilterModel = filterModel;
            FilterUpdated?.Invoke(this, new FilterUpdatedEventArgs(filterModel));
        }
        #endregion ExecuteApplyFilter

        #region ExecuteClearAuthor
        private void ExecuteClearAuthor()
        {
            Author = null;
            ExecuteApplyFilter();
        }
        #endregion ExecuteClearAuthor

        #region IncludeTagInFilter
        public void IncludeTagInFilter(string tagName)
        {
            ExcludedTagsViewModel.RemoveTag(tagName);
            var matchingFandomTag = Fandoms.FirstOrDefault(tag => tag.Name == tagName);
            if (matchingFandomTag != null)
            {
                matchingFandomTag.IsSelected = true;
            }
            else
            {
                IncludedTagsViewModel.AddTag(tagName);
            }
            ExecuteApplyFilter();
        }
        #endregion IncludeTagInFilter

        #region LoadAsync
        public async Task LoadAsync()
        {
            var fandoms = await _EbookFileService.LoadFandomsAsync();
            foreach (var fandom in fandoms)
            {
                Fandoms.Add(new TagViewModel(fandom));
            }
            ExecuteApplyFilter();
        }
        #endregion LoadAsync

        #region ExcludeTagFromFilter
        public void ExcludeTagFromFilter(string tagName)
        {
            IncludedTagsViewModel.RemoveTag(tagName);
            ExcludedTagsViewModel.AddTag(tagName);
            var matchingFandomTag = Fandoms.FirstOrDefault(tag => tag.Name == tagName);
            if (matchingFandomTag != null)
            {
                matchingFandomTag.IsSelected = false;
            }
        }
        #endregion ExcludeTagFromFilter

        #endregion Methods...
    }
}
