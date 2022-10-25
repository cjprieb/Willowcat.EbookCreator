using Microsoft.Extensions.Logging;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModel;
using Willowcat.EbookCreator.Engines;
using Willowcat.EbookCreator.Epub;
using Willowcat.EbookCreator.Models;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;

namespace Willowcat.EbookDesktopUI.ViewModels
{
    public class MergeBooksViewModel : ViewModelBase, IProgress<LogItem>
    {
        #region Member Variables...

        private readonly SettingsModel _Settings = null;
        private readonly Ao3BibliographyModelFactory _BibliographyModelFactory = new Ao3BibliographyModelFactory();
        private readonly MergeBooksModel _Model = new MergeBooksModel();

        private bool _OverwriteOriginalFiles = false;
        private string _OutputDirectory = string.Empty;

        #endregion Member Variables...

        #region Properties...

        #region BookTitle
        public string BookTitle
        {
            get => string.IsNullOrEmpty(_Model.BookTitle) ? _Model.SeriesName : _Model.BookTitle;
            set
            {
                _Model.BookTitle = value;
                OnPropertyChanged();
            }
        }
        #endregion BookTitle

        #region ClearFieldsCommand
        public ICommand ClearFieldsCommand { get; private set; }
        #endregion ClearFieldsCommand

        #region DoesDirectoryExist
        public bool DoesDirectoryExist
        {
            get => !string.IsNullOrEmpty(OutputDirectory) && Directory.Exists(OutputDirectory);
        }
        #endregion DoesDirectoryExist

        #region FolderName
        public string FolderName
        {
            get => _Model.FolderName;
            set
            {
                _Model.FolderName = value;
                OnPropertyChanged();
                SetOutputDirectory();
            }
        }
        #endregion FolderName

        #region GenerateBookCommand
        public ICommand GenerateBookCommand { get; private set; }
        #endregion GenerateBookCommand

        #region IncludeIndexes
        public string IncludeIndexes
        {
            get => _Model.IncludeIndexes;
            set
            {
                _Model.IncludeIndexes = value;
                OnPropertyChanged();
            }
        }
        #endregion IncludeIndexes

        #region Logs
        public ObservableCollection<LogItemViewModel> Logs { get; private set; } = new ObservableCollection<LogItemViewModel>();
        #endregion Logs

        #region OutputDirectory
        public string OutputDirectory
        {
            get => _OutputDirectory;
            set
            {
                _OutputDirectory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DoesDirectoryExist));
            }
        }
        #endregion OutputDirectory

        #region OverwriteOriginalFiles
        /// <summary>
        /// Set to false to prevent re-downloading the book files
        /// </summary>
        public bool OverwriteOriginalFiles
        {
            get => _OverwriteOriginalFiles;
            set
            {
                _OverwriteOriginalFiles = value;
                OnPropertyChanged();
            }
        }
        #endregion OverwriteOriginalFiles

        #region SeriesIndex
        public string SeriesIndex
        {
            get => _Model.SeriesIndex;
            set
            {
                _Model.SeriesIndex = value;
                OnPropertyChanged();
            }
        }
        #endregion SeriesIndex

        #region SeriesName
        public string SeriesName
        {
            get => _Model.SeriesName;
            set
            {
                _Model.SeriesName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BookTitle));
            }
        }
        #endregion SeriesName

        #region SeriesUrl
        public string SeriesUrl
        {
            get => _Model.SeriesUrl;
            set
            {
                _Model.SeriesUrl = value;
                OnPropertyChanged();
            }
        }
        #endregion SeriesUrl

        #region WorkUrls
        public string WorkUrls
        {
            get => _Model.WorkUrls;
            set
            {
                _Model.WorkUrls = value;
                OnPropertyChanged();
            }
        }
        #endregion WorkUrls

        #endregion Properties...

        #region Constructors...

        #region MergeBooksViewModel
        public MergeBooksViewModel(SettingsModel settings)
        {
            _Settings = settings;
            GenerateBookCommand = new DelegateCommand(ExecuteGenerateBook);
            ClearFieldsCommand = new DelegateCommand(ExecuteClearFields);
        }
        #endregion MergeBooksViewModel

        #region MergeBooksViewModel
        public MergeBooksViewModel(SettingsModel settings, EpubDisplayModel displayModel, EpubSeriesModel series)
            : this (settings)
        {
            InitializeFieldsFromDisplayModel(displayModel, series);
        }
        #endregion MergeBooksViewModel

        #endregion Constructors...

        #region Methods...

        #region ExecuteClearFields
        private void ExecuteClearFields()
        {
            Logs.Clear();
            BookTitle = string.Empty;
            FolderName = string.Empty;
            IncludeIndexes = string.Empty;
            SeriesIndex = string.Empty;
            SeriesName = string.Empty;
            SeriesUrl = string.Empty;
            WorkUrls = string.Empty;
            FolderName = string.Empty;
        }
        #endregion ExecuteClearFields

        #region ExecuteGenerateBook
        private async void ExecuteGenerateBook()
        {
            try
            {
                Logs.Clear();

                await GenerateBookAsync();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                Logs.Add(new LogItemViewModel(LogLevel.Error, errorMessage));
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage = innerException.Message + Environment.NewLine + innerException.StackTrace;
                    Logs.Add(new LogItemViewModel(LogLevel.Error, errorMessage));
                    innerException = ex.InnerException;
                }
            }
        }
        #endregion ExecuteGenerateBook

        #region GenerateBookAsync
        private async Task GenerateBookAsync()
        {
            int? seriesIndex = ParseSeriesIndex();
            var series = new SeriesModel()
            {
                SeriesUrl = !string.IsNullOrEmpty(SeriesUrl) ? SeriesUrl : null,
                SeriesName = !string.IsNullOrEmpty(SeriesName) ? SeriesName : null,
                OverrideBookTitle = !string.IsNullOrEmpty(BookTitle) ? BookTitle : null,
                WorkUrls = GetWorkUrls(),
                SeriesIndex = seriesIndex
            };
            series.SetWorkIndexesFromString(IncludeIndexes);
            EbookPathsModel paths = SetOutputDirectory();

            if (paths != null)
            {
                var epubBuilder = new EpubBuilder(new MergeLoggingService<EpubBuilder>(this));
                var epubOptions = new EpubOptions()
                {
                    OverwriteOriginalFiles = OverwriteOriginalFiles,
                    WordsReadPerMinute = _Settings.WordsReadPerMinute
                };
                var publicationEngine = new MergeBooksPublicationEngine(new MergeLoggingService<MergeBooksPublicationEngine>(this), epubBuilder, epubOptions, _BibliographyModelFactory);
                await publicationEngine.PublishAsync(series, paths.ToFilePaths());
                OnPropertyChanged(nameof(DoesDirectoryExist));
            }
        }
        #endregion GenerateBookAsync

        #region GetWorkUrls
        private string[] GetWorkUrls()
        {
            string[] result = null;
            if (WorkUrls.Any())
            {
                result = WorkUrls
                    .Split('\n')
                    .Select(line => line.Trim())
                    .ToArray();
            }
            return result;
        }
        #endregion GetWorkUrls

        #region InitializeFieldsFromDisplayModel
        private void InitializeFieldsFromDisplayModel(EpubDisplayModel displayModel, EpubSeriesModel series)
        {
            string folderName = displayModel.Title;
            if (series != null)
            {
                SeriesUrl = series.Url;
                BookTitle = series.Title;
                folderName = $"{series.Title} - {displayModel.Author}";
            }
            FolderName = folderName
                .Replace("&", "")
                .Replace("\"", "")
                .Replace(",", "");
        }
        #endregion InitializeFieldsFromDisplayModel

        #region LoadAsync
        public Task LoadAsync()
        {
            OverwriteOriginalFiles = Properties.Settings.Default.OverwriteOriginalFiles;
            return Task.CompletedTask;
        }
        #endregion LoadAsync

        #region ParseSeriesIndex
        private int? ParseSeriesIndex()
        {
            int? result = null;
            if (!string.IsNullOrEmpty(SeriesIndex))
            {
                if (int.TryParse(SeriesIndex, out int value))
                {
                    result = value;
                }
            }
            return result;
        }
        #endregion ParseSeriesIndex

        #region Report
        public void Report(LogItem value)
        {
            Logs.Add(new LogItemViewModel(value.LogLevel, value.Message));
        }
        #endregion Report

        #region SetOutputDirectory
        private EbookPathsModel SetOutputDirectory()
        {
            EbookPathsModel paths = null;
            if (!string.IsNullOrEmpty(SeriesName) || !string.IsNullOrEmpty(FolderName))
            {
                string folderName = !string.IsNullOrEmpty(FolderName) ? FolderName : null;
                paths = new EbookPathsModel(_Settings.BaseMergeDirectory, string.IsNullOrEmpty(folderName) ? SeriesName : folderName);
                OutputDirectory = Path.GetDirectoryName(paths.EpubFilePath);
            }
            else
            {
                OutputDirectory = string.Empty;
            }
            return paths;
        }
        #endregion SetOutputDirectory

        #endregion Methods...
    }
}
