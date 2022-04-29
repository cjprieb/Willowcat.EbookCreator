using System;
using System.Xml.Linq;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Engines
{
    public class CalibreContentParser
    {
        #region Member Variables...

        private readonly XElement _RootElement;

        private const string dcNamespaceUrl = "http://purl.org/dc/elements/1.1/";
        private readonly XNamespace _DcNamespace = dcNamespaceUrl;

        private const string opfNamespaceUrl = "http://www.idpf.org/2007/opf";
        private readonly XNamespace _OpfNamespace = opfNamespaceUrl;
        private readonly bool _SplitByComma;

        #endregion Member Variables...

        #region Constructors...

        /// <summary>
        /// Parses XML at filePath
        /// </summary>
        /// <param name="filePath"></param>
        public CalibreContentParser(string filePath, bool splitCreatorsByComma = false)
        {
            _RootElement = XElement.Load(filePath);
            _SplitByComma = splitCreatorsByComma;
        }
        #endregion Constructors...

        #region ParseForBibliography
        public BibliographyModel ParseForBibliography()
        {
            BibliographyModel bibliography = new BibliographyModel();
            var metadataElement = _RootElement.Element(_OpfNamespace + "metadata");
            foreach (var subElement in metadataElement.Elements())
            {
                if (subElement.Name.Namespace == _DcNamespace)
                {
                    SetDcMetadata(subElement, bibliography);
                }
            }
            return bibliography;
        }
        #endregion ParseForBibliography

        #region SetDcMetadata
        private void SetDcMetadata(XElement subElement, BibliographyModel bibliography)
        {
            string key = subElement.Name.LocalName.ToLower();
            string value = subElement.Value;
            switch (key)
            {
                case "publisher":
                    bibliography.Publisher = value;
                    break;

                case "description":
                    bibliography.Description = value;
                    break;

                case "language":
                    bibliography.Language = value;
                    break;

                case "creator":
                    bibliography.SetCreators(value, _SplitByComma);
                    break;

                case "date":
                    string[] parts = value.Split('T');
                    if (parts.Length == 10)
                    {
                        bibliography.PublishedDate = DateTime.ParseExact(parts[0], "yyyy-MM-dd", null);
                    }
                    // DateTime.ParseExact("2019-11-09", "yyyy-MM-ddTHH:mm:ss+zzz", null)
                    break;

                case "title":
                    bibliography.Title = value;
                    break;

                case "subject":
                    bibliography.Tags.Add(value);
                    break;
            }
        }
        #endregion SetDcMetadata
    }
}
