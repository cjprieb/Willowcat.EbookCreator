﻿using Ionic.Zip;
using Ionic.Zlib;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Willowcat.EbookCreator.Models;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreator.Epub
{
    public class EpubBuilder
    {
        #region Member Variables...
        private const string _METADATA = "META-INF/container.xml";
        private readonly ILogger<EpubBuilder> _Logger = null;

        private IBibliographyModel _Bibliography;
        private ContentsFileGenerator _ContentsFileGenerator;
        private ICoverGenerator _CoverGenerator;
        private ITableOfContentsGenerator _TableOfContentsGenerator;
        private ITitlePageGenerator _TitlePageGenerator;
        private TableOfContentsModel _TableOfContents;

        #endregion Member Variables...

        #region Properties...

        #region ContentsFileBuilder
        public ContentsFileGenerator ContentsFileGenerator
        {
            get
            {
                if (_ContentsFileGenerator == null)
                {
                    _ContentsFileGenerator = new ContentsFileGenerator();
                }
                return _ContentsFileGenerator;
            }
            set => _ContentsFileGenerator = value;
        }
        #endregion ContentsFileBuilder

        #region CoverGenerator
        public ICoverGenerator CoverGenerator
        {
            get
            {
                if (_CoverGenerator == null)
                {
                    _CoverGenerator = new DefaultCoverGenerator();
                }
                return _CoverGenerator;
            }
            set => _CoverGenerator = value;
        }
        #endregion CoverGenerator

        #region TableOfContentsGenerator
        public ITableOfContentsGenerator TableOfContentsGenerator
        {
            get
            {
                if (_TableOfContentsGenerator == null)
                {
                    _TableOfContentsGenerator = new DefaultTableOfContentsGenerator();
                }
                return _TableOfContentsGenerator;
            }
            set => _TableOfContentsGenerator = value;
        }
        #endregion TableOfContentsGenerator

        #region TitlePageGenerator
        public ITitlePageGenerator TitlePageGenerator
        {
            get
            {
                if (_TitlePageGenerator == null)
                {
                    _TitlePageGenerator = new DefaultTitlePageGenerator();
                }
                return _TitlePageGenerator;
            }
            set => _TitlePageGenerator = value;
        }
        #endregion TitlePageGenerator

        #endregion Properties...

        #region Constructors...

        #region EpubBuilder
        public EpubBuilder() : this(null)
        {
        }
        #endregion EpubBuilder

        #region EpubBuilder
        public EpubBuilder(ILogger<EpubBuilder> logger)
        {
            _Logger = logger ?? new NullLogger<EpubBuilder>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        #endregion EpubBuilder

        #endregion Constructors...

        #region Methods...

        #region Create
        public void Create(BookModel bookModel, string sourceLocation, string outputFilePath)
        {
            _Logger.LogInformation($"Creating zipped epub from { sourceLocation } into { outputFilePath }");

            _Bibliography = bookModel?.Bibliography;
            _TableOfContents = bookModel?.TableOfContents;

            if (_Bibliography == null) throw new ArgumentNullException($"A {nameof(_Bibliography)} file is need to build an epub file.", nameof(_Bibliography));
            if (_TableOfContents == null) throw new ArgumentNullException($"A {nameof(_TableOfContents)} file is need to build an epub file.", nameof(_TableOfContents));

            var contentsFileModel = new ContentsFileModel()
            {
                Cover = CoverGenerator.CreateCover(_Bibliography, _TableOfContents.CoverFileName),
                TableOfContentsPage = TableOfContentsGenerator.CreateTableOfContents(_Bibliography, _TableOfContents),
                TitlePage = TitlePageGenerator.CreateTitlePageFile(_Bibliography)
            };
            contentsFileModel.ChapterFiles.AddRange(_TableOfContents.ChapterFiles);
            contentsFileModel.OtherFiles.AddRange(_TableOfContents.OtherFiles);

            CalibreCustomFieldModel syncBookField = CalibreCustomFields.CreateSyncBookField(true);
            _Bibliography.AddCustomField(syncBookField.PropertyName, syncBookField);

            //Create package.opf file
            contentsFileModel.ContentsFile = ContentsFileGenerator.CreateContentsFile(_Bibliography, contentsFileModel);

            //Create META-INF file
            var metafile = Path.Combine(sourceLocation, _METADATA);
            WriteFileContents(metafile, Properties.Resources.MetafileTemplate);

            WriteFilesToSourceLocation(sourceLocation, contentsFileModel);

            Zip(sourceLocation, outputFilePath, metafile);
        }
        #endregion Create

        #region SetTableOfContentsModel
        public void SetTableOfContentsModel(TableOfContentsModel tableOfContentsModel)
        {
            _TableOfContents = tableOfContentsModel;
        }
        #endregion SetTableOfContentsModel

        #region WriteFilesToSourceLocation
        private void WriteFilesToSourceLocation(string sourceLocation, ContentsFileModel contentsFileModel)
        {
            foreach (var file in contentsFileModel.GetGeneratedFiles())
            {
                if (file != null)
                {
                    var filePath = Path.Combine(sourceLocation, file.RelativeFilePath);
                    WriteFileContents(filePath, file.FileContents);
                }
            }
        }
        #endregion WriteFilesToSourceLocation

        #region WriteFileContents
        private void WriteFileContents(string filePath, string contents)
        {
            if (!string.IsNullOrEmpty(contents))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                _Logger.LogDebug($"Writing file to {filePath}");
                File.WriteAllText(filePath, contents);
            }
        }
        #endregion WriteFileContents

        #region Zip
        private void Zip(string directoryToZip, string outputFilePath, string metafile)
        {
            // Creating ZIP file and writing mimetype
            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(fileStream))
                {
                    var entry = zipStream.PutNextEntry("mimetype");
                    entry.CompressionLevel = CompressionLevel.None;

                    byte[] buffer = Encoding.ASCII.GetBytes("application/epub+zip");
                    zipStream.Write(buffer, 0, buffer.Length);

                    zipStream.PutNextEntry(_METADATA);
                    ZipExistingFile(zipStream, metafile);

                    ZipFilesFromSource(_Logger, zipStream, directoryToZip);
                }
            }
        }
        #endregion Zip

        #region ZipExistingFile
        private static void ZipExistingFile(Stream output, string fileName)
        {
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                int length = -1;
                byte[] buffer = new byte[2048];
                while ((length = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, length);
                }
            }
        }
        #endregion ZipExistingFile

        #region ZipFilesFromSource
        private static void ZipFilesFromSource(ILogger<EpubBuilder> logger, ZipOutputStream zipStream, string sourceDirectory)
        {
            logger.LogDebug($"Adding directory { sourceDirectory } to zip file");
            foreach (var sourceFile in Directory.EnumerateFileSystemEntries(sourceDirectory))
            {
                if (File.Exists(sourceFile))
                {
                    var relativePath = Path.GetFileName(sourceFile);
                    if (relativePath != "mimetype")
                    {
                        logger.LogDebug($"Adding file { relativePath } to zip file");
                        zipStream.PutNextEntry(relativePath);
                        ZipExistingFile(zipStream, sourceFile);
                    }
                }
            }
        }
        #endregion ZipFilesFromSource

        #endregion Methods...
    }
}
