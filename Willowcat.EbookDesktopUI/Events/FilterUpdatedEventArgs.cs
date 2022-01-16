using System;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.Events
{
    public class FilterUpdatedEventArgs : EventArgs
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #region FilterModel
        public FilterModel FilterModel { get; private set; }
        #endregion FilterModel

        #endregion Properties...

        #region Constructors...

        #region FilterUpdatedEventArgs
        public FilterUpdatedEventArgs(FilterModel filterModel)
        {
            FilterModel = filterModel;
        }
        #endregion FilterUpdatedEventArgs

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
