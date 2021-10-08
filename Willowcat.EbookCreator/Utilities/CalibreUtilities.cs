using System;

namespace Willowcat.EbookCreator.Utilities
{
    public class CalibreUtilities
    {
        #region Member Variables...

        #endregion Member Variables...

        #region Properties...

        #endregion Properties...

        #region Constructors...

        #endregion Constructors...

        #region Methods...

        #region GenerateCustomTimeReadMetadata
        public static string GenerateCustomTimeReadMetadata(TimeSpan timeToRead)
        {
            string value = timeToRead.ToString();
            return "{&quot;column&quot;: &quot;value&quot;, &quot;datatype&quot;: &quot;text&quot;, &quot;rec_index&quot;: 27, &quot;search_terms&quot;: [&quot;#readtime&quot;], &quot;is_multiple&quot;: null, &quot;link_column&quot;: &quot;value&quot;, &quot;label&quot;: &quot;readtime&quot;, &quot;is_category&quot;: true, &quot;#extra#&quot;: null, &quot;kind&quot;: &quot;field&quot;, &quot;is_custom&quot;: true, &quot;name&quot;: &quot;Length&quot;, &quot;is_csp&quot;: false, &quot;colnum&quot;: 8, &quot;display&quot;: {&quot;description&quot;: &quot;Length of time to read book&quot;, &quot;use_decorations&quot;: 0}, &quot;is_editable&quot;: true, &quot;is_multiple2&quot;: {}, &quot;#value#&quot;: &quot;" +
                value +
                "&quot;, &quot;category_sort&quot;: &quot;value&quot;, &quot;table&quot;: &quot;custom_column_8&quot;}";
        }
        #endregion GenerateCustomTimeReadMetadata

        #endregion Methods...
    }
}
