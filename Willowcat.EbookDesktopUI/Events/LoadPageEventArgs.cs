using System;

namespace Willowcat.EbookDesktopUI.Events
{
    public class LoadPageEventArgs : EventArgs
    {
        public int Page { get; private set; }
        public LoadPageEventArgs(int page)
        {
            Page = page;
        }
    }
}
