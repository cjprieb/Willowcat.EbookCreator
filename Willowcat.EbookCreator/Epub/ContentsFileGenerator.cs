﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Willowcat.EbookCreator.Models;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreator.Epub
{
    public class ContentsFileGenerator
    {
        #region Member Variables...
        private const string _XmlUtf8Encoding = "<?xml version='1.0' encoding='utf-8'?>\n";

        private const string opfNamespaceUrl = "http://www.idpf.org/2007/opf";
        private static XNamespace opfNamespace = opfNamespaceUrl;

        private const string dcNamespaceUrl = "http://purl.org/dc/elements/1.1/";
        private static XNamespace dcNamespace = dcNamespaceUrl;

        private const string dcTermsNamespaceUrl = "http://purl.org/dc/terms/";
        private static XNamespace dcTermsNamespace = dcTermsNamespaceUrl;

        private const string calibreNamespaceUrl = "http://calibre.kovidgoyal.net/2009/metadata";
        private static XNamespace calibreNamespace = calibreNamespaceUrl;

        private const string xsiNamespaceUrl = "http://www.w3.org/2001/XMLSchema-instance";
        private static XNamespace xsiNamespace = xsiNamespaceUrl;

        private IBibliographyModel _Bibliography;
        private ContentsFileModel _ContentsModel;

        #endregion Member Variables...

        #region Properties...

        private CoverModel Cover => _ContentsModel.Cover;
        private FileItemModel TableOfContentsPage => _ContentsModel.TableOfContentsPage;

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region CreateContentsFile
        public virtual FileItemModel CreateContentsFile(IBibliographyModel bibliography, ContentsFileModel contentsModel)
        {
            _Bibliography = bibliography;
            _ContentsModel = contentsModel;

            var referenceElements = GetReferenceElements();
            var metadataElements = GetMetadataElements();
            var manifestList = _ContentsModel.GetManifestList();
            var spineList = _ContentsModel.GetSpineList();

            var doc = new XElement(opfNamespace + "package",
                new XAttribute("xmlns", opfNamespaceUrl),
                new XAttribute("version", "3.0"),
                new XAttribute(XNamespace.Xml + "lang", "en-US"),
                new XAttribute("unique-identifier", "uid"),
                new XElement(opfNamespace + "metadata",
                    new XAttribute(XNamespace.Xmlns + "calibre", calibreNamespace),
                    new XAttribute(XNamespace.Xmlns + "dc", dcNamespaceUrl),
                    new XAttribute(XNamespace.Xmlns + "dcterms", dcTermsNamespaceUrl),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsiNamespaceUrl),
                    metadataElements
                ),
                new XElement(opfNamespace + "manifest",
                    from fileItem in manifestList
                    select
                        new XElement(opfNamespace + "item",
                            new XAttribute("href", fileItem.RelativeFilePath),
                            new XAttribute("id", fileItem.Id),
                            new XAttribute("media-type", fileItem.MediaTypeString)
                        )

                ),
                new XElement(opfNamespace + "spine",
                    new XAttribute("toc", TableOfContentsPage?.Id ?? string.Empty),
                    from fileItem in spineList
                    select
                        new XElement(opfNamespace + "itemref", new XAttribute("idref", fileItem.Id))
                ),
                new XElement(opfNamespace + "guide", referenceElements)
            );

            StringBuilder builder = new StringBuilder();
            builder.Append(_XmlUtf8Encoding);
            builder.Append(doc.ToString());

            return new FileItemModel("content.opf", MediaType.NavXml)
            {
                FileContents = builder.ToString()
            };
        }
        #endregion CreateContentsFile

        #region CreateReferenceElement
        private XElement CreateReferenceElement(string type, string title, FileItemModel fileModel)
        {
            return new XElement(opfNamespace + "reference",
                new XAttribute("type", type),
                new XAttribute("title", title),
                new XAttribute("href", fileModel.RelativeFilePath)
            );
        }
        #endregion CreateReferenceElement

        #region GetBibliographyElements
        private IEnumerable<XElement> GetBibliographyElements()
        {
            if (_Bibliography != null)
            {
                yield return new XElement(dcNamespace + "identifier",
                    new XAttribute("id", "uid"),
                    new XAttribute(opfNamespace + "scheme", "uuid"),
                    new XText(_Bibliography.Guid.ToString())
                );

                yield return new XElement(dcNamespace + "title", new XText(_Bibliography.Title));

                yield return new XElement(dcNamespace + "language", new XText(_Bibliography.Language));

                yield return new XElement(dcNamespace + "creator",
                    new XAttribute(opfNamespace + "role", "aut"),
                    new XAttribute(opfNamespace + "file-as", _Bibliography.CreatorSort),
                    new XText(BibliographyModel.FormatCreatorList(_Bibliography.Creators))
                );

                yield return new XElement(dcNamespace + "publisher", new XText(_Bibliography.Publisher));

                yield return new XElement(dcNamespace + "description", new XText(_Bibliography.Description));

                yield return new XElement(dcNamespace + "date",
                    new XAttribute(opfNamespace + "event", "published"),
                    new XText(_Bibliography.PublishedDate.ToString("yyyy-MM-dd"))
                );

                foreach (var tag in _Bibliography.Tags)
                {
                    yield return new XElement(dcNamespace + "subject", new XText(tag));
                }
            }
        }
        #endregion GetBibliographyElements

        #region GetMetadataElements
        private List<XElement> GetMetadataElements()
        {
            List<XElement> MetadataElements = new List<XElement>();

            MetadataElements.AddRange(GetBibliographyElements());

            if (_Bibliography.CustomFields != null)
            {
                MetadataElements.Add(new XElement(opfNamespace + "meta",
                    new XAttribute("property", "calibre:user_metadata"),
                    _Bibliography.CustomFields.SerializeToJson()
                ));
            }

            if (Cover != null && !string.IsNullOrEmpty(Cover.CoverImage.Id))
            {
                MetadataElements.Add(new XElement(opfNamespace + "meta",
                    new XAttribute("name", "cover"),
                    new XAttribute("content", Cover.CoverImage.Id)
                ));
            }

            MetadataElements.AddRange(GetSeriesBibliographyElements());

            return MetadataElements;
        }
        #endregion GetMetadataElements

        #region GetReferenceElements
        private List<XElement> GetReferenceElements()
        {
            List<XElement> referenceElements = new List<XElement>();

            if (Cover?.CoverHtmlPage != null)
            {
                referenceElements.Add(CreateReferenceElement("cover", "Cover", Cover.CoverHtmlPage));
            }

            if (TableOfContentsPage != null)
            {
                referenceElements.Add(CreateReferenceElement("toc", "Table of Contents", TableOfContentsPage));
            }

            return referenceElements;
        }
        #endregion GetReferenceElements

        #region GetSeriesBibliographyElements
        private IEnumerable<XElement> GetSeriesBibliographyElements()
        {
            if (_Bibliography != null && !string.IsNullOrEmpty(_Bibliography.Series))
            {

                yield return new XElement(opfNamespace + "meta",
                    new XAttribute("property", "belongs-to-collection"),
                    new XAttribute("id", "id-2"),
                    _Bibliography.Series
                );

                yield return new XElement(opfNamespace + "meta",
                    new XAttribute("refines", "#id-2"),
                    new XAttribute("property", "collection-type"),
                    "series"
                );

                if (_Bibliography.SeriesIndex.HasValue)
                {
                    yield return new XElement(opfNamespace + "meta",
                        new XAttribute("refines", "#id-2"),
                        new XAttribute("property", "group-position"),
                        _Bibliography.SeriesIndex.Value.ToString()
                    );
                }
            }
        }
        #endregion GetSeriesBibliographyElements

        #endregion Methods...
    }
}
