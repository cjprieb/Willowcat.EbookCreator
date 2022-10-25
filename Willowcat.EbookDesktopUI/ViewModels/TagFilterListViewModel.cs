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
        private bool _ShowComboBox = false;
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

        #region PossibleTags
        public ObservableCollection<TagViewModel> PossibleTags { get; private set; } = new ObservableCollection<TagViewModel>();
        #endregion PossibleTags

        #region RemoveTagCommand
        public ICommand RemoveTagCommand { get; private set; }
        #endregion RemoveTagCommand

        #region SelectedTags
        public ObservableCollection<TagViewModel> SelectedTags { get; set; } = new ObservableCollection<TagViewModel>();
        #endregion SelectedTags

        #region ShowComboBox
        public bool ShowComboBox
        {
            get => _ShowComboBox;
            set
            {
                _ShowComboBox = value;
                OnPropertyChanged();
            }
        }
        #endregion ShowComboBox

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

        #region AddTag
        public bool AddTag(string tagName)
        {
            bool TagAdded = false;
            if (!string.IsNullOrEmpty(tagName) && !SelectedTags.Any(tag => tag.Name == tagName))
            {
                if (ShowComboBox)
                {
                    if (PossibleTags.Any(tag => tag.Name == tagName))
                    {
                        SelectedTags.Add(new TagViewModel(tagName, true));
                        TagAdded = true;
                    }
                }
                else
                {
                    SelectedTags.Add(new TagViewModel(tagName, true));
                    TagAdded = true;
                }
            }
            return TagAdded;
        }
        #endregion AddTag

        #region ExecuteAddTag
        private void ExecuteAddTag()
        {
            if (!string.IsNullOrEmpty(NewTagName))
            {
                AddTag(NewTagName);
                NewTagName = string.Empty;
            }
        }
        #endregion ExecuteAddTag

        #region ExecuteRemoveTag
        private void ExecuteRemoveTag(string tagName)
        {
            RemoveTag(tagName);
        }
        #endregion ExecuteRemoveTag

        #region RemoveTag
        public void RemoveTag(string tagName)
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
        #endregion RemoveTag

        #endregion Methods...
    }
}
