﻿using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Willowcat.EbookDesktopUI.Common;
using Willowcat.EbookDesktopUI.ViewModels;

namespace Willowcat.EbookDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for MergeBooksView.xaml
    /// </summary>
    public partial class MergeBooksView : UserControl
    {
        public MergeBooksView()
        {
            InitializeComponent();
        }

        private void OpenDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null && DataContext is MergeBooksViewModel viewModel)
            {
                PathExtensions.ExploreToDirectory(viewModel.OutputDirectory);
            }
        }
    }
}
