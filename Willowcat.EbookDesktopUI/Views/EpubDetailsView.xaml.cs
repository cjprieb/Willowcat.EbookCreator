using System.Windows.Controls;
using System.Windows.Navigation;
using Willowcat.EbookDesktopUI.Common;
using Willowcat.EbookDesktopUI.ViewModels;

namespace Willowcat.EbookDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for EpubDetailsView.xaml
    /// </summary>
    public partial class EpubDetailsView : UserControl
    {
        public EpubDetailsView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            HyperlinkExtensions.Navigate(e.Uri);
        }

        private void OpenFilePathButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext == null) return;

            if (DataContext is EpubItemViewModel viewModel)
            {
                PathExtensions.ExploreToFile(viewModel.DisplayModel.LocalFilePath);
            }
        }
    }
}
