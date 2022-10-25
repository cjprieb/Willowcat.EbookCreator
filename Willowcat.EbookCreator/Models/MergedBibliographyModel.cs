using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookCreator.Models
{
    public class MergedBibliographyModel : IBibliographyModel
    {
        #region Member Variables...
        private IBibliographyModel _OriginalBibliography = null;

        //private Dictionary<string, List<string>> _TagsForDescription = new Dictionary<string, List<string>>();
        protected List<string> _Creators = new List<string>();
        protected List<string> _TagsForMetadata = new List<string>();
        protected List<string> _TitlesForDescription = new List<string>();

        private string _NewTitle = null;
        private string _NewSeries = null;
        private int? _NewSeriesIndex = null;

        #endregion Member Variables...

        #region Properties...

        public IEnumerable<string> Creators => _Creators;
        public string CreatorSort => BibliographyModel.FormatCreatorListForSort(_Creators);
        public string Description => BuildDescription();
        public Guid Guid => _OriginalBibliography.Guid;
        public string Language => _OriginalBibliography.Language;
        public string OriginalLink => _OriginalBibliography.OriginalLink;
        public string Proofreader => _OriginalBibliography.Proofreader;
        public string Publisher => _OriginalBibliography.Publisher;
        public DateTime PublishedDate => _OriginalBibliography.PublishedDate;
        public List<string> Tags => GetTags();
        public string Title
        {
            get => !string.IsNullOrEmpty(_NewTitle) ? _NewTitle : _OriginalBibliography.Title;
            set => _NewTitle = value;
        }
        public string Translator => _OriginalBibliography.Translator;
        public string Series
        {
            get =>  !string.IsNullOrEmpty(_NewSeries) ? _NewSeries : _OriginalBibliography.Series;
            set =>  _NewSeries = value;
        }
        public int? SeriesIndex
        {
            get => _NewSeriesIndex.HasValue ? _NewSeriesIndex : _OriginalBibliography.SeriesIndex;
            set => _NewSeriesIndex = value;
        }
        public CalibreCustomFields CustomFields => _OriginalBibliography.CustomFields;

        #endregion Properties...

        #region Constructors...

        #region MergedBibliographyModel
        public MergedBibliographyModel(IBibliographyModel originalBibliography)
        {
            _OriginalBibliography = originalBibliography;
            _TitlesForDescription.Add(_OriginalBibliography.Title);
            MergeCreators(originalBibliography);
            MergeTags(originalBibliography);
        }
        #endregion MergedBibliographyModel

        #endregion Constructors...

        #region Methods...

        public virtual void AddCustomField(string name, CalibreCustomFieldModel value)
            => _OriginalBibliography.AddCustomField(name, value);

        protected virtual string BuildDescription()
        {
            StringBuilder descriptionBuilder = new StringBuilder();
            descriptionBuilder.Append(_OriginalBibliography.Description);
            descriptionBuilder.AppendLine("<p><strong>Also Includes:</strong></p><ol>");
            foreach (var title in _TitlesForDescription)
            {
                descriptionBuilder.AppendLine($"<li>{title}</li>");
            }
            descriptionBuilder.AppendLine("</ol>");

            return descriptionBuilder.ToString();
        }

        protected virtual List<string> GetTags() => _TagsForMetadata;

        public virtual void MergeBibliography(IBibliographyModel bibliography)
        {
            _TitlesForDescription.Add(bibliography.Title);
            MergeCreators(bibliography);
            MergeTags(bibliography);
        }

        protected virtual void MergeCreators(IBibliographyModel bibliography)
        {
            foreach (var creator in bibliography.Creators)
            {
                if (!_Creators.Contains(creator))
                {
                    _Creators.Add(creator);
                }
            }
        }

        protected virtual void MergeTags(IBibliographyModel bibliography)
        {
            foreach (var tag in bibliography.Tags)
            {
                if (!_TagsForMetadata.Contains(tag))
                {
                    _TagsForMetadata.Add(tag);
                }
            }
        }

        #endregion Methods...
    }
}
