using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Willowcat.EbookDesktopUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = $"Unexpected error occurred.{Environment.NewLine}{e.Exception.Message}{Environment.NewLine}{Environment.NewLine}{e.Exception.StackTrace}";
            MessageBox.Show(message, "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
