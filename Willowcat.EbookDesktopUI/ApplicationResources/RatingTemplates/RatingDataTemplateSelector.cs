using System;
using System.Windows;
using System.Windows.Controls;
using Willowcat.EbookDesktopUI.Models;

namespace Willowcat.EbookDesktopUI.ApplicationResources.RatingTemplates
{
    public class RatingDataTemplateSelector : DataTemplateSelector
    {
        #region Methods...

        #region GetResourceNameForTaskStatus
        private static string GetResourceNameForTaskStatus(RatingType rating)
        {
            string resourceName = null;
            switch (rating)
            {
                case RatingType.None:
                    resourceName = "NoRatingTemplate";
                    break;

                case RatingType.GeneralAudiences:
                    resourceName = "GeneralRatingTemplate";
                    break;

                case RatingType.Teen:
                    resourceName = "TeenRatingTemplate";
                    break;

                case RatingType.Mature:
                    resourceName = "MatureRatingTemplate";
                    break;
            }

            return resourceName;
        }
        #endregion GetResourceNameForTaskStatus

        #region SelectTemplate
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element == null) return null;

            if (item == null) return null;

            RatingType? rating = item != null ? item as RatingType? : null;
            if (!rating.HasValue) return null;

            string resourceName = GetResourceNameForTaskStatus(rating.Value);
            if (string.IsNullOrEmpty(resourceName)) return null;

            try
            {
                return element.FindResource(resourceName) as DataTemplate;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion SelectTemplate

        #endregion Methods...
    }
}
