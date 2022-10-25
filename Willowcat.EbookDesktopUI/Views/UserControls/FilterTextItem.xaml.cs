using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Willowcat.EbookDesktopUI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for FilterTextItem.xaml
    /// </summary>
    public partial class FilterTextItem : UserControl
    {
        public ICommand ClearCommand
        {
            get { return (ICommand)GetValue(ClearCommandProperty); }
            set { SetValue(ClearCommandProperty, value); }
        }

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(FilterTextItem), new PropertyMetadata(null));

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(FilterTextItem), new PropertyMetadata(null));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(FilterTextItem), new PropertyMetadata(null));



        public FilterTextItem()
        {
            InitializeComponent();
        }
    }
}
