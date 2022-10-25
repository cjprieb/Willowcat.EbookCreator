using Willowcat.Common.UI.ViewModel;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class TagViewModel : ViewModelBase
    {
        #region Member Variables...
        private bool _IsSelected = false;
        private string _Name = string.Empty;
        #endregion Member Variables...

        #region Properties...

        #region Name
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }
        #endregion Name

        #region IsSelected
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                OnPropertyChanged();
            }
        }
        #endregion IsSelected

        #endregion Properties...

        #region Constructors...

        #region TagViewModel
        public TagViewModel(string name, bool isSelected = false)
        {
            Name = name;
            IsSelected = isSelected;
        }
        #endregion TagViewModel

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
