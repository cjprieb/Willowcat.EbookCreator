using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Willowcat.Common.Utilities;
using Willowcat.EbookCreator.Engines;
using Willowcat.EbookCreator.Utilities;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.Services
{
    public class EbookFileService
    {
        #region Member Variables...
        private readonly SettingsModel _Settings = null;

        //private HashSet<string> _CachedFandomList = new HashSet<string>();
        //private IEnumerable<EpubDisplayModel> _CachedPublications = null;
        #endregion Member Variables...

        #region Properties...

        public IProgress<LoadProgressModel> LoadingProgress { get; set; }

        #endregion Properties...

        #region Constructors...

        #region EbookFileService
        public EbookFileService(SettingsModel settings)
        {
            _Settings = settings;
        }
        #endregion EbookFileService

        #endregion Constructors...

        #region Methods...

        #region AddProcessTagAsync
        public async Task AddProcessTagAsync(EpubDisplayModel displayModel, ProcessTagType processTag)
        {
            if (displayModel != null && !displayModel.ProcessTags.Contains(processTag))
            {
                List<ProcessTagType> tags = new List<ProcessTagType>(displayModel.ProcessTags);
                tags.Add(ProcessTagType.InCalibre);
                displayModel.ProcessTags = tags.ToArray();
                await Task.Run(() => EpubUtilities.AddSubjectToContentFile(displayModel.LocalFilePath, processTag.ToTagName()));
            }
        }
        #endregion AddProcessTagAsync

        #region LoadPublicationAsync
        private Task<EpubDisplayModel> LoadPublicationAsync(string filePath)
        {
            return Task.Run(() =>
            {
                EpubDisplayModel result = null;

                var builder = new EpubDisplayModelBuilder();
                var tempDirectory = Path.Combine(_Settings.BaseMergeDirectory, "temp");
                var unzipper = new CalibreEpubUnzipper(tempDirectory)
                {
                    NumberOfChapterFilesToInclude = 2
                };
                try
                {
                    var ebook = unzipper.ExtractFilesFromBook(filePath);
                    var parser = new CalibreContentParser(ebook.ContentFilePath);
                    var bibliography = parser.ParseForBibliography();
                    builder.SetBibliography(bibliography);

                    if (ebook.ChaptersFilePaths.Count >= 1)
                    {
                        var metadata = new BookPrefaceParser(ebook.ChaptersFilePaths[0]);
                        builder.SetMetadata(metadata.GetMetadataElements());
                    }

                    if (ebook.ChaptersFilePaths.Count >= 2)
                    {
                        var chapter = new BookChapterParser(ebook.ChaptersFilePaths[1]);
                        builder.SetDescription(chapter.GetDescription());
                    }

                    result = builder.Build();
                    result.LocalFilePath = filePath;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Error extracting files from {filePath}", ex);
                }

                LoadingProgress?.Report(new LoadProgressModel()
                {
                    IncrementCount = true
                });

                return result;
            });
        }
        #endregion LoadPublicationAsync

        #region GetAllPublicationFilePaths
        private async Task<IEnumerable<string>> GetAllPublicationFilePaths(string directory)
        {
            var result = new List<string>();

            foreach (var file in Directory.GetFiles(directory, "*.epub"))
            {
                result.Add(file);
            }

            foreach (var childDirectory in Directory.GetDirectories(directory))
            {
                result.AddRange(await GetAllPublicationFilePaths(childDirectory));
            }

            return result;
        }
        #endregion GetAllPublicationFilePaths

        #region LoadPublicationsFromCatalogDirectoryAsync
        private async Task<IEnumerable<Task<EpubDisplayModel>>> LoadPublicationsFromCatalogDirectoryAsync(string directory)
        {
            var result = new List<Task<EpubDisplayModel>>();
            var filePaths = await GetAllPublicationFilePaths(directory);
            foreach (var file in filePaths)
            {
                result.Add(LoadPublicationAsync(file));
            }
            return result;
        }
        #endregion LoadPublicationsFromCatalogDirectoryAsync

        #region GetAllResultsAsync
        public Task<IEnumerable<Task<EpubDisplayModel>>> GetAllResultsAsync()
        {
            return LoadPublicationsFromCatalogDirectoryAsync(_Settings.BaseCatalogDirectory);
        }
        #endregion GetAllResultsAsync

        #region GetSampleFilteredResultsAsync
        public Task<IEnumerable<EpubDisplayModel>> GetSampleFilteredResultsAsync(FilterModel model)
        {
            IEnumerable<EpubDisplayModel> result = new List<EpubDisplayModel>()
            {
                new EpubDisplayModel()
                {
                    Author="author1",
                    Title = "this is the title",
                    WorkId = "123456",
                    WorkUrl = "https://www.duckduckgo.com",
                    SeriesUrl = "https://www.duckduckgo.com",
                    Rating = RatingType.GeneralAudiences,
                    FandomTags = new string[] { "one", "two" },
                    RelationshipTags = new string[] { "one/two", "three&four" },
                    CharacterTags = new string[] { "one", "two", "three", "four" },
                    AdditionalTags = new string[] { "additional" },
                    Series = new EpubSeriesModel[] { new EpubSeriesModel() {
                        Title = "best works",
                        Index = 1,
                        Url = "htp"
                    } },
                    Statistics = new EpubStatisticsModel()
                    {
                        DatePublished = new DateTime(2021, 4, 13),
                        DateCompleted = new DateTime(2021, 6, 8),
                        DateUpdated = new DateTime(2021, 6, 8),
                        ChaptersReleased = 4,
                        TotalChapters = 4,
                        Words = 10688
                    },
                    Description = "<p><b>summary</b>the first part of the summary says this</p>",
                    LocalFilePath = "file path"
                },
                new EpubDisplayModel()
                {
                    Author="bestauthor",
                    Title = "title number two",
                    WorkId = "789456",
                    WorkUrl = "https://www.duckduckgo.com",
                    SeriesUrl = "https://www.duckduckgo.com",
                    Rating = RatingType.Teen,
                    WarningTags = new string[] { "a warning"},
                    FandomTags = new string[] { "one", "two", "three", "four" },
                    RelationshipTags = new string[] { "one/two", "three&four" },
                    CharacterTags = new string[] { "one", "two", "three", "four" },
                    AdditionalTags = new string[] { "additional" },
                    Series = new EpubSeriesModel[] { },
                    Statistics = new EpubStatisticsModel()
                    {
                        DatePublished = new DateTime(2021, 4, 13),
                        DateCompleted = new DateTime(2021, 6, 8),
                        DateUpdated = new DateTime(2021, 6, 8),
                        ChaptersReleased = 4,
                        TotalChapters = 4,
                        Words = 10688
                    },
                    Description = "<p><b>summary</b>this is the summary text of the second part of the text</p>",
                    LocalFilePath = "file path"
                }
            };

            return Task.FromResult(result);
        }
        #endregion GetSampleFilteredResultsAsync

        #region MarkAddToCalibreAsync
        public async Task MarkAddToCalibreAsync(EpubDisplayModel displayModel)
        {
            if (displayModel != null)
            {
                await AddProcessTagAsync(displayModel, ProcessTagType.InCalibre);

                string calibreDirectory = _Settings.MoveToCalibreDirectory;
                if (!string.IsNullOrEmpty(calibreDirectory) &&
                    !string.IsNullOrEmpty(displayModel.LocalFilePath) &&
                    File.Exists(displayModel.LocalFilePath))
                {
                    await Task.Run(() =>
                    {
                        if (!Directory.Exists(calibreDirectory))
                        {
                            Directory.CreateDirectory(calibreDirectory);
                        }

                        string newFilePath = Path.Combine(calibreDirectory, Path.GetFileName(displayModel.LocalFilePath));
                        if (!File.Exists(newFilePath))
                        {
                            File.Copy(displayModel.LocalFilePath, newFilePath);
                        }
                    });
                }
            }
        }
        #endregion MarkAddToCalibreAsync

        #endregion Methods...
    }
}
