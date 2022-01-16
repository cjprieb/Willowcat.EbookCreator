using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Willowcat.EbookDesktopUI.Common;

namespace Willowcat.EbookDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for EpubListItemView.xaml
    /// </summary>
    public partial class EpubListItemView : UserControl
    {
        public EpubListItemView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            HyperlinkExtensions.Navigate(e.Uri);
        }

        private void MergeSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is null) return;

            if (DataContext is EpubListItemView listItem)
            {
                // TODO: finish merge request action
            }
        }
    }
}
