using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.Services;
using Willowcat.EbookDesktopUI.ViewModels;
using Willowcat.EbookDesktopUI.Views;

namespace Willowcat.EbookDesktopUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _ViewModel = null;

        public MainWindow()
        {
            InitializeComponent();

            var settings = new SettingsModel();
            _ViewModel = new MainViewModel(new EbookFileService(settings), settings);

            _ViewModel.EpubSearchViewModel.SeriesMergeRequested += EpubSearchViewModel_SeriesMergeRequested;

            DataContext = _ViewModel;
        }

        private void CreateSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateSeriesWindow(_ViewModel.Settings);
            dialog.Owner = this;
            dialog.Show();
        }
        private void EpubSearchViewModel_SeriesMergeRequested(object sender, Events.SeriesMergeEventArgs e)
        {
            _ViewModel.MergeBooksViewModel = new MergeBooksViewModel(_ViewModel.Settings, e.DisplayModel, e.SeriesModel);
            MainTabControl.SelectedItem = MergeTabItem;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsWindow(new SettingsViewModel(_ViewModel.Settings));
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _ViewModel.LoadAsync();
        }
    }
}
