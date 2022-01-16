using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Willowcat.Common.Utilities;

namespace Willowcat.EbookCreator.Models
{
    public class SeriesModel
    {
        #region Properties...

        #region FromIndex
        public int? FromIndex { get; set; }
        #endregion FromIndex

        #region WorkIndexes
        public IEnumerable<int> WorkIndexes { get; set; }
        #endregion WorkIndexes

        #region ToIndex
        public int? ToIndex { get; set; }
        #endregion ToIndex

        #region OverrideBookTitle
        public string OverrideBookTitle { get; set; }
        #endregion OverrideBookTitle

        #region SeriesIndex
        public int? SeriesIndex { get; set; }
        #endregion SeriesIndex

        #region SeriesName
        public string SeriesName { get; set; }
        #endregion SeriesName

        #region SeriesUrl
        public string SeriesUrl { get; set; }
        #endregion SeriesUrl

        #region WorkUrls
        public string[] WorkUrls { get; set; }
        #endregion WorkUrls

        #endregion Properties...

        #region Constructors...

        #region SeriesModel
        public SeriesModel()
        {
        }
        #endregion SeriesModel

        #endregion Constructors...

        #region Methods...

        #region GetWorksToInclude
        public IEnumerable<WorkModel> GetWorksToInclude()
        {
            int chapterIndex = 1;
            Regex workUrlPattern = new Regex(@"/works/(\d+)");
            foreach (var url in WorkUrls)
            {
                var match = workUrlPattern.Match(url);
                if (match.Success)
                {
                    var workId = match.Groups[1].Value;
                    //var encodedWorkTitle = Uri.EscapeDataString(workTitle.Replace("?", "").Replace(":", "").Trim());
                    //https://archiveofourown.org/downloads/21982333/Anchor%20Point.epub
                    yield return new WorkModel()
                    {
                        Index = chapterIndex,
                        WorkUrl = url,
                        Title = workId,
                        EpubUrl = $"https://www.archiveofourown.org/downloads/{workId}/{workId}.epub"
                    };
                    chapterIndex++;
                }
            }
        }
        #endregion

        #region FilterWorksToInclude
        public IEnumerable<WorkModel> FilterWorksToInclude(List<WorkModel> works)
        {
            IEnumerable<WorkModel> result = works;
            if (WorkIndexes != null && WorkIndexes.Count() > 0)
            {
                result = WorkIndexes.Select(i => works[i - 1]);
            }
            else
            {
                if (FromIndex.HasValue)
                {
                    result = result.Skip(FromIndex.Value - 1); // FromIndex starts at 1
                }

                if (ToIndex.HasValue)
                {
                    int startIndex = FromIndex ?? 1;
                    int takeCount = ToIndex.Value - startIndex + 1;
                    result = result.Take(takeCount);
                }
            }

            return result;
        }
        #endregion FilterWorksToInclude

        #region SetWorkIndexes
        public void SetWorkIndexes(int? fromWorkIndex, int? toWorkIndex)
        {
            FromIndex = fromWorkIndex;
            ToIndex = toWorkIndex;
        }
        #endregion SetWorkIndexes

        #region SetWorkIndexesFromString
        public void SetWorkIndexesFromString(string indexesAsString)
        {
            List<int> indexes = new List<int>();
            if (!string.IsNullOrEmpty(indexesAsString))
            {
                string[] rangeSections = indexesAsString.Split(',');
                foreach (var section in rangeSections)
                {
                    string fromIndexString = section.Trim();
                    string toIndexString = null;

                    int dashPosition = section.IndexOf('-');
                    if (dashPosition > 0)
                    {
                        fromIndexString = section.Substring(0, dashPosition);
                        toIndexString = section.Substring(dashPosition+1, section.Length - dashPosition - 1);
                    }

                    int? fromIndex = IntegerHelper.ParseOrNull(fromIndexString);
                    int? toIndex = IntegerHelper.ParseOrNull(toIndexString);
                    if (fromIndex.HasValue)
                    {
                        if (toIndex.HasValue)
                        {
                            for (int i = fromIndex.Value; i <= toIndex.Value; i++)
                            {
                                indexes.Add(i);
                            }
                        }
                        else
                        {
                            indexes.Add(fromIndex.Value);
                        }
                    }
                }
            }

            if (indexes.Any())
            {
                WorkIndexes = indexes;
            }
        }
        #endregion SetWorkIndexesFromString

        #endregion Methods...
    }
}
