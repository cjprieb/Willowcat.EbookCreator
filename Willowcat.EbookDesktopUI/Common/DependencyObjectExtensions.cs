using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Willowcat.EbookDesktopUI.Common
{
    // TODO: move to Willowcat.Common.UI - DependencyObjectExtensions
    public static class DependencyObjectExtensions
    {
        #region Methods...

        #region GetScrollViewer
        public static DependencyObject GetScrollViewer(this DependencyObject o)
        {
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }
        #endregion GetScrollViewer

        #region ScrollToTop
        public static void ScrollToTop(this DependencyObject o)
        {
            if (o.GetScrollViewer() is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(0);
            }
        }
        #endregion ScrollToTop

        #endregion Methods...
    }
}
