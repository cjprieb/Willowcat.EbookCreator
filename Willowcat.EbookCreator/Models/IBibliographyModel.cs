using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookCreator.Models
{
    public interface IBibliographyModel
    {
        IEnumerable<string> Creators { get; }
        string CreatorSort { get; }
        string Description { get; }
        Guid Guid { get; }
        string Language { get; }
        string OriginalLink { get; }
        string Proofreader { get; }
        string Publisher { get; }
        DateTime PublishedDate { get; }
        List<string> Tags { get; }
        string Title { get; }
        string Translator { get; }
        string Series { get; }
        int? SeriesIndex { get; }
        CalibreCustomFields CustomFields { get; }
        void AddCustomField(string name, CalibreCustomFieldModel value);
    }
}
