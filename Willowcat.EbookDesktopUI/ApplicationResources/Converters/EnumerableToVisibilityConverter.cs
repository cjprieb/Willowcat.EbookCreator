using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Willowcat.EbookDesktopUI.ApplicationResources.Converters
{
    public class EnumerableToVisibilityConverter : IValueConverter
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = Visibility.Visible;
            if (value == null)
            {
                result = Visibility.Collapsed;
            }
            else if (value is IEnumerable list)
            {
                var enumerator = list.GetEnumerator();
                result = enumerator.MoveNext() ? Visibility.Visible : Visibility.Collapsed;
            }
            return result;
        }
        #endregion Convert

        #region ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion ConvertBack

        #endregion Methods...
    }
}
