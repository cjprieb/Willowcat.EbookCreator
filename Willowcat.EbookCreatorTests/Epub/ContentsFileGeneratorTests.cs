using Microsoft.VisualStudio.TestTools.UnitTesting;
using Willowcat.EbookCreator.Epub;
using System;
using System.Collections.Generic;
using System.Text;
using Willowcat.EbookCreator.Models;
using System.IO;
namespace Willowcat.EbookCreatorTests.Epub
{
    [TestClass()]
    public class ContentsFileGeneratorTests
    {
        private ContentsFileGenerator _ContentsFileGenerator = null;

        [TestInitialize] 
        public void TestInititalize()
        {
            _ContentsFileGenerator = new ContentsFileGenerator();
        }

        private static void AreEqual(string expectedContents, string actualContents)
        {
            var expectedLines = expectedContents.Split('\n');
            var actualLines = actualContents.Split('\n');
            for (int i = 0; i < expectedLines.Length; i++)
            {
                if (i < actualLines.Length)
                {
                    Assert.AreEqual(expectedLines[i].Trim(), actualLines[i].Trim(), $"difference at line {i}");
                }
                else
                {
                    Assert.Fail($"expected at least {i+1} lines");
                }
            }
        }

        private static ContentsFileModel GetSampleBookFiles()
        {
            var result = new ContentsFileModel()
            {
                Cover = new CoverModel()
                {
                    CoverHtmlPage = new FileItemModel("cover.html", MediaType.HtmlXml),
                    CoverImage = new FileItemModel("cover.png", MediaType.ImagePng)
                },
                TableOfContentsPage = new FileItemModel("toc.ncx", MediaType.NavXml),
                TitlePage = new FileItemModel("titlepage.html", MediaType.HtmlXml),
            };
            for (int i = 0; i < 5; i++)
            {
                result.ChapterFiles.Add(new FileItemModel($"chapter{i}.html", MediaType.HtmlXml));
            }
            result.OtherFiles.Add(new FileItemModel("styles.css", MediaType.Unknown));
            return result;
        }

        [TestMethod()]
        public void CreateContentsFileTest_Basic()
        {
            var bibliography = new BibliographyModel()
            {
                Title = "the title",
                Description = "<p>a description of the book with <b>formatting</b>.</p>",
                PublishedDate = new DateTime(2015, 3, 5),
                CreatorSort = "creator name",
                Guid = new Guid("1ed2e6a5-acde-4492-9bee-62d0df446cc1"),
                Publisher = "a publishing co"       
            };
            bibliography.SetCreators("name of creator");
            ContentsFileModel bookFiles = GetSampleBookFiles();
            var fileItem = _ContentsFileGenerator.CreateContentsFile(bibliography, bookFiles);
            Assert.AreEqual("content", fileItem.Id, nameof(FileItemModel.Id));
            Assert.AreEqual(MediaType.NavXml, fileItem.MediaType, nameof(FileItemModel.MediaType));
            Assert.AreEqual("content.opf", fileItem.RelativeFilePath, nameof(FileItemModel.RelativeFilePath));
            AreEqual(Properties.Resources.contents_basic, fileItem.FileContents);
        }

        [TestMethod()]
        public void CreateContentsFileTest_CustomFields()
        {
            var bibliography = new BibliographyModel()
            {
                Title = "the title",
                Description = "<p>a description of the book with <b>formatting</b>.</p>",
                PublishedDate = new DateTime(2015, 3, 5),
                CreatorSort = "creator name",
                Guid = new Guid("1ed2e6a5-acde-4492-9bee-62d0df446cc1"),
                Publisher = "a publishing co",
                CustomFields = new CalibreCustomFields()
                {
                    { "#date_read", CalibreCustomFields.CreateDateReadField() },
                    //{ "#dateadded", CalibreCustomFields.CreateDateAddedField(new DateTime(2021, 10, 9, 21, 40) },
                    { "#is_read", CalibreCustomFields.CreateIsReadField() },
                    { "#sync_book", CalibreCustomFields.CreateSyncBookField(true) },
                    { "#read_time", CalibreCustomFields.CreateTimeToReadField(new TimeSpan(3, 45, 0)) },
                }
            };
            bibliography.SetCreators("name of creator");

            ContentsFileModel bookFiles = GetSampleBookFiles();
            var fileItem = _ContentsFileGenerator.CreateContentsFile(bibliography, bookFiles);
            Assert.AreEqual("content", fileItem.Id, nameof(FileItemModel.Id));
            Assert.AreEqual(MediaType.NavXml, fileItem.MediaType, nameof(FileItemModel.MediaType));
            Assert.AreEqual("content.opf", fileItem.RelativeFilePath, nameof(FileItemModel.RelativeFilePath));
            AreEqual(Properties.Resources.contents_customfields, fileItem.FileContents);
        }

        [TestMethod()]
        public void CreateContentsFileTest_Language()
        {
            var bibliography = new BibliographyModel()
            {
                Title = "the title",
                Description = "<p>a description of the book with <b>formatting</b>.</p>",
                PublishedDate = new DateTime(2015, 3, 5),
                CreatorSort = "creator name",
                Guid = new Guid("1ed2e6a5-acde-4492-9bee-62d0df446cc1"),
                Publisher = "a publishing co",
                Language = "deustch",
                Translator = "the translator person"
            };
            bibliography.SetCreators("name of creator");
            ContentsFileModel bookFiles = GetSampleBookFiles();
            var fileItem = _ContentsFileGenerator.CreateContentsFile(bibliography, bookFiles);
            Assert.AreEqual("content", fileItem.Id, nameof(FileItemModel.Id));
            Assert.AreEqual(MediaType.NavXml, fileItem.MediaType, nameof(FileItemModel.MediaType));
            Assert.AreEqual("content.opf", fileItem.RelativeFilePath, nameof(FileItemModel.RelativeFilePath));
            AreEqual(Properties.Resources.contents_language, fileItem.FileContents);
        }

        [TestMethod()]
        public void CreateContentsFileTest_Proofreading()
        {
            var bibliography = new BibliographyModel()
            {
                Title = "the title",
                Description = "<p>a description of the book with <b>formatting</b>.</p>",
                PublishedDate = new DateTime(2015, 3, 5),
                CreatorSort = "creator name",
                Guid = new Guid("1ed2e6a5-acde-4492-9bee-62d0df446cc1"),
                Publisher = "a publishing co",
                Proofreader = "proofing person"
            };
            bibliography.SetCreators("name of creator");
            ContentsFileModel bookFiles = GetSampleBookFiles();
            var fileItem = _ContentsFileGenerator.CreateContentsFile(bibliography, bookFiles);
            Assert.AreEqual("content", fileItem.Id, nameof(FileItemModel.Id));
            Assert.AreEqual(MediaType.NavXml, fileItem.MediaType, nameof(FileItemModel.MediaType));
            Assert.AreEqual("content.opf", fileItem.RelativeFilePath, nameof(FileItemModel.RelativeFilePath));
            AreEqual(Properties.Resources.contents_proofreader, fileItem.FileContents);
        }

        [TestMethod()]
        public void CreateContentsFileTest_Series()
        {
            var bibliography = new BibliographyModel()
            {
                Title = "the title",
                Description = "<p>a description of the book with <b>formatting</b>.</p>",
                PublishedDate = new DateTime(2015, 3, 5),
                CreatorSort = "creator name",
                Guid = new Guid("1ed2e6a5-acde-4492-9bee-62d0df446cc1"),
                Publisher = "a publishing co",
                Series = "series name",
                SeriesIndex = 3
            };
            bibliography.SetCreators("name of creator");
            ContentsFileModel bookFiles = GetSampleBookFiles();
            var fileItem = _ContentsFileGenerator.CreateContentsFile(bibliography, bookFiles);
            Assert.AreEqual("content", fileItem.Id, nameof(FileItemModel.Id));
            Assert.AreEqual(MediaType.NavXml, fileItem.MediaType, nameof(FileItemModel.MediaType));
            Assert.AreEqual("content.opf", fileItem.RelativeFilePath, nameof(FileItemModel.RelativeFilePath));
            AreEqual(Properties.Resources.contents_series, fileItem.FileContents);
        }

        [TestMethod()]
        public void CreateContentsFileTest_Tags()
        {
            var bibliography = new BibliographyModel()
            {
                Title = "the title",
                Description = "<p>a description of the book with <b>formatting</b>.</p>",
                PublishedDate = new DateTime(2015, 3, 5),
                CreatorSort = "creator name",
                Guid = new Guid("1ed2e6a5-acde-4492-9bee-62d0df446cc1"),
                Publisher = "a publishing co"
            };
            bibliography.SetCreators("name of creator");
            bibliography.Tags.Add("action");
            bibliography.Tags.Add("fantasy");
            ContentsFileModel bookFiles = GetSampleBookFiles();
            var fileItem = _ContentsFileGenerator.CreateContentsFile(bibliography, bookFiles);
            Assert.AreEqual("content", fileItem.Id, nameof(FileItemModel.Id));
            Assert.AreEqual(MediaType.NavXml, fileItem.MediaType, nameof(FileItemModel.MediaType));
            Assert.AreEqual("content.opf", fileItem.RelativeFilePath, nameof(FileItemModel.RelativeFilePath));
            AreEqual(Properties.Resources.contents_tags, fileItem.FileContents);
        }
    }
}