using System;
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

        #region ApplyPaginationAsync
        public async Task ApplyPaginationAsync(int itemsToSkip, int itemsToDisplay)
        {
            await Task.Run(() =>
            {
                foreach (var item in Books.Where(book => book.IsMatch))
                {
                    item.IsVisible = false;
                }
            });

            var itemsToEnable = Books
                .Where(book => book.IsMatch)
                .Skip(itemsToSkip)
                .Take(itemsToDisplay);

            await Task.Run(() =>
            {
                EpubItemViewModel firstVisibleItem = null;
                foreach (var item in itemsToEnable)
                {
                    if (firstVisibleItem == null)
                    {
                        firstVisibleItem = item;
                    }
                    item.IsVisible = true;
                }
                SelectedEpubItemViewModel = firstVisibleItem;
            });

            FirePageRequested(itemsToSkip, itemsToDisplay);
        }
        #endregion ApplyPaginationAsync

        #region FirePageRequested
        private void FirePageRequested(int itemsToSkip, int itemsToDisplay)
        {
            OnPageRequested?.Invoke(this, new PageRequestedEventArgs(itemsToSkip, itemsToDisplay));
        }
        #endregion FirePageRequested

        #endregion Methods...
    }
}
