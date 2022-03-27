using Prism.Commands;
using System;
using System.Collections.Generic;
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
        private static Dictionary<ProcessTagType, string> _ProcessTags = new Dictionary<ProcessTagType, string>() 
        {
            { ProcessTagType.All, "" },
            { ProcessTagType.None, "Untagged" },
            { ProcessTagType.InCalibre, "In Calibre" },
            { ProcessTagType.CombineAsSeries, "Combine As Series" },
            { ProcessTagType.CombineAsShortStories, "Combine As Short Stories" },
            { ProcessTagType.IncludeAsBookmark, "Include As Bookmark" },
            { ProcessTagType.Skip, "Skip" },
            { ProcessTagType.Maybe, "Maybe" }
        };

        private readonly EbookFileService _EbookFileService;

        private bool _DoFullTextSearch = false;
        private FilterModel _FilterModel;
        private string _Author = null;
        private string _Keyword = null;
        private TaskProgressType _SearchTaskStatus = TaskProgressType.None;
        private ProcessTagType _SelectedProcessTagType = ProcessTagType.None;
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

        #region ClearKeywordCommand
        public ICommand ClearKeywordCommand { get; private set; }
        #endregion ClearKeywordCommand

        #region DoFullTextSearch
        public bool DoFullTextSearch
        {
            get => _DoFullTextSearch;
            set
            {
                _DoFullTextSearch = value;
                OnPropertyChanged();
            }
        }
        #endregion DoFullTextSearch

        #region ExcludedTagsViewModel
        public TagFilterListViewModel ExcludedTagsViewModel { get; private set; } = new TagFilterListViewModel("Exclude:");
        #endregion ExcludedTagsViewModel

        #region IncludedTagsViewModel
        public TagFilterListViewModel IncludedTagsViewModel { get; private set; } = new TagFilterListViewModel("Include:");
        #endregion IncludedTagsViewModel

        #region FandomsViewModel
        public TagFilterListViewModel FandomsViewModel { get; private set; } = new TagFilterListViewModel("Fandoms:");
        #endregion FandomsViewModel

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

        #region Keyword
        public string Keyword
        {
            get => _Keyword;
            set
            {
                _Keyword = value;
                OnPropertyChanged();
            }
        }
        #endregion Keyword

        #region IsSearching
        public bool IsSearching
        {
            get => (SearchTaskStatus == TaskProgressType.Running);
        }
        #endregion IsSearching

        #region ProcessTags
        public Dictionary<ProcessTagType, string> ProcessTags
        {
            get => _ProcessTags;
        }
        #endregion ProcessTags

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

        #region SelectedProcessTag
        public ProcessTagType SelectedProcessTag
        {
            get => _SelectedProcessTagType;
            set
            {
                _SelectedProcessTagType = value;
                OnPropertyChanged();
            }
        }
        #endregion SelectedProcessTag

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
            ClearKeywordCommand = new DelegateCommand(ExecuteClearKeyword);
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

        #region ExcludeTagFromFilter
        public void ExcludeTagFromFilter(string tagName)
        {
            IncludedTagsViewModel.RemoveTag(tagName);
            ExcludedTagsViewModel.AddTag(tagName);
            FandomsViewModel.RemoveTag(tagName);
        }
        #endregion ExcludeTagFromFilter

        #region ExecuteApplyFilter
        private void ExecuteApplyFilter()
        {
            var filterModel = new FilterModel();
            filterModel.DoFullTextSearch = DoFullTextSearch;
            filterModel.Author = Author;
            filterModel.Keyword = Keyword;
            filterModel.ExcludedTags.AddAll(ExcludedTagsViewModel.SelectedTags.Select(tag => tag.Name));
            filterModel.IncludedTags.AddAll(IncludedTagsViewModel.SelectedTags.Select(tag => tag.Name));
            filterModel.Fandoms.AddAll(FandomsViewModel.SelectedTags.Select(tag => tag.Name));
            filterModel.SelectedProcessTag = SelectedProcessTag;
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

        #region ExecuteClearKeyword
        private void ExecuteClearKeyword()
        {
            Keyword = null;
            ExecuteApplyFilter();
        }
        #endregion ExecuteClearKeyword

        #region IncludeTagInFilter
        public void IncludeTagInFilter(string tagName)
        {
            ExcludedTagsViewModel.RemoveTag(tagName);
            if (!FandomsViewModel.AddTag(tagName))
            {
                IncludedTagsViewModel.AddTag(tagName);
            }
            ExecuteApplyFilter();
        }
        #endregion IncludeTagInFilter

        #region InitializeFandoms
        public void InitializeFandoms(IEnumerable<EpubDisplayModel> books)
        {
            var fandomTags = new HashSet<string>();

            foreach (var pub in books)
            {
                if (pub != null && pub.FandomTags != null)
                {
                    fandomTags.AddAll(pub.FandomTags);
                }
            }

            FandomsViewModel.ShowComboBox = true;
            foreach (var fandom in fandomTags.OrderBy(tag => tag))
            {
                FandomsViewModel.PossibleTags.Add(new TagViewModel(fandom));
            }

            FilterModel = new FilterModel()
            {
                SelectedProcessTag = SelectedProcessTag
            };
        }
        #endregion InitializeFandoms

        #endregion Methods...
    }
}
