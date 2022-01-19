using System;
using System.Collections.Generic;
using System.Linq;
using Willowcat.EbookCreator.Engines;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookDesktopUI.Models
{
    public class EpubDisplayModel
    {
        public IEnumerable<string> AdditionalTags { get; set; } = new string[] { };
        public IEnumerable<string> OverflowTags { get; set; } = new string[] { };
        public IEnumerable<string> CharacterTags { get; set; } = new string[] { };
        public IEnumerable<string> FandomTags { get; set; } = new string[] { };
        public RatingType Rating { get; set; }
        public IEnumerable<string> RelationshipTags { get; set; } = new string[] { };
        public string Title { get; set; }
        public IEnumerable<EpubSeriesModel> Series { get; set; }
        public IEnumerable<string> SeriesDisplay => Series.Select(x => $"Part {x.Index} of {x.Title}");
        public string SeriesUrl { get; set; }
        public EpubStatisticsModel Statistics { get; set; } = new EpubStatisticsModel();
        public IEnumerable<string> WarningTags { get; set; } = new string[] { };
        public string WorkId { get; set; }
        public string WorkUrl { get; set; }
        public string LocalFilePath { get; set; }
        public string Description { get; set; }
        public string FirstChapterText { get; set; }
        public string Author { get; set; }
        public IEnumerable<ProcessTagType> ProcessTags { get; set; } = new ProcessTagType[] { };
        public IEnumerable<string> AllTags
        {
            get
            {
                IEnumerable<string> result = new string[] { };
                if (AdditionalTags != null)
                {
                    result = result.Union(AdditionalTags);
                }
                if (CharacterTags != null)
                {
                    result = result.Union(CharacterTags);
                }
                if (RelationshipTags != null)
                {
                    result = result.Union(RelationshipTags);
                }
                if (WarningTags != null)
                {
                    result = result.Union(WarningTags);
                }
                if (FandomTags != null)
                {
                    result = result.Union(FandomTags);
                }
                if (OverflowTags != null)
                {
                    result = result.Union(OverflowTags);
                }
                if (ProcessTags != null)
                {
                    result = result.Union(ProcessTags.Select(tag => tag.ToTagName()));
                }
                return result;
            }
        }
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
