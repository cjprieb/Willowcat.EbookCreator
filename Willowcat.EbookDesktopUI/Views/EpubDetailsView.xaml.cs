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

        public void ResetVisibleControls()
        {
            BookDetailsTabControl.SelectedItem = SummaryTabItem;
            SummaryTextControl.ScrollToTop();
            FirstChapterTextControl.ScrollToTop();
        }
    }
}
