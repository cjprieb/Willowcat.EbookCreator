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

namespace Willowcat.EbookCreator.Engines
{
    public class MergeBooksPublicationEngine
    {
        #region Member Variables...
        protected readonly ILogger<MergeBooksPublicationEngine> _Logger;

        private readonly EpubBuilder _EpubBuilder;
        private readonly EpubOptions _Options;
        #endregion Member Variables...

        #region Properties...

        #region Series
        public SeriesModel Series { get; set; }
        #endregion Series

        #endregion Properties...

        #region Constructors...

        #region MergeBooksPublicationEngine
        public MergeBooksPublicationEngine(ILogger<MergeBooksPublicationEngine> logger, EpubBuilder epubBuilder, EpubOptions options)
        {
            _Logger = logger ?? new NullLogger<MergeBooksPublicationEngine>();
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
        private TableOfContentsLinkModel BuildChapterEntry(string filePath)
        {
            var parser = new BookChapterParser(filePath);
            var chapterTitle = parser.GetChapterTitle();
            var fileName = Path.GetFileName(filePath);
            return new TableOfContentsLinkModel(chapterTitle ?? "Title Page", new FileItemModel(fileName, MediaType.HtmlXml));
        }
        #endregion BuildChapterEntry

        #region BuildEntryFromBookFiles
        private TableOfContentsLinkModel BuildEntryFromBookFiles(ExtractedEpubFilesModel bookFiles)
        {
            List<TableOfContentsLinkModel> chapterEntries = new List<TableOfContentsLinkModel>();
            foreach (var filePath in bookFiles.ChaptersFilePaths)
            {
                chapterEntries.Add(BuildChapterEntry(filePath));
            }

            var lastChapter = chapterEntries.Last();
            if (lastChapter.Name == "Afterword")
            {
                chapterEntries.Remove(lastChapter);
            }

            string bookTitle = GetBookTitle(bookFiles.ContentFilePath);
            AdjustChapterTitles(bookTitle, chapterEntries);

            var result = new TableOfContentsLinkModel(bookTitle, chapterEntries.First().FileItem);
            result.ChildEntries.AddRange(chapterEntries);
            return result;
        }
        #endregion BuildEntryFromBookFiles

        #region CreateBibliography
        private BibliographyModel CreateBibliography(string contentFilePath)
        {
            CalibreContentParser parser = new CalibreContentParser(contentFilePath);
            BibliographyModel bibliography = parser.ParseForBibliography();
            bibliography.Series = Series.SeriesName;
            bibliography.SeriesIndex = Series.SeriesIndex;
            return bibliography;
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

                foreach (var bookFiles in listOfBookFiles)
                {
                    UpdateStylesheetReferences(bookFiles);

                    result.TableOfContents.OtherFiles.AddRange(bookFiles.GetOtherFiles());
                    result.TableOfContents.ChapterFiles.AddRange(bookFiles.GetChapterFiles());
                    result.TableOfContents.Entries.Add(BuildEntryFromBookFiles(bookFiles));
                }
                result.Bibliography = CreateBibliography(listOfBookFiles.First().ContentFilePath);
            }

            return result;
        }
        #endregion CreateBookItemData

        #region DownloadOriginalEbooks
        private async Task DownloadOriginalEbooks(string sourceDirectory, string url)
        {
            Console.WriteLine($"  Getting ebook links from \"{url}\"...");
            Directory.CreateDirectory(sourceDirectory);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                IEnumerable<WorkModel> works = Series.FilterWorksToInclude(ParseForBookUrls(content));

                foreach (var work in works)
                {
                    string fileName = Path.Combine(sourceDirectory, $"{work.Index:D2}-{work.Title.Replace(":", "")}.epub");
                    if (!File.Exists(fileName))
                    {
                        Console.WriteLine($"  Downloading \"{work.EpubUrl}\" to {fileName}...");

                        using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        using (var webStream = client.GetStreamAsync(work.EpubUrl).Result)
                        {
                            webStream.CopyTo(fileStream);
                        }
                        Console.WriteLine($"  \"{fileName}\" downloaded");
                    }
                }
            }
        }
        #endregion DownloadOriginalEbooks

        #region ExtractFilesFromBook
        private List<ExtractedEpubFilesModel> ExtractFilesFromBook(string sourceDirectory, string tempDirectory)
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
            List<ExtractedEpubFilesModel> bookfiles = new List<ExtractedEpubFilesModel>();
            IEnumerable<string> files = Directory.GetFiles(sourceDirectory, "*.epub").OrderBy(x => x);
            if (files.Any())
            {
                var unzipper = new CalibreEpubUnzipper(tempDirectory);
                var seriesIndex = 1;
                foreach (var file in files)
                {
                    var bookFiles = unzipper.ExtractFilesFromBook(file, seriesIndex);
                    if (bookFiles != null)
                    {
                        bookfiles.Add(bookFiles);// BuildEntryFromBookFiles(Path.GetFileNameWithoutExtension(file), chapters));
                        seriesIndex++;
                    }
                }
            }
            return bookfiles;
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
                    var encodedWorkTitle = Uri.EscapeDataString(workTitle);
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

            if (!string.IsNullOrEmpty(Series.SeriesUrl) && _Options.OverwriteOriginalFiles)
            {
                await DownloadOriginalEbooks(filePaths.SourceDirectory, Series.SeriesUrl);
            }
            var bookItemData = CreateBookItemData(filePaths.SourceDirectory, filePaths.StagingDirectory);
            bookItemData.WordsReadPerMinute = _Options.WordsReadPerMinute;
            _EpubBuilder.Create(bookItemData, filePaths.StagingDirectory, filePaths.EpubFilePath);
        }
        #endregion PublishAsync

        #region UpdateStylesheetReferences
        private void UpdateStylesheetReferences(ExtractedEpubFilesModel bookFiles)
        {
            Dictionary<string, string> stylesheetReplacement = bookFiles.Stylesheets.ToDictionary(x => x.Key, x => Path.GetFileName(x.Value));
            foreach (var chapterFile in bookFiles.ChaptersFilePaths)
            {
                string htmlText = File.ReadAllText(chapterFile);
                foreach (var kvp in stylesheetReplacement)
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
}