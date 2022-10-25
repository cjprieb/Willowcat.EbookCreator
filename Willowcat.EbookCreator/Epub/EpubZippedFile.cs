using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Willowcat.EbookCreator.Epub
{
    public class EpubZippedFile : IDisposable
    {
        #region Member Variables...
        private string _EpubFilePath;
        private ZipFile _ZipFile = null;

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #region EpubZippedFileEditor
        public EpubZippedFile(string epubFilePath)
        {
            _EpubFilePath = epubFilePath;
            _ZipFile = ZipFile.Read(_EpubFilePath);
        }
        #endregion EpubZippedFileEditor

        #endregion Constructors...

        #region Dispose
        public void Dispose()
        {
            if (_ZipFile != null)
            {
                _ZipFile.Dispose();
            }
        }
        #endregion Dispose

        #region Methods...

        #region ProcessChapterFiles
        public void ProcessChapterFiles(Action<MemoryStream> processHtmlStream)
        {
            foreach (ZipEntry e in _ZipFile.Where(entry => entry.FileName.EndsWith("html")))
            {
                using (var stream = new MemoryStream())
                {
                    e.Extract(stream);
                    processHtmlStream(stream);
                }
            }
        }
        #endregion ProcessChapterFiles

        #region UpdateContentFile
        public bool UpdateContentFile(Func<ContentFileMetadataEditor, bool> updateContentFile)
        {
            bool fileUpdated = false;

            var contentFileEntry = _ZipFile.FirstOrDefault(entry => entry.FileName.EndsWith(".opf"));
            if (contentFileEntry != null)
            {
                using (var stream = new MemoryStream())
                {
                    contentFileEntry.Extract(stream);

                    var xmlData = Encoding.UTF8.GetString(stream.ToArray());
                    ContentFileMetadataEditor editor = new ContentFileMetadataEditor(xmlData);
                    if (updateContentFile(editor))
                    {
                        var newOpfFileContents = editor.BuildXmlString();
                        _ZipFile.UpdateEntry(contentFileEntry.FileName, Encoding.UTF8.GetBytes(newOpfFileContents));
                        _ZipFile.Save();
                        fileUpdated = true;
                    }
                }
            }
            return fileUpdated;
        }
        #endregion UpdateContentFile

        #endregion Methods...
    }
}
