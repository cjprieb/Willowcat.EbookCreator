using System.Collections.Generic;
using System.Collections.ObjectModel;
using Willowcat.Common.UI.ViewModel;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubTagItemsViewModel : ViewModelBase
    {
        #region Member Variables...
        #endregion Member Variables...

        #region Properties...

        #region Tags
        public ObservableCollection<string> Tags { get; private set; } = new ObservableCollection<string>();
        #endregion Tags

        #endregion Properties...

        #region Constructors...

        #region EpubTagItemsViewModel
        public EpubTagItemsViewModel()
        {
        }
        #endregion EpubTagItemsViewModel

        #endregion Constructors...

        #region Methods...

        #region SetTags
        public void SetTags(IEnumerable<string> tags)
        {
            Tags.Clear();
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    Tags.Add(tag);
                }
            }
        }
        #endregion SetTags

        #endregion Methods...
    }
}
