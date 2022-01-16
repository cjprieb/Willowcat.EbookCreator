using CsQuery;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        #endregion Member Variables...

        #region Properties...

        #region Series
        public SeriesModel Series { get; set; }
        #endregion Series

        #endregion Properties...

        #region Constructors...

        #region MergeBooksPublicationEngine
        public MergeBooksPublicationEngine(EpubBuilder epubBuilder, EpubOptions options)
            : this(null, epubBuilder, options)
        {
        }
        #endregion MergeBooksPublicationEngine

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
            if (chapterTitle == "Preface" || chapterTitle == "Afterword")
            {
                return null;
            }
            return new TableOfContentsLinkModel(chapterTitle ?? "Title Page", new FileItemModel(fileName, MediaType.HtmlXml));
        }
        #endregion BuildChapterEntry

        #region BuildEntryFromBookFiles
        private TableOfContentsLinkModel BuildEntryFromBookFiles(ExtractedEpubFilesModel bookFiles)
        {
            List<TableOfContentsLinkModel> chapterEntries = new List<TableOfContentsLinkModel>();
            foreach (var filePath in bookFiles.ChaptersFilePaths)
            {
                var entry = BuildChapterEntry(filePath);
                if (entry != null)
                {
                    chapterEntries.Add(entry);
                }
            }

            //var lastChapter = chapterEntries.Last();
            //if (lastChapter.Name == "Afterword")
            //{
            //    chapterEntries.Remove(lastChapter);
            //}

            string bookTitle = GetBookTitle(bookFiles.ContentFilePath);
            AdjustChapterTitles(bookTitle, chapterEntries);

            var result = new TableOfContentsLinkModel(bookTitle, chapterEntries.First().FileItem);
            result.ChildEntries.AddRange(chapterEntries);
            return result;
        }
        #endregion BuildEntryFromBookFiles

        #region CreateBibliography
        private BibliographyModel CreateBibliography(List<ExtractedEpubFilesModel> combinedEbooks)
        {
            BibliographyModel masterBibliography = null;
            List<BibliographyModel> childBibliographies = new List<BibliographyModel>();
            foreach (var ebook in combinedEbooks)
            {
                CalibreContentParser parser = new CalibreContentParser(ebook.ContentFilePath);
                var bibliography = parser.ParseForBibliography();
                if (masterBibliography == null)
                {
                    masterBibliography = bibliography;
                }
                childBibliographies.Add(bibliography);
            }
            MergeBibliographies(masterBibliography, childBibliographies);
            if (masterBibliography != null)
            {
                masterBibliography.Series = Series.SeriesName;
                masterBibliography.SeriesIndex = Series.SeriesIndex;
                if (!string.IsNullOrEmpty(Series.OverrideBookTitle))
                {
                    masterBibliography.Title = Series.OverrideBookTitle;
                }
            }
            return masterBibliography;
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
                    try
                    {
                        var bookFiles = unzipper.ExtractFilesFromBook(file, seriesIndex);
                        if (bookFiles != null)
                        {
                            bookfiles.Add(bookFiles);// BuildEntryFromBookFiles(Path.GetFileNameWithoutExtension(file), chapters));
                            seriesIndex++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException($"Error extracting files from {file}", ex);
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

        #region MergeBibliographies
        private void MergeBibliographies(BibliographyModel masterBibliography, List<BibliographyModel> childBibliographies)
        {
            List<string> creators = new List<string>();
            List<string> tags = new List<string>();
            StringBuilder descriptionBuilder = new StringBuilder();
            descriptionBuilder.Append(masterBibliography.Description);
            descriptionBuilder.AppendLine("<p><strong>Also Includes:</strong></p><ol>");
            foreach (var bibliography in childBibliographies)
            {
                if (!creators.Contains(bibliography.Creator))
                {
                    creators.Add(bibliography.Creator);
                }
                foreach (var tag in bibliography.Tags)
                {
                    if (!tags.Contains(tag))
                    {
                        tags.Add(tag);
                    }
                }
                descriptionBuilder.AppendLine($"<li>{bibliography.Title}</li>");
            }
            descriptionBuilder.AppendLine("</ol>");

            masterBibliography.Description = descriptionBuilder.ToString();
            masterBibliography.Tags.Clear();
            masterBibliography.Tags.AddRange(tags);
            masterBibliography.Creator = string.Join(", ", creators);
            masterBibliography.CreatorSort = masterBibliography.Creator.ToLower();
        }
        #endregion MergeBibliographies

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

            await DownloadOriginalEbooks(filePaths.SourceDirectory);
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