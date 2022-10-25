using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Willowcat.EbookDesktopUI.Common;
using Willowcat.EbookDesktopUI.ViewModels;

namespace Willowcat.EbookDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for EpubListView.xaml
    /// </summary>
    public partial class EpubListView : UserControl
    {
        public event EventHandler<SelectionChangedEventArgs> OnSelectionChanged;

        public EpubListView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EpubListViewModel viewModel)
            {
                viewModel.OnPageRequested += ViewModel_OnPageRequested;
            }
        }

        private void ViewModel_OnPageRequested(object sender, Events.PageRequestedEventArgs e)
        {
            BookListViewControl.ScrollToTop();
        }

        private void BookListViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged?.Invoke(this, e);
        }
    }
}
