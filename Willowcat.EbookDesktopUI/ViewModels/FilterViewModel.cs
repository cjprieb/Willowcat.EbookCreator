using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;
using Willowcat.EbookCreator.Utilities;
using System.Text;
using Willowcat.Common.Utilities;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class FilterViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly EbookFileService _EbookFileService;

        private FilterModel _FilterModel;
        #endregion Member Variables...

        #region Properties...

        #region ApplyFilterCommand
        public ICommand ApplyFilterCommand { get; private set; }
        #endregion ApplyFilterCommand

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

        #endregion Properties...

        #region Constructors...

        #region FilterViewModel
        public FilterViewModel(EbookFileService ebookFileService)
        {
            _EbookFileService = ebookFileService;

            ApplyFilterCommand = new DelegateCommand(ExecuteApplyFilter);
        }
        #endregion FilterViewModel

        #endregion Constructors...

        #region Methods...

        #region ExecuteApplyFilter
        private void ExecuteApplyFilter()
        {
            var filterModel = new FilterModel();
            filterModel.ExcludedTags.AddAll(ExcludedTagsViewModel.SelectedTags.Select(tag => tag.Name));
            filterModel.IncludedTags.AddAll(IncludedTagsViewModel.SelectedTags.Select(tag => tag.Name));
            filterModel.Fandoms.AddRange(Fandoms.Where(tag => tag.IsSelected).Select(tag => tag.Name));
            FilterModel = filterModel;
        }
        #endregion ExecuteApplyFilter

        #region LoadAsync
        public async Task LoadAsync()
        {
            var fandoms = await _EbookFileService.LoadFandomsAsync();
            foreach (var fandom in fandoms)
            {
                Fandoms.Add(new TagViewModel(fandom));
            }
        }
        #endregion LoadAsync

        #endregion Methods...
    }
}
