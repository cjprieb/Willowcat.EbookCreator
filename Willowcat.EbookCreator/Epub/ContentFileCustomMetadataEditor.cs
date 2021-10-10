using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        private CalibreCustomFields _CustomFields = new CalibreCustomFields();
        private XElement _CustomFieldsMetaElement = null;

        #endregion Member Variables...

        #region Properties...

        #region RootElement
        public XElement RootElement { get; private set; }
        #endregion RootElement

        #region Version
        public string Version
        {
            get
            {
                return RootElement.Attribute("version").Value;
            }
        }
        #endregion Version

        #endregion Properties...

        #region Constructors...

        #region ContentFileCustomMetadataEditor
        public ContentFileCustomMetadataEditor(string xmlString)
        {
            RootElement = XElement.Parse(xmlString, LoadOptions.PreserveWhitespace | LoadOptions.SetBaseUri);
        }
        #endregion ContentFileCustomMetadataEditor

        #endregion Constructors...

        #region Methods...

        #region AddCustomFieldsMetadataElement
        private static XElement AddCustomFieldsMetadataElement(XElement metadataElement)
        {
            XElement userMetadataField = new XElement(_OpfNamespace + "meta", new XAttribute("property", "calibre:user_metadata"));
            metadataElement.Add(userMetadataField);
            return userMetadataField;
        }
        #endregion AddCustomFieldsMetadataElement

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
            var metadataElement = RootElement.Element(_OpfNamespace + "metadata");
            var elementsToRemove = new List<XElement>();
            foreach (var subElement in metadataElement.Elements())
            {
                var propertyAttribute = subElement.Attribute("property");
                var nameAttribute = subElement.Attribute("name");
                var contentAttribute = subElement.Attribute("content");

                if (propertyAttribute != null && propertyAttribute.Value == "calibre:user_metadata")
                {
                    _CustomFieldsMetaElement = subElement;
                    var customFields = JsonSerializer.Deserialize<CalibreCustomFields>(_CustomFieldsMetaElement.Value);
                    MergeIntoExistingCustomFields(customFields);
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

            if (_CustomFieldsMetaElement == null)
            {
                _CustomFieldsMetaElement = AddCustomFieldsMetadataElement(metadataElement);
            }
        }
        #endregion ParseForCustomFields

        #region RemoveCustomFieldValue
        public void RemoveCustomFieldValue(string propertyName, bool overrideExistingValue = false)
        {
            if (_CustomFields == null || _CustomFieldsMetaElement == null)
            {
                ParseForCustomFields();
            }

            if (_CustomFields.ContainsKey(propertyName))
            {
                _CustomFields.Remove(propertyName);
            }
            _CustomFieldsMetaElement.Value = _CustomFields.SerializeToJson();
        }
        #endregion RemoveCustomFieldValue

        #region SetCustomFieldValue
        public void SetCustomFieldValue(CalibreCustomFieldModel customFieldModel, bool overrideExistingValue = false)
        {
            if (_CustomFields == null || _CustomFieldsMetaElement == null)
            {
                ParseForCustomFields();
            }

            string key = customFieldModel.PropertyName;
            if (!_CustomFields.ContainsKey(key) || !string.IsNullOrEmpty(_CustomFields[key].Value?.ToString()) || overrideExistingValue)
            {
                _CustomFields[key] = customFieldModel;
            }
            _CustomFieldsMetaElement.Value = _CustomFields.SerializeToJson();
        }
        #endregion SetCustomFieldValue

        #endregion Methods...
    }
}
