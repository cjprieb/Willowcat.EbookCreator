using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.Services
{
    public class EbookFileService
    {
        #region Member Variables...
        private IEnumerable<string> _CachedFandomList = null;
        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #region EbookFileService
        public EbookFileService()
        {
        }
        #endregion EbookFileService

        #endregion Constructors...

        #region Methods...

        #region LoadFandomsAsync
        public Task<IEnumerable<string>> LoadFandomsAsync()
        {
            if (_CachedFandomList == null)
            {
                _CachedFandomList = new List<string>()
                {
                    "One",
                    "Two",
                    "Three"
                };
            }
            return Task.FromResult(_CachedFandomList);
        }
        #endregion LoadFandomsAsync

        #region GetFilteredResultsAsync
        public Task<IEnumerable<EpubDisplayModel>> GetFilteredResultsAsync(FilterModel model)
        {
            IEnumerable<EpubDisplayModel> result = new List<EpubDisplayModel>()
            {
                new EpubDisplayModel()
                {
                    Title = "this is the title",
                    WorkId = "123456",
                    WorkUrl = "https://www.duckduckgo.com",
                    SeriesUrl = "https://www.duckduckgo.com",
                    Rating = RatingType.GeneralAudiences,
                    FandomTags = new string[] { "one", "two" },
                    RelationshipTags = new string[] { "one/two", "three&four" },
                    CharacterTags = new string[] { "one", "two", "three", "four" },
                    AdditionalTags = new string[] { "additional" },
                    Series = new EpubSeriesModel[] { new EpubSeriesModel() {
                        Title = "best works",
                        Index = 1,
                        Url = "htp"
                    } },
                    Statistics = new EpubStatisticsModel()
                    {
                        DatePublished = new DateTime(2021, 4, 13),
                        DateCompleted = new DateTime(2021, 6, 8),
                        DateUpdated = new DateTime(2021, 6, 8),
                        ChaptersReleased = 4,
                        TotalChapters = 4,
                        Words = 10688
                    }
                }
            };

            return Task.FromResult(result);
        }
        #endregion GetFilteredResultsAsync

        #endregion Methods...
    }
}
