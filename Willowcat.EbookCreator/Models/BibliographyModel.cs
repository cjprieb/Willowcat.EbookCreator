using System;
using System.Collections.Generic;

namespace Willowcat.EbookCreator.Models
{
    public class BibliographyModel
    {
        public string Creator { get; set; } = "";

        public string CreatorSort { get; set; } = "";

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
    }
}
