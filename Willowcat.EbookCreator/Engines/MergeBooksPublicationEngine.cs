using CsQuery;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Willowcat.EbookCreator.Epub;
using Willowcat.EbookCreator.EPub;
using Willowcat.EbookCreator.Models;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreator.Engines
{
    public class MergeBooksPublicationEngine
    {
        #region Member Variables...
        protected readonly ILogger<MergeBooksPublicationEngine> _Logger;
        
        private readonly EpubBuilder _EpubBuilder;
        private readonly EpubOptions _Options;
        private readonly BibliographyModelFactory _BibliographyModelFactory;
        #endregion Member Variables...

        #region Properties...

        #region Series
        public SeriesModel Series { get; set; }
        #endregion Series

        #endregion Properties...

        #region Constructors...

        #region MergeBooksPublicationEngine
        public MergeBooksPublicationEngine(ILogger<MergeBooksPublicationEngine> logger, EpubBuilder epubBuilder, EpubOptions options, BibliographyModelFactory factory)
        {
            _Logger = logger ?? new NullLogger<MergeBooksPublicationEngine>();
            _BibliographyModelFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _EpubBuilder = epubBuilder;
            _Options = options ?? new EpubOptions()
            {
                OverwriteOriginalFiles = true
            };
        }
        #endregion MergeBooksPublicationEngine

        #endregion Constructors...

        #region Methods...

        #region AdjustChapterTitles
        private static void AdjustChapterTitles(string bookTitle, List<TableOfContentsLinkModel> chapterEntries)
        {
            var bookTitleEntries = chapterEntries.Where(x => x.Name == bookTitle);
            if (bookTitleEntries.Count() == 1)
            {
                bookTitleEntries.First().Name = bookTitle;
            }
            else if (bookTitleEntries.Count() > 1)
            {
                int i = 1;
                foreach (var missingEntry in bookTitleEntries)
                {
                    missingEntry.Name = $"{bookTitle}: Part {i}";
                }
            }
        }
        #endregion AdjustChapterTitles

        #region BuildChapterEntry
        private (TableOfContentsLinkModel chapterEntry, FileItemModel metadataEntry) BuildChapterEntry(string filePath)
        {
            TableOfContentsLinkModel chapterEntry = null;
            FileItemModel metadataEntry = null;

            var parser = new BookChapterParser(filePath);
            var chapterTitle = parser.GetChapterTitle();
            var fileName = Path.GetFileName(filePath);
            if (chapterTitle == "Preface")
            {
                metadataEntry = new FileItemModel(fileName, MediaType.HtmlXml);
            }
            else if (chapterTitle == "Afterword")
            {
                // skip entry for this page
            }
            else
            {
                chapterEntry = new TableOfContentsLinkModel(chapterTitle ?? "Title Page", new FileItemModel(fileName, MediaType.HtmlXml));
            }
            return (chapterEntry, metadataEntry);
        }
        #endregion BuildChapterEntry

        #region BuildEntryFromBookFiles
        private TableOfContentsLinkModel BuildEntryFromBookFiles(ExtractedEpubFilesModel bookFiles)
        {
            List<TableOfContentsLinkModel> chapterEntries = new List<TableOfContentsLinkModel>();
            FileItemModel firstMetadataPage = null;
            foreach (var filePath in bookFiles.ChaptersFilePaths)
            {
                (var chapterEntry,var metadataEntry) = BuildChapterEntry(filePath);
                if (chapterEntry != null)
                {
                    chapterEntries.Add(chapterEntry);
                }
                else if (firstMetadataPage == null && metadataEntry != null)
                {
                    firstMetadataPage = metadataEntry;
                }
            }

            string bookTitle = GetBookTitle(bookFiles.ContentFilePath);
            AdjustChapterTitles(bookTitle, chapterEntries);

            var result = new TableOfContentsLinkModel(bookTitle, firstMetadataPage);
            result.ChildEntries.AddRange(chapterEntries);
            return result;
        }
        #endregion BuildEntryFromBookFiles

        #region CreateBibliography
        private MergedBibliographyModel CreateBibliography(List<ExtractedEpubFilesModel> combinedEbooks)
        {
            IBibliographyModel masterBibliography = null;
            List<IBibliographyModel> childBibliographies = new List<IBibliographyModel>();
            foreach (var ebook in combinedEbooks)
            {
                CalibreContentParser parser = new CalibreContentParser(ebook.ContentFilePath);
                IBibliographyModel bibliography = parser.ParseForBibliography();
                bibliography = _BibliographyModelFactory.ExtractAdditionalMetadata(ebook, bibliography);
                if (masterBibliography == null)
                {
                    masterBibliography = bibliography;
                }
                else
                {
                    childBibliographies.Add(bibliography);
                }
            }
            MergedBibliographyModel mergedBibliography = MergeBibliographies(masterBibliography, childBibliographies);
            if (mergedBibliography != null)
            {
                mergedBibliography.Series = Series.SeriesName;
                mergedBibliography.SeriesIndex = Series.SeriesIndex;
                if (!string.IsNullOrEmpty(Series.OverrideBookTitle))
                {
                    mergedBibliography.Title = Series.OverrideBookTitle;
                }
            }
            return mergedBibliography;
        }
        #endregion CreateBibliography

        #region CreateBookItemData
        public BookModel CreateBookItemData(string sourceDirectory, string tempDirectory)
        {
            BookModel result = null;

            List<ExtractedEpubFilesModel> listOfBookFiles = ExtractFilesFromBook(sourceDirectory, tempDirectory);
            if (listOfBookFiles != null)
            {
                result = new BookModel()
                {
                    TableOfContents = new TableOfContentsModel()
                };

                _Logger.LogInformation($"Writing {listOfBookFiles.SelectMany(x => x.ChaptersFilePaths).Count()} chapter files");

                IEnumerable<string> otherFiles = new string[] { };
                foreach (var bookFiles in listOfBookFiles)
                {
                    UpdateStylesheetReferences(bookFiles);

                    otherFiles = otherFiles.Union(bookFiles.GetOtherFiles());
                    result.TableOfContents.ChapterFiles.AddRange(bookFiles.GetChapterFiles());
                    result.TableOfContents.Entries.Add(BuildEntryFromBookFiles(bookFiles));
                }
                result.TableOfContents.OtherFiles.AddRange(otherFiles.Distinct().Select(x => new FileItemModel(x, MediaType.Unknown)));
                result.Bibliography = CreateBibliography(listOfBookFiles);
            }

            return result;
        }
        #endregion CreateBookItemData

        #region DownloadOriginalEbooks
        private async Task DownloadOriginalEbooks(string sourceDirectory)
        {
            Directory.CreateDirectory(sourceDirectory);

            using (HttpClient client = new HttpClient())
            {
                IEnumerable<WorkModel> works = null;
                if (Series.WorkUrls != null && Series.WorkUrls.Any())
                {
                    _Logger.Log(LogLevel.Information, $"  Getting ebook links from work urls...");
                    works = Series.GetWorksToInclude();
                }
                else if (!string.IsNullOrEmpty(Series.SeriesUrl))
                {
                    _Logger.Log(LogLevel.Information, $"  Getting ebook links from \"{Series.SeriesUrl}\"...");
                    HttpResponseMessage response = await client.GetAsync(Series.SeriesUrl);
                    string content = await response.Content.ReadAsStringAsync();
                    works = Series.FilterWorksToInclude(ParseForBookUrls(content));
                }
                else
                {
                    throw new ApplicationException("No series url or work urls to download");
                }

                foreach (var work in works)
                {
                    string fileName = Path.Combine(sourceDirectory, PathExtensions.FormatAsEPubFileName(work.Index, work.Title));
                    if (!File.Exists(fileName) || _Options.OverwriteOriginalFiles)
                    {
                        _Logger.Log(LogLevel.Information, $"  Downloading \"{work.EpubUrl}\" to {fileName}...");

                        using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        using (var webStream = client.GetStreamAsync(work.EpubUrl).Result)
                        {
                            webStream.CopyTo(fileStream);
                        }
                        _Logger.Log(LogLevel.Information, $"  \"{fileName}\" downloaded");
                    }
                }
            }
        }
        #endregion DownloadOriginalEbooks

        #region ExtractFilesFromBook
        private List<ExtractedEpubFilesModel> ExtractFilesFromBook(string sourceDirectory, string outputDirectory)
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }
            List<ExtractedEpubFilesModel> listOfBookFiles = new List<ExtractedEpubFilesModel>();
            IEnumerable<string> files = Directory.GetFiles(sourceDirectory, "*.epub").OrderBy(x => x).ToList();
            if (files.Any())
            {
                var unzipper = new CalibreEpubUnzipper();
                var seriesIndex = 1;
                foreach (var file in files)
                {
                    try
                    {
                        var tempDirectory = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(file));
                        var bookFiles = unzipper.ExtractFilesFromBook(file, tempDirectory);
                        if (bookFiles != null)
                        {
                            listOfBookFiles.Add(bookFiles);// BuildEntryFromBookFiles(Path.GetFileNameWithoutExtension(file), chapters));
                            MoveBookFilesToStagingDirectory(bookFiles, seriesIndex, outputDirectory);
                            seriesIndex++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException($"Error extracting files from {file}", ex);
                    }
                }

                ReducePossibleStyleSheets(listOfBookFiles);
            }
            return listOfBookFiles;
        }
        #endregion ExtractFilesFromBook

        #region GetBookTitle
        private string GetBookTitle(string contentFilePath)
        {
            CalibreContentParser parser = new CalibreContentParser(contentFilePath);
            var bibliography = parser.ParseForBibliography();
            return bibliography.Title;
        }
        #endregion GetBookTitle

        #region MergeBibliographies
        private MergedBibliographyModel MergeBibliographies(IBibliographyModel masterBibliography, IEnumerable<IBibliographyModel> childBibliographies)
        {
            MergedBibliographyModel MergedModel = _BibliographyModelFactory.CreateMergedBibliographyModel(masterBibliography);
            foreach (var child in childBibliographies)
            {
                MergedModel.MergeBibliography(child);
            }
            return MergedModel;
        }
        #endregion MergeBibliographies

        #region MoveBookFilesToStagingDirectory
        private void MoveBookFilesToStagingDirectory(ExtractedEpubFilesModel bookFiles, int seriesIndex, string outputDirectory)
        {
            for (int i = 0; i < bookFiles.ChaptersFilePaths.Count; i++)
            {
                string newFilePath = MoveFileToStagingDirectory(bookFiles.ChaptersFilePaths[i], seriesIndex, outputDirectory);
                bookFiles.ChaptersFilePaths[i] = newFilePath;
            }

            string[] keys = bookFiles.Stylesheets.Keys.ToArray();
            foreach (string stylesheetReference in keys)
            {
                string oldFilePath = bookFiles.Stylesheets[stylesheetReference];
                string newFilePath = MoveFileToStagingDirectory(oldFilePath, seriesIndex, outputDirectory);
                bookFiles.Stylesheets[stylesheetReference] = newFilePath;
            }

            string newContentFilePath = Path.Combine(outputDirectory, Path.GetFileName(bookFiles.ContentFilePath));
            if (!File.Exists(newContentFilePath))
            {
                File.Move(bookFiles.ContentFilePath, newContentFilePath);
                bookFiles.ContentFilePath = newContentFilePath;
            }
        }
        #endregion MoveBookFilesToStagingDirectory

        #region MoveBookFilesToStagingDirectory
        private string MoveFileToStagingDirectory(string oldFilePath, int seriesIndex, string outputDirectory)
        {
            string newFilePath = Path.Combine(outputDirectory, Path.GetFileName(oldFilePath));
            if (File.Exists(newFilePath))
            {
                newFilePath = PathExtensions.AddIndexToFileName(newFilePath, seriesIndex);
            }
            File.Move(oldFilePath, newFilePath);
            return newFilePath;
        }
        #endregion MoveBookFilesToStagingDirectory

        #region ParseForBookUrls
        private List<WorkModel> ParseForBookUrls(string content)
        {
            int chapterIndex = 1;
            CQ document = content;
            CQ worksInSeries = document["ul.series.index > li"];

            List<WorkModel> works = new List<WorkModel>();
            Regex workUrlPattern = new Regex(@"/works/(\d+)");

            foreach (IDomObject node in worksInSeries)
            {
                var chapterItem = node.Cq();
                var workLink = chapterItem.Find("a").First();
                var workHref = workLink.Attr("href");
                var match = workUrlPattern.Match(workHref);
                if (match.Success)
                {
                    var workId = match.Groups[1].Value;
                    var workTitle = workLink.Text();
                    if (workTitle.Length > 20)
                    {
                        workTitle = workTitle.Substring(0, 20);
                    }
                    var encodedWorkTitle = Uri.EscapeDataString(workTitle.Replace("?", "").Replace(":", "").Trim());
                    //https://archiveofourown.org/downloads/21982333/Anchor%20Point.epub
                    works.Add(new WorkModel()
                    {
                        Index = chapterIndex,
                        WorkUrl = workHref,
                        Title = workTitle,
                        EpubUrl = $"https://archiveofourown.org/downloads/{workId}/{encodedWorkTitle}.epub"
                    });
                }
                chapterIndex++;
            }

            return works;
        }
        #endregion ParseForBookUrls

        #region PublishAsync
        public async Task PublishAsync(SeriesModel series, EpubFilePaths filePaths)
        {
            Series = series;
            if (Series == null) throw new NullReferenceException(nameof(Series));

            try
            {
                await DownloadOriginalEbooks(filePaths.SourceDirectory);
                var bookItemData = CreateBookItemData(filePaths.SourceDirectory, filePaths.StagingDirectory);
                bookItemData.WordsReadPerMinute = _Options.WordsReadPerMinute;
                _EpubBuilder.Create(bookItemData, filePaths.StagingDirectory, filePaths.EpubFilePath);
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Error, $"Error! {ex.Message}");
            }
        }
        #endregion PublishAsync

        #region ReducePossibleStyleSheets
        private void ReducePossibleStyleSheets(List<ExtractedEpubFilesModel> listOfBookFiles)
        {
            List<Stylesheet> stylesheetsToKeep = new List<Stylesheet>();
            foreach (var bookFiles in listOfBookFiles)
            {
                var keys = bookFiles.Stylesheets.Keys.ToArray();
                foreach (var stylesheetReference in keys)
                {
                    string extractedFilePath = bookFiles.Stylesheets[stylesheetReference];
                    string text = File.ReadAllText(extractedFilePath);
                    bool foundMatch = false;
                    foreach (var stylesheet in stylesheetsToKeep)
                    {
                        if (stylesheet.Text == text)
                        {
                            bookFiles.Stylesheets[stylesheetReference] = stylesheet.FilePath;
                            File.Delete(extractedFilePath);
                            foundMatch = true;
                        }
                    }
                    if (!foundMatch)
                    {
                        stylesheetsToKeep.Add(new Stylesheet(extractedFilePath, text));
                    }
                }
            }
        }
        #endregion ReducePossibleStyleSheets

        #region UpdateStylesheetReferences
        private void UpdateStylesheetReferences(ExtractedEpubFilesModel bookFiles)
        {
            Dictionary<string, string> stylesheetReplacementPaths = bookFiles.Stylesheets.ToDictionary(x => x.Key, x => Path.GetFileName(x.Value));
            foreach (var chapterFile in bookFiles.ChaptersFilePaths)
            {
                string htmlText = File.ReadAllText(chapterFile);
                foreach (var kvp in stylesheetReplacementPaths)
                {
                    // double checking I have a css string
                    if (kvp.Key.EndsWith(".css"))
                    {
                        htmlText = htmlText.Replace(kvp.Key, kvp.Value);
                    }
                }
                File.WriteAllText(chapterFile, htmlText);
            }
        }
        #endregion UpdateStylesheetReferences

        #endregion Methods...
    }

    internal struct Stylesheet
    {
        public string FilePath;
        public string Text;
        public Stylesheet(string filePath, string text)
        {
            FilePath = filePath;
            Text = text;
        }
    }
}