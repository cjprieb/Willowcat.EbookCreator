using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public class DefaultTableOfContentsGenerator : ITableOfContentsGenerator
    {
        #region Member Variables...

        private const string ncxNamespaceUrl = "http://www.daisy.org/z3986/2005/ncx/";
        private static XNamespace ncxNamespace = ncxNamespaceUrl;

        private int _NextAvailableIndexId = 1;

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region CreateNavPointElement
        private XElement CreateNavPointElement(XNamespace ncxNamespace, TableOfContentsLinkModel chapter)
        {
            List<XElement> nestedElements = new List<XElement>();
            foreach (var item in chapter.ChildEntries)
            {
                XElement nestedNavPoint = CreateNavPointElement(ncxNamespace, item);
                nestedElements.Add(nestedNavPoint);
            }
            XElement content = new XElement(ncxNamespace + "navPoint",
                new XAttribute("id", $"num_{_NextAvailableIndexId}"),
                new XAttribute("playOrder", _NextAvailableIndexId.ToString()),
                new XElement(ncxNamespace + "navLabel", new XElement(ncxNamespace + "text", new XText(chapter.Name))),
                new XElement(ncxNamespace + "content", new XAttribute("src", chapter.FileItem.RelativeFilePath)),
                nestedElements
            );
            _NextAvailableIndexId++;
            return content;
        }
        #endregion CreateNavPointElement

        #region CreateTableOfContents
        public FileItemModel CreateTableOfContents(BibliographyModel bibliography, TableOfContentsModel tableOfContents)
        {
            var doc = new XElement(ncxNamespace + "ncx",
                new XAttribute("xmlns", ncxNamespaceUrl),
                new XAttribute("version", "2005-1"),
                new XAttribute(XNamespace.Xml + "lang", "en-US")
            );

            //<head>
            doc.Add(new XElement(ncxNamespace + "head",
                new XElement(ncxNamespace + "meta",
                    new XAttribute("content", bibliography?.Guid.ToString() ?? string.Empty),
                    new XAttribute("name", "dtb:uid")
                )
            ));

            //<docTitle>
            doc.Add(new XElement(ncxNamespace + "docTitle",
                new XElement(ncxNamespace + "title", new XText(bibliography?.Title ?? "untitled"))
            ));

            var navMap = new XElement(ncxNamespace + "navMap");
            doc.Add(navMap);

            if (tableOfContents != null)
            {
                _NextAvailableIndexId = 1; // reset as we're recreating the file
                foreach (var chapter in tableOfContents.Entries)
                {
                    XElement navPoint = CreateNavPointElement(ncxNamespace, chapter);
                    navMap.Add(navPoint);
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("<?xml version='1.0' encoding='utf-8'?>\n");
            builder.Append(doc.ToString());
            return new FileItemModel("toc.ncx", MediaType.NavXml)
            {
                FileContents = builder.ToString()
            };
        }
        #endregion CreateTableOfContents

        #endregion Methods...
    }
}
