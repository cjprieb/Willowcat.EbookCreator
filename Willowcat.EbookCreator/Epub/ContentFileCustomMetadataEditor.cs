using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookCreator.Epub
{
    public class ContentFileCustomMetadataEditor
    {
        #region Member Variables...
        private const string opfNamespaceUrl = "http://www.idpf.org/2007/opf";
        private static readonly XNamespace _OpfNamespace = opfNamespaceUrl;

        //private Dictionary<string, XElement> _CustomFieldElements = new Dictionary<string, XElement>();
        private CalibreCustomFields _CustomFields = new CalibreCustomFields();
        private XElement _CustomFieldsMetadataElement = null;

        #endregion Member Variables...

        #region Properties...

        #region RootElement
        protected XElement RootElement { get; private set; }
        #endregion RootElement

        #region Version
        public string Version => RootElement.Attribute("version").Value;
        #endregion Version

        #endregion Properties...

        #region Constructors...

        #region ContentFileCustomMetadataEditor
        public ContentFileCustomMetadataEditor(string xmlString)
        {
            RootElement = XElement.Parse(xmlString, LoadOptions.PreserveWhitespace);
        }
        #endregion ContentFileCustomMetadataEditor

        #endregion Constructors...

        #region Methods...

        #region AddVersion2CustomFieldsMetadataElement
        private void AddVersion2CustomFieldsMetadataElement(KeyValuePair<string, CalibreCustomFieldModel> kvp)
        {
            string jsonValue = JsonSerializer.Serialize(kvp.Value);
            string xmlValue = WebUtility.HtmlEncode(jsonValue);
            XElement userMetadataField = new XElement(_OpfNamespace + "meta", 
                new XAttribute("name", $"calibre:user_metadata:{kvp.Key}"),
                new XAttribute("content", xmlValue)
             );
            _CustomFieldsMetadataElement.Add(userMetadataField);
        }
        #endregion AddVersion2CustomFieldsMetadataElement

        #region AddVersion3CustomFieldsMetadataElement
        private void AddVersion3CustomFieldsMetadataElement()
        {
            XElement userMetadataField = new XElement(_OpfNamespace + "meta", new XAttribute("property", "calibre:user_metadata"));
            userMetadataField.Value = _CustomFields.SerializeToJson();
            _CustomFieldsMetadataElement.Add(userMetadataField);
        }
        #endregion AddVersion3CustomFieldsMetadataElement

        #region BuildXmlString
        public string BuildXmlString()
        {
            SetCustomElements();
            return "<?xml version='1.0' encoding='utf-8'?>\n" + RootElement.ToString();
        }
        #endregion BuildXmlString

        #region MergeIntoExistingCustomFields
        private void MergeIntoExistingCustomFields(CalibreCustomFields customFields)
        {
            foreach (var kvp in customFields)
            {
                if (!_CustomFields.ContainsKey(kvp.Key))
                {
                    _CustomFields[kvp.Key] = kvp.Value;
                }
            }
        }
        #endregion MergeIntoExistingCustomFields

        #region ParseForCustomFields
        private void ParseForCustomFields()
        {
            _CustomFieldsMetadataElement = RootElement.Element(_OpfNamespace + "metadata");
            var opfAttribute = _CustomFieldsMetadataElement.Attribute(_OpfNamespace + "opf");
            if (opfAttribute != null)
            {
                opfAttribute.Remove();
            }
            var elementsToRemove = new List<XElement>();
            foreach (var subElement in _CustomFieldsMetadataElement.Elements())
            {
                var propertyAttribute = subElement.Attribute("property");
                var nameAttribute = subElement.Attribute("name");
                var contentAttribute = subElement.Attribute("content");

                if (propertyAttribute != null && propertyAttribute.Value == "calibre:user_metadata")
                {
                    var customFields = JsonSerializer.Deserialize<CalibreCustomFields>(subElement.Value);
                    MergeIntoExistingCustomFields(customFields);
                    elementsToRemove.Add(subElement);
                }
                else if (nameAttribute != null && contentAttribute != null && nameAttribute.Value.StartsWith("calibre:user_metadata"))
                {
                    string[] tokens = nameAttribute.Value.Split(':');
                    string customFieldName = tokens.Length > 2 ? tokens[2] : null;
                    string customFieldContentValue = WebUtility.HtmlDecode(contentAttribute.Value);
                    if (!_CustomFields.ContainsKey(customFieldName))
                    {
                        CalibreCustomFieldModel calibreCustomFieldModel = JsonSerializer.Deserialize<CalibreCustomFieldModel>(customFieldContentValue);
                        if (calibreCustomFieldModel != null)
                        {
                            _CustomFields[customFieldName] = calibreCustomFieldModel;
                        }
                    }
                    elementsToRemove.Add(subElement);
                }
            }

            if (elementsToRemove.Any())
            {
                foreach (var element in elementsToRemove)
                {
                    element.Remove();
                }
            }
        }
        #endregion ParseForCustomFields

        #region RemoveCustomFieldValue
        public void RemoveCustomFieldValue(string propertyName, bool overrideExistingValue = false)
        {
            if (_CustomFields == null || _CustomFieldsMetadataElement == null)
            {
                ParseForCustomFields();
            }

            if (_CustomFields.ContainsKey(propertyName))
            {
                _CustomFields.Remove(propertyName);
            }
        }
        #endregion RemoveCustomFieldValue

        #region SetCustomElements
        private void SetCustomElements()
        {
            if (Version == "3.0")
            {
                AddVersion3CustomFieldsMetadataElement();
            }
            else if (Version == "2.0")
            {
                foreach (var kvp in _CustomFields)
                {
                    AddVersion2CustomFieldsMetadataElement(kvp);
                }
            }
            else
            {
                throw new ApplicationException($"Unsupported content.opf version: {Version}");
            }
        }
        #endregion SetCustomElements

        #region SetCustomFieldValue
        public void SetCustomFieldValue(CalibreCustomFieldModel customFieldModel, bool overrideExistingValue = false)
        {
            if (_CustomFields == null || _CustomFieldsMetadataElement == null)
            {
                ParseForCustomFields();
            }

            string key = customFieldModel.PropertyName;
            if (!_CustomFields.ContainsKey(key) || !string.IsNullOrEmpty(_CustomFields[key].Value?.ToString()) || overrideExistingValue)
            {
                _CustomFields[key] = customFieldModel;
            }
        }
        #endregion SetCustomFieldValue

        #endregion Methods...
    }
}
