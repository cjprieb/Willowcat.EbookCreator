using System;

namespace Willowcat.EbookDesktopUI.Events
{
    public class PageRequestedEventArgs : EventArgs
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #region ItemsToDisplay
        public int ItemsToDisplay { get; private set; }
        #endregion ItemsToDisplay

        #region ItemsToSkip
        public int ItemsToSkip { get; private set; }
        #endregion ItemsToSkip

        #endregion Properties...

        #region Constructors...

        #region PageRequestedEventArgs
        public PageRequestedEventArgs(int itemsToSkip, int itemsToDisplay)
        {
            ItemsToSkip = itemsToSkip;
            ItemsToDisplay = itemsToDisplay;
        }
        #endregion PageRequestedEventArgs

        #endregion Constructors...

        #region Methods...

        #endregion Methods...
    }
}
