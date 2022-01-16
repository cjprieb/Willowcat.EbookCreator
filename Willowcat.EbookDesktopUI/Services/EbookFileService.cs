using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        #endregion Methods...
    }
}
