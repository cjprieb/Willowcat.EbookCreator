using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using Willowcat.EbookDesktopUI.Common;

namespace Willowcat.EbookDesktopUI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for OpenInBrowserHyperlink.xaml
    /// </summary>
    [ContentProperty(nameof(AdditionalContent))]
    public partial class OpenInBrowserHyperlink : UserControl
    {
        public object AdditionalContent
        {
            get { return (object)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }
        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(OpenInBrowserHyperlink), new PropertyMetadata(null));


        public Style HyperlinkStyle
        {
            get { return (Style)GetValue(HyperlinkStyleProperty); }
            set { SetValue(HyperlinkStyleProperty, value); }
        }
        public static readonly DependencyProperty HyperlinkStyleProperty =
            DependencyProperty.Register("HyperlinkStyle", typeof(Style), typeof(OpenInBrowserHyperlink), new PropertyMetadata(null));


        public Uri NavigateUri
        {
            get { return (Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }
        public static readonly DependencyProperty NavigateUriProperty =
        DependencyProperty.Register("NavigateUri", typeof(Uri), typeof(OpenInBrowserHyperlink), new PropertyMetadata(null));

        public OpenInBrowserHyperlink()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            HyperlinkExtensions.Navigate(e.Uri);
        }

        private void CopyLinkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string urlValue = NavigateUri?.ToString();
            if (!string.IsNullOrEmpty(urlValue))
            {
                Clipboard.SetText(urlValue);
            }
        }
    }
}
