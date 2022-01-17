using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Willowcat.EbookCreator.Models;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.Services
{
    public class EpubDisplayModelBuilder
    {
        #region Member Variables...
        /// <summary>
        /// 2019-05-15
        /// </summary>
        private static Regex _DatePattern = new Regex(@"(\d{4})-(\d{2})-(\d{2})");

        private BibliographyModel _Bibliography = null;
        private Dictionary<string, List<string>> _MetadataDictionaries = null;
        private string _Description = null;

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region Build
        public EpubDisplayModel Build()
        {
            EpubDisplayModel model = new EpubDisplayModel();

            if (_Bibliography != null) 
            {
                InitializeBibliography(model);
            }

            if (_MetadataDictionaries != null) 
            {
                InitializeTags(model);
                InitializeStatistics(model);
                InitializeSeries(model);
            }

            model.Description = _Description;

            return model;
        }
        #endregion Build

        #region InitializeBibliography
        private void InitializeBibliography(EpubDisplayModel model)
        {
            model.Author = _Bibliography.Creator;
            model.Title = _Bibliography.Title;
            if (!string.IsNullOrEmpty(_Bibliography.Series))
            {
                model.Series = new EpubSeriesModel[]
                {
                    new EpubSeriesModel()
                    {
                        Title = _Bibliography.Series,
                        Index = _Bibliography.SeriesIndex.Value
                    }
                };
            }
        }
        #endregion InitializeBibliography

        #region InitializeSeries
        private void InitializeSeries(EpubDisplayModel model)
        {
            //throw new NotImplementedException();
        }
        #endregion InitializeSeries

        #region InitializeStatistics
        private void InitializeStatistics(EpubDisplayModel model)
        {
            model.Statistics.DatePublished = ParseDate("Published") ?? DateTime.MinValue;
            model.Statistics.DateCompleted = ParseDate("Completed");
            model.Statistics.DateUpdated = ParseDate("Updated");
            if (!model.Statistics.DateUpdated.HasValue && model.Statistics.DateUpdated.HasValue)
            {
                model.Statistics.DateUpdated = model.Statistics.DateUpdated;
            }

            model.Statistics.Words = ParseInteger("Words") ?? 0;

            (int count, int? total) = ParseChapters("Chapters");
            model.Statistics.ChaptersReleased = count;
            model.Statistics.TotalChapters = total;
        }
        #endregion InitializeStatistics

        #region InitializeTags
        private void InitializeTags(EpubDisplayModel model)
        {
            model.Rating = ParseRating("Rating");
            model.WarningTags = GetTags("Archive Warning");
            model.FandomTags = GetTags("Fandom");
            model.RelationshipTags = GetTags("Relationship");
            model.CharacterTags = GetTags("Character");
            // TODO: move extra tags to "detail" section
            model.AdditionalTags = GetTags("AdditionalTags");
        }
        #endregion InitializeTags

        #region ParseChapters
        private (int count, int? total) ParseChapters(string value)
        {
            int count = 1;
            int? total = 1;
            Regex chapterPattern = new Regex(@"(\d+)/([\d\?]+)");
            Match match = chapterPattern.Match(value);
            if (match.Success)
            {
                string countString = match.Groups[1].Value;
                if (int.TryParse(countString, out int i1))
                {
                    count = i1;
                }

                string totalString = match.Groups[2].Value;
                if (int.TryParse(totalString, out int i2))
                {
                    total = i2;
                }
                else
                {
                    total = null;
                }
            }
            return (count, total);
        }
        #endregion ParseChapters

        #region ParseDate
        private DateTime? ParseDate(string dateKey)
        {
            DateTime? result = null;
            if (_MetadataDictionaries.ContainsKey(dateKey))
            {
                string dateText = _MetadataDictionaries[dateKey].FirstOrDefault();
                if (!string.IsNullOrEmpty(dateText))
                {
                    var match = _DatePattern.Match(dateText);
                    if (match.Success)
                    {
                        var year = int.Parse(match.Groups[1].Value);
                        var month = int.Parse(match.Groups[2].Value);
                        var day = int.Parse(match.Groups[3].Value);
                        result = new DateTime(year, month, day);
                    }
                }
            }
            return result;
        }
        #endregion ParseDate

        #region ParseInteger
        private int? ParseInteger(string key)
        {
            int? result = null;
            if (_MetadataDictionaries.ContainsKey(key))
            {
                string integerText = _MetadataDictionaries[key].FirstOrDefault();
                if (!string.IsNullOrEmpty(integerText) && int.TryParse(integerText, out int i))
                {
                    result = i;
                }
            }
            return result;
        }
        #endregion ParseInteger

        #region ParseRating
        private RatingType ParseRating(string key)
        {
            RatingType rating = RatingType.None;
            var lists = GetTags(key);
            if (lists != null && lists.Any())
            {
                var firstItem = lists.First();
                if (firstItem == "General Audiences")
                {
                    rating = RatingType.GeneralAudiences;
                }
                else if (firstItem.Contains("Teen"))
                {
                    rating = RatingType.Teen;
                }
                else if (firstItem.Contains("Mature"))
                {
                    rating = RatingType.Mature;
                }
            }
            return rating;
        }
        #endregion ParseRating

        #region ParseTags
        private IEnumerable<string> GetTags(string key)
        {
            if (!_MetadataDictionaries.ContainsKey(key))
            {
                return new string[] { };
            }

            return _MetadataDictionaries[key];
        }
        #endregion ParseTags

        #region SetBibliographyFields
        public EpubDisplayModelBuilder SetBibliography(BibliographyModel bibliography)
        {
            _Bibliography = bibliography;
            return this;
        }
        #endregion SetBibliographyFields

        #region SetDescription
        public void SetDescription(string description)
        {
            _Description = description;
        }
        #endregion SetDescription

        #region SetMetadata
        public EpubDisplayModelBuilder SetMetadata(Dictionary<string, List<string>> dictionaries)
        {
            _MetadataDictionaries = dictionaries;
            return this;
        }
        #endregion SetMetadata

        #endregion Methods...
    }
}
