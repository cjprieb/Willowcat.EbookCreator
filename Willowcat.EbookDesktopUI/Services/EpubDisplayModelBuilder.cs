using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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

        private IBibliographyModel _Bibliography = null;
        private Dictionary<string, List<string>> _MetadataDictionaries = null;
        private string _Description = null;
        private string _FirstChapterText = null;
        private string _LocalFilePath = null;
        private string _WorkUrl = null;

        #endregion Member Variables...

        #region Properties...

        public int MaxAdditionalTags { get; set; } = 8;
        public int MaxLengthOfAdditionalTag { get; set; } = 15;

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
            model.FirstChapterText = _FirstChapterText;
            model.LocalFilePath = _LocalFilePath;
            model.WorkUrl = _WorkUrl;
            model.WorkId = ParseWorkId(_WorkUrl);

            return model;
        }
        #endregion Build

        #region ExtractProcessTags
        private List<ProcessTagType> ExtractProcessTags()
        {
            List<ProcessTagType> processTags = new List<ProcessTagType>();
            foreach (var subject in _Bibliography.Tags)
            {
                if (subject.StartsWith("process"))
                {
                    ProcessTagType? processTagType = ParseAsProcessTag(subject);
                    if (processTagType.HasValue && processTagType.Value != ProcessTagType.None && processTagType.Value != ProcessTagType.All)
                    {
                        processTags.Add(processTagType.Value);
                    }
                }
            }

            return processTags;
        }
        #endregion ExtractProcessTags

        #region InitializeBibliography
        private void InitializeBibliography(EpubDisplayModel model)
        {
            model.Author = BibliographyModel.FormatCreatorList(_Bibliography.Creators);
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

            model.ProcessTags = ExtractProcessTags();
        }
        #endregion InitializeBibliography

        #region InitializeSeries
        private void InitializeSeries(EpubDisplayModel model)
        {
            model.Series = ParseSeries("Series");
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

            if (!string.IsNullOrEmpty(_LocalFilePath) && File.Exists(_LocalFilePath))
            {
                model.Statistics.DateFileCreated = File.GetCreationTime(_LocalFilePath);
            }
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
             
            var parsedTags = GetTags("Additional Tags");
            var additionalTags = new List<string>();
            var overflowTags = new List<string>();
            foreach (var tag in parsedTags)
            {
                if (additionalTags.Count < MaxAdditionalTags && tag.Length < MaxLengthOfAdditionalTag)
                {
                    additionalTags.Add(tag);
                }
                else
                {
                    overflowTags.Add(tag);
                }
            }
            model.AdditionalTags = additionalTags;
            model.OverflowTags = overflowTags;
        }
        #endregion InitializeTags

        #region ParseAsProcessTag
        private ProcessTagType? ParseAsProcessTag(string subject)
        {
            ProcessTagType? result = null;
            if (subject.StartsWith(ProcessTagTypeExtensions.ProcessTagPrefix))
            {
                result = ProcessTagTypeExtensions.Parse(subject.Substring(ProcessTagTypeExtensions.ProcessTagPrefix.Length));
            }
            return result;
        }
        #endregion ParseAsProcessTag

        #region ParseChapters
        private (int count, int? total) ParseChapters(string key)
        {
            var values = GetTags(key);
            var value = values.Any() ? values.First() : string.Empty;
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

        #region ParseSeries
        private IEnumerable<EpubSeriesModel> ParseSeries(string key)
        {
            var seriesTextList = GetTags(key);
            if (seriesTextList != null && seriesTextList.Any())
            {
                Regex seriesPattern = new Regex("Part (\\d+) of <a href=\"([^\"]+)\">([^<]+)</a>");
                foreach (Match match in seriesPattern.Matches(seriesTextList.First()))
                {
                    string indexString = match.Groups[1].Value;
                    string url = match.Groups[2].Value;
                    string title = match.Groups[3].Value
                            .Replace("&amp;", "&")
                            .Replace("&quot;", "\"");
                    yield return new EpubSeriesModel()
                    {
                        Index = int.Parse(indexString),
                        Title = title,
                        Url = url
                    };
                }
            }
        }
        #endregion ParseSeries

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

        #region ParseWorkId
        private string ParseWorkId(string workUrl)
        {
            //http://archiveofourown.org/works/24359968
            string id = null;
            if (workUrl != null)
            {
                Match match = Regex.Match(workUrl, @"http://archiveofourown.org/works/(\d+)");
                if (match.Success)
                {
                    id = match.Groups[1].Value;
                }
            }
            return id;
        }
        #endregion ParseWorkId

        #region SetBibliographyFields
        public EpubDisplayModelBuilder SetBibliography(BibliographyModel bibliography)
        {
            _Bibliography = bibliography;
            return this;
        }
        #endregion SetBibliographyFields

        #region SetDescription
        public void SetDescription(string description, string firstChapterText)
        {
            _Description = description;
            _FirstChapterText = !string.IsNullOrEmpty(firstChapterText) ? firstChapterText + "<p>...</p>" : string.Empty;
        }
        #endregion SetDescription

        #region SetLocalFilePath
        public void SetLocalFilePath(string filePath)
        {
            _LocalFilePath = filePath;
        }
        #endregion SetLocalFilePath

        #region SetMetadata
        public EpubDisplayModelBuilder SetMetadata(Dictionary<string, List<string>> dictionaries)
        {
            _MetadataDictionaries = dictionaries;
            return this;
        }
        #endregion SetMetadata

        #region SetWorkUrl
        public void SetWorkUrl(string url)
        {
            _WorkUrl = url;
        }
        #endregion WorkUrl

        #endregion Methods...
    }
}
 