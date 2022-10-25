using System.Windows;
using System.Windows.Controls;

namespace Willowcat.EbookDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for EpubTagItemsView.xaml
    /// </summary>
    public partial class EpubTagItemsView : UserControl
    {
        public static readonly DependencyProperty LabelTextProperty
            = DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(EpubTagItemsView));

        public static readonly DependencyProperty TagStyleProperty
            = DependencyProperty.Register(nameof(TagStyle), typeof(Style), typeof(EpubTagItemsView));

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        public Style TagStyle
        {
            get => (Style)GetValue(TagStyleProperty);
            set => SetValue(TagStyleProperty, value);
        }

        public EpubTagItemsView()
        {
            InitializeComponent();
        }
    }
}
