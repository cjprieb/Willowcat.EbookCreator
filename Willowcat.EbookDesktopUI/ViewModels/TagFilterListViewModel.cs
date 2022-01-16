using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModel;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class TagFilterListViewModel : ViewModelBase
    {
        #region Member Variables...
        private string _NewTagName = string.Empty;
        private string _Label = string.Empty;
        #endregion Member Variables...

        #region Properties...

        #region AddTagCommand
        public ICommand AddTagCommand { get; private set; }
        #endregion AddTagCommand

        #region Label
        public string Label 
        {
            get => _Label;
            set
            {
                _Label = value;
                OnPropertyChanged();
            }
        }
        #endregion Label

        #region NewTagName
        public string NewTagName
        {
            get => _NewTagName;
            set
            {
                _NewTagName = value;
                OnPropertyChanged();
            }
        }
        #endregion NewTagName

        #region RemoveTagCommand
        public ICommand RemoveTagCommand { get; private set; }
        #endregion RemoveTagCommand

        #region SelectedTags
        public ObservableCollection<TagViewModel> SelectedTags { get; set; } = new ObservableCollection<TagViewModel>();
        #endregion SelectedTags

        #endregion Properties...

        #region Constructors...

        #region FilterViewModel
        public TagFilterListViewModel(string label)
        {
            Label = label;
            AddTagCommand = new DelegateCommand(ExecuteAddTag);
            RemoveTagCommand = new DelegateCommand<string>(ExecuteRemoveTag);
        }
        #endregion FilterViewModel

        #endregion Constructors...

        #region Methods...

        #region ExecuteAddTag
        private void ExecuteAddTag()
        {
            SelectedTags.Add(new TagViewModel(NewTagName, true));
            NewTagName = string.Empty;
        }
        #endregion ExecuteAddTag

        #region ExecuteRemoveTag
        private void ExecuteRemoveTag(string tagName)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                var TagToRemove = SelectedTags.FirstOrDefault(tag => tag.Name == tagName);
                if (TagToRemove != null)
                {
                    SelectedTags.Remove(TagToRemove);
                }
            }
        }
        #endregion ExecuteRemoveTag

        #endregion Methods...
    }
}
