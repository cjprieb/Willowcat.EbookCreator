using System.Windows;
using Willowcat.EbookDesktopUI.ViewModels;

namespace Willowcat.EbookDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel _ViewModel = null;

        public SettingsWindow(SettingsViewModel viewModel)
        {
            _ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _ViewModel.OnSaveExcecute();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //_ViewModel.Load();
        }
    }
}
