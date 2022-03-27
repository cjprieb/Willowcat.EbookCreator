using System;
using System.Collections.Generic;
using System.Linq;

namespace Willowcat.EbookCreator.Models
{
    public class BibliographyModel : IBibliographyModel
    {
        #region Member Variables...
        private List<string> _Creators = new List<string>();
        private string _CustomCreatorSort = null;
        #endregion Member Variables...

        #region Properties...
        public IEnumerable<string> Creators => _Creators;
        public string CreatorSort 
        {
            get => string.IsNullOrEmpty(_CustomCreatorSort) ? FormatCreatorListForSort(_Creators) : _CustomCreatorSort;
            set => _CustomCreatorSort = value;
        }
        public string Description { get; set; } = "";
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Language { get; set; } = "";
        public string OriginalLink { get; set; } = "";
        public string Proofreader { get; set; } = "";
        public string Publisher { get; set; } = "";
        public DateTime PublishedDate { get; set; }
        public List<string> Tags { get; private set; } = new List<string>();
        public string Title { get; set; } = "";
        public string Translator { get; set; } = "";
        public string Series { get; set; } = "";
        public int? SeriesIndex { get; set; }
        public CalibreCustomFields CustomFields { get; set; }

        #endregion Properties...

        public virtual void AddCustomField(string name, CalibreCustomFieldModel value)
        {
            if (CustomFields == null)
            {
                CustomFields = new CalibreCustomFields();
            }
            CustomFields[name] = value;
        }

        public static string FormatCreatorList(IEnumerable<string> creators)
        {
            return string.Join(" & ", creators);
        }

        public static string FormatCreatorListForSort(IEnumerable<string> creators)
        {
            return FormatCreatorList(creators);
        }

        public void SetCreators(string value)
        {
            _Creators = value.Split('&').ToList();
        }
    }
}
