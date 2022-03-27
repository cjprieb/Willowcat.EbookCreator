using System.Collections.Generic;
using System.Threading.Tasks;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Member Variables...
        private EpubSearchViewModel _EpubSearchViewModel = null;
        private MergeBooksViewModel _MergeBooksViewModel = null;
        #endregion Member Variables...

        #region Properties...

        #region EpubSearchViewModel
        public EpubSearchViewModel EpubSearchViewModel
        {
            get => _EpubSearchViewModel;
            set
            {
                _EpubSearchViewModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowEpubSearchViewModel));
            }
        }
        #endregion EpubSearchViewModel

        #region MergeBooksViewModel
        public MergeBooksViewModel MergeBooksViewModel
        {
            get => _MergeBooksViewModel;
            set
            {
                _MergeBooksViewModel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowMergeBooksViewModel));
            }
        }
        #endregion MergeBooksViewModel

        #region Settings
        public SettingsModel Settings { get; private set; }
        #endregion Settings

        #region ShowEpubSearchViewModel
        public bool ShowEpubSearchViewModel
        {
            get => EpubSearchViewModel != null;
        }
        #endregion ShowEpubSearchViewModel

        #region ShowMergeBooksViewModel
        public bool ShowMergeBooksViewModel
        {
            get => MergeBooksViewModel != null;
        }
        #endregion ShowMergeBooksViewModel

        #endregion Properties...

        #region Constructors...

        #region MainViewModel
        public MainViewModel(EbookFileService ebookFileService, SettingsModel settings)
        {
            Settings = settings;
            EpubSearchViewModel = new EpubSearchViewModel(ebookFileService, Settings);
            MergeBooksViewModel = new MergeBooksViewModel(Settings);
        }
        #endregion MainViewModel

        #endregion Constructors...

        #region Methods...

        #region LoadAsync
        public Task LoadAsync()
        {
            Settings.LoadFromProperties();

            List<Task> tasks = new List<Task>();

            if (EpubSearchViewModel != null)
            {
                tasks.Add(EpubSearchViewModel.LoadAsync());
            }

            if (MergeBooksViewModel != null)
            {
                tasks.Add(MergeBooksViewModel.LoadAsync());
            }

            return Task.WhenAll(tasks.ToArray());
        }
        #endregion LoadAsync

        #endregion Methods...
    }
}
