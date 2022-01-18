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
    }
}
