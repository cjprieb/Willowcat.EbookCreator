﻿using System;
using System.Collections.Generic;
using System.Linq;
using Willowcat.EbookCreator.Models;

namespace Willowcat.EbookDesktopUI.Models
{
    public class EpubDisplayModel
    {
        public IEnumerable<string> AdditionalTags { get; set; } = new string[] { };
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
        public string Author { get; set; }


        public void InitializeFrom(BibliographyModel bibliography)
        {
            Author = bibliography.Creator;
            Title = bibliography.Title;
            if (!string.IsNullOrEmpty(bibliography.Series))
            {
                Series = new EpubSeriesModel[]
                {
                    new EpubSeriesModel()
                    {
                        Title = bibliography.Series,
                        Index = bibliography.SeriesIndex.Value
                    }
                };
            }
            AdditionalTags = bibliography.Tags;
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
