using System;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.EbookDesktopUI.Models
{
    public class LoadProgressModel
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #region IncrementCount
        public bool? IncrementCount { get; set; }
        #endregion IncrementCount

        #region CurrentCount
        public int? CurrentCount { get; set; }
        #endregion CurrentCount

        #region TotalCount
        public int? TotalCount { get; set; }
        #endregion TotalCount

        #endregion Properties...

        #region Constructors...
        public LoadProgressModel()
        {

        }
        public LoadProgressModel(int curr, int total)
        {
            CurrentCount = curr;
            TotalCount = total;
        }
        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
