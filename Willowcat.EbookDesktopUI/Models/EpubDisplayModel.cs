using System;
using System.Collections.Generic;
using System.Linq;

namespace Willowcat.EbookDesktopUI.Models
{
    public class EpubDisplayModel
    {
        public IEnumerable<string> AdditionalTags { get; set; }
        public IEnumerable<string> CharacterTags { get; set; }
        public IEnumerable<string> FandomTags { get; set; }
        public RatingType Rating { get; set; }
        public IEnumerable<string> RelationshipTags { get; set; }
        public string Title { get; set; }
        public IEnumerable<EpubSeriesModel> Series { get; set; }
        public IEnumerable<string> SeriesDisplay => Series.Select(x => $"Part {x.Index} of {x.Title}");
        public string SeriesUrl { get; set; }
        public EpubStatisticsModel Statistics { get; set; } = new EpubStatisticsModel();
        public IEnumerable<string> WarningTags { get; set; }
        public string WorkId { get; set; }
        public string WorkUrl { get; set; }
        public string LocalFilePath { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
    }

    public class EpubSeriesModel
    {
        public string Title { get; set; }
        public int Index { get; set; }
        public string Url { get; set; }
    }

    public class EpubStatisticsModel
    {
        public DateTime? DateCompleted { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int ChaptersReleased { get; set; }
        public int? TotalChapters { get; set; }
        public int Words { get; set; }
        public string ChapterDisplay => $"{ChaptersReleased}/{(TotalChapters.HasValue ? TotalChapters.ToString() : "?")}";
    }

    public enum RatingType
    {
        None,
        GeneralAudiences,
        Teen,
        Mature,
    }
}
