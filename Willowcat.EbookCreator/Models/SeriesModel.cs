using System.Collections.Generic;
using System.Linq;

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

        #region SeriesIndex
        public int? SeriesIndex { get; set; }
        #endregion SeriesIndex

        #region SeriesName
        public string SeriesName { get; set; }
        #endregion SeriesName

        #region SeriesUrl
        public string SeriesUrl { get; set; }
        #endregion SeriesUrl

        #endregion Properties...

        #region Constructors...

        #region SeriesModel
        public SeriesModel()
        {
        }
        #endregion SeriesModel

        #endregion Constructors...

        #region Methods...

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

        #endregion Methods...
    }
}
