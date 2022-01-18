using Ionic.Zip;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Willowcat.EbookCreator.Epub
{
    public class EpubZippedFile
    {
        #region Member Variables...
        string _EpubFilePath;

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #region EpubZippedFileEditor
        public EpubZippedFile(string epubFilePath)
        {
            _EpubFilePath = epubFilePath;
        }
        #endregion EpubZippedFileEditor

        #endregion Constructors...

        #region Methods...

        #region UpdateContentFile
        public bool UpdateContentFile(Func<ContentFileMetadataEditor, bool> updateContentFile)
        {
            bool fileUpdated = false;

            using (ZipFile zip = ZipFile.Read(_EpubFilePath))
            {
                var contentFileEntry = zip.FirstOrDefault(entry => entry.FileName.EndsWith(".opf"));
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
                            zip.UpdateEntry(contentFileEntry.FileName, Encoding.UTF8.GetBytes(newOpfFileContents));
                            zip.Save();
                            fileUpdated = true;
                        }
                    }
                }
            }
            return fileUpdated;
        }
        #endregion UpdateContentFile

        #endregion Methods...
    }
}
