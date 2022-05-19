using System;
using System.Diagnostics;

namespace Willowcat.EbookDesktopUI.Common
{
    // TODO: move to Willowcat.Common.UI - HyperlinkExtensions
    public class HyperlinkExtensions
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region Navigate
        public static void Navigate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = value,
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(startInfo);
            }
        }
        #endregion Navigate

        #region Navigate
        public static void Navigate(Uri uri)
        {
            Navigate(uri?.ToString());
        }
        #endregion Navigate

        #endregion Methods...
    }
}
