using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Engines
{
    public class CalibreEpubUnzipper
    {
        public int? NumberOfChapterFilesToInclude { get; set; } = null;

        public CalibreEpubUnzipper()
        {
        }

        #region ExtractFilesFromBook
        public ExtractedEpubFilesModel ExtractFilesFromBook(string epubPath, string outputDirectory)
        {
            List<string> chapterOutputPaths = new List<string>();
            List<ZipEntry> chapterEntries = new List<ZipEntry>();
            List<string> stylesheets = new List<string>();
            string contentFilePath = null;

            using (ZipFile zip = ZipFile.Read(epubPath))
            {
                foreach (ZipEntry e in zip)
                {
                    if (IsStyleSheet(e) && !NumberOfChapterFilesToInclude.HasValue)
                    {
                        string stylesheetPath = ExtractFile(e, outputDirectory);
                        stylesheets.Add(stylesheetPath);
                    }
                    else if (IsChapterFile(e))
                    {
                        chapterEntries.Add(e);
                    }
                    else if (IsContentFile(e))
                    {
                        contentFilePath = ExtractFile(e, outputDirectory);
                    }
                }

                chapterEntries = chapterEntries.OrderBy(x => x.FileName).ToList();
                foreach (var e in chapterEntries)
                {
                    if (NumberOfChapterFilesToInclude.HasValue && chapterOutputPaths.Count >= NumberOfChapterFilesToInclude)
                    {
                        // skip;
                    }
                    else
                    {
                        chapterOutputPaths.Add(ExtractFile(e, outputDirectory));
                    }
                }
            }
            chapterOutputPaths.Sort();
            return new ExtractedEpubFilesModel()
            {
                ChaptersFilePaths = chapterOutputPaths,
                Stylesheets = stylesheets.ToDictionary(x => Path.GetFileName(x), x => x),
                OriginalEpubFileName = epubPath,
                ContentFilePath = contentFilePath
            };
        }
        #endregion ExtractFilesFromBook

        #region ExtractFile
        private string ExtractFile(ZipEntry e, string outputDirectory)
        {
            string outputFilePath = Path.Combine(outputDirectory, e.FileName.Replace("/", "\\").Trim());
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
            e.Extract(outputDirectory);
            return outputFilePath;
        }
        #endregion ExtractFile

        #region IsBaseEPubFile
        private bool IsBaseEPubFile(ZipEntry e)
        {
            if (e.FileName == "content.opf") return true;
            if (e.FileName == "META-INF/") return true;
            if (e.FileName == "META-INF/container.xml") return true;
            //if (e.FileName == "stylesheet.css") return true;
            //if (e.FileName == "page_styles.css") return true;
            return false;
        }
        #endregion IsBaseEPubFile

        #region IsChapterFile
        private bool IsChapterFile(ZipEntry e)
        {
            if (e.FileName.EndsWith(".xhtml")) return true;
            return false;
        }
        #endregion IsChapterFile

        #region IsContentFile
        private static bool IsContentFile(ZipEntry e)
        {
            return e.FileName.EndsWith(".opf");
        }
        #endregion IsContentFile

        #region IsStyleSheet
        private bool IsStyleSheet(ZipEntry e)
        {
            if (e.FileName.EndsWith(".css")) return true;
            return false;
        }
        #endregion IsStyleSheet
    }
}
