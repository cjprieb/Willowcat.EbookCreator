using System.Diagnostics;
using System.IO;
using System.Windows;
using Willowcat.EbookDesktopUI.Models;
using Willowcat.EbookDesktopUI.ViewModels;

namespace Willowcat.EbookDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for CreateSeriesWindow.xaml
    /// </summary>
    public partial class CreateSeriesWindow : Window
    {
        public CreateSeriesWindow(SettingsModel settings)
        {
            InitializeComponent();
            DataContext = new MergeBooksViewModel(settings);
        }

        private void OpenDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MergeBooksViewModel viewModel)
            {
                string OutputDirectory = viewModel.OutputDirectory;
                if (!string.IsNullOrEmpty(OutputDirectory) && Directory.Exists(OutputDirectory))
                {
                    Process.Start("explorer.exe", OutputDirectory);
                }
            }
        }
    }
}
