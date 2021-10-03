using Microsoft.VisualStudio.TestTools.UnitTesting;
using Willowcat.EbookCreator.Epub;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Willowcat.EbookCreator.Engines;
using Willowcat.EbookCreator.Models;
using System.Threading.Tasks;
using Willowcat.EbookCreator.EPub;
using System.IO;

namespace Willowcat.EbookCreator.Epub.Tests
{
    [TestClass()]
    public class EpubBuilderTests
    {        
        private static MergeBooksPublicationEngine GetMergeBooksPublicationEngine()
        {
            return new MergeBooksPublicationEngine(null, new EpubBuilder(null), false);
        }

        [TestMethod()]
        public async Task CreateTest()
        {
            var series = new SeriesModel()
            {
                SeriesUrl = "https://archiveofourown.org/series/1311746",
                SeriesName = "The Desert Storm",
                SeriesIndex = 2
            };
            series.SetWorkIndexes(2, 6);
            var paths = new EbookPaths("Padawan Training");

            var publicationEngine = GetMergeBooksPublicationEngine();
            await publicationEngine.PublishAsync(series, paths.ToFilePaths());
        }
    }

    class EbookPaths
    {
        private const string _EBookDirectory = @"D:\Users\Crystal\Personal\eBooks";
        private readonly string _BookName = "";

        #region EpubFilePath
        public string EpubFilePath => Path.Combine(_EBookDirectory, _BookName, $"{_BookName}.epub");
        #endregion EpubFilePath

        #region InputDirectory
        public string InputDirectory => Path.Combine(_EBookDirectory, _BookName, "original");
        #endregion InputDirectory

        #region OutputDirectory
        public string OutputDirectory => Path.Combine(_EBookDirectory, _BookName, "epub files");
        #endregion OutputDirectory

        public EbookPaths(string bookName)
        {
            _BookName = bookName;
        }

        public EpubFilePaths ToFilePaths()
        {
            return new EpubFilePaths()
            {
                SourceDirectory = InputDirectory,
                StagingDirectory = OutputDirectory,
                EpubFilePath = EpubFilePath
            };
        }
    }
}