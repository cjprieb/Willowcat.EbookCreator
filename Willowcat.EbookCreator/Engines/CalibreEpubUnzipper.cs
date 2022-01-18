using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Engines
{
    public class CalibreEpubUnzipper
    {
        private readonly string _OutputDirectory;

        public int? NumberOfChapterFilesToInclude { get; set; } = null;

        public CalibreEpubUnzipper(string outputDirectory)
        {
            _OutputDirectory = outputDirectory;
        }

        #region ExtractFilesFromBook
        public ExtractedEpubFilesModel ExtractFilesFromBook(string epubPath, int? seriesIndex = null)
        {
            List<string> chapterOutputPaths = new List<string>();
            List<ZipEntry> chapterEntries = new List<ZipEntry>();
            Dictionary<string, string> stylesheets = new Dictionary<string, string>();
            string contentFilePath = null;
            //string temporaryDirectory = GetTempoaryOutputDirectory(epubPath);

            using (ZipFile zip = ZipFile.Read(epubPath))
            {
                foreach (ZipEntry e in zip)
                {
                    if (IsStyleSheet(e))
                    {
                        string stylesheetPath = ExtractFile(e, epubPath);
                        if (seriesIndex.HasValue)
                        {
                            (string oldName, string newPath) = RenameStylesheet(stylesheetPath, seriesIndex.Value);
                            stylesheets[oldName] = newPath;
                        }
                        else
                        {
                            stylesheets[stylesheetPath] = stylesheetPath;
                        }
                    }
                    else if (IsChapterFile(e))
                    {
                        chapterEntries.Add(e);
                    }
                    else if (IsContentFile(e))
                    {
                        contentFilePath = ExtractFile(e, epubPath);
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
                        chapterOutputPaths.Add(ExtractFile(e, epubPath));
                    }
                }
            }
            chapterOutputPaths.Sort();
            return new ExtractedEpubFilesModel()
            {
                ChaptersFilePaths = chapterOutputPaths,
                Stylesheets = stylesheets,
                OriginalEpubFileName = epubPath,
                ContentFilePath = contentFilePath
            };
        }
        #endregion ExtractFilesFromBook

        #region ExtractFile
        private  string ExtractFile(ZipEntry e, string epubPath)
        {
            string epubFileName = Path.GetFileNameWithoutExtension(epubPath).Trim();
            string outputDirectory = Path.Combine(_OutputDirectory, epubFileName);
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

        #region RenameStylesheet
        private (string oldName, string newName) RenameStylesheet(string outputFilePath, int seriesIndex)
        {
            string oldName = Path.GetFileName(outputFilePath);
            string newName = $"{Path.GetFileNameWithoutExtension(outputFilePath)}_{seriesIndex}.css";
            string newFilePath = Path.Combine(Path.GetDirectoryName(outputFilePath), newName);
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }
            File.Move(outputFilePath, newFilePath);
            return (oldName, newFilePath);
        }
        #endregion RenameStylesheet
    }
}
