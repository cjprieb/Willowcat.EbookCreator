using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Willowcat.EbookDesktopUI.ApplicationResources.Converters
{
    public class NullToVisibilityConverter : IValueConverter
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
            return (value == null) ? Visibility.Collapsed : Visibility.Visible;
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
