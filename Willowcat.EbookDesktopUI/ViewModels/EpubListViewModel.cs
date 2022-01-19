using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Events;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class EpubListViewModel : ViewModelBase
    {
        #region Member Variables...
        private readonly EbookFileService _EbookFileService;
        private readonly SettingsModel _Settings = null;

        private EpubItemViewModel _SelectedEpubItemViewModel = null;
        #endregion Member Variables...

        #region Properties...

        #region Books
        public ObservableCollection<EpubItemViewModel> Books { get; set; } = new ObservableCollection<EpubItemViewModel>();
        #endregion Books

        #region SelectedEpubItemViewModel
        public EpubItemViewModel SelectedEpubItemViewModel
        {
            get => _SelectedEpubItemViewModel;
            set
            {
                _SelectedEpubItemViewModel = value;
                OnPropertyChanged();
            }
        }
        #endregion SelectedEpubItemViewModel

        #endregion Properties...

        #region Events...
        public event EventHandler<PageRequestedEventArgs> OnPageRequested;
        #endregion Events...

        #region Constructors...

        #region EpubListViewModel
        public EpubListViewModel(EbookFileService ebookFileService, SettingsModel settings)
        {
            _Settings = settings;
            _EbookFileService = ebookFileService;
        }
        #endregion EpubListViewModel

        #endregion Constructors...

        #region Methods...

        #region ApplyPagination
        public void ApplyPagination(IEnumerable<EpubItemViewModel> items, int itemsToSkip, int itemsToDisplay)
        {
            var itemsToEnable = items
                //.Where(book => book.IsMatch)
                .Skip(itemsToSkip)
                .Take(itemsToDisplay);

            Books.Clear();
            foreach (var item in itemsToEnable)
            {
                Books.Add(item);
            }
            SelectedEpubItemViewModel = itemsToEnable.FirstOrDefault();

            FirePageRequested(itemsToSkip, itemsToDisplay);
        }
        #endregion ApplyPagination

        #region FirePageRequested
        private void FirePageRequested(int itemsToSkip, int itemsToDisplay)
        {
            OnPageRequested?.Invoke(this, new PageRequestedEventArgs(itemsToSkip, itemsToDisplay));
        }
        #endregion FirePageRequested

        #endregion Methods...
    }
}
