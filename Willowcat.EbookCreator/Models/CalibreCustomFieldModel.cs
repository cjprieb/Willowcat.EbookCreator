using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Willowcat.EbookCreator.Models
{
    public class CalibreCustomFieldModel
    {
        [JsonPropertyName("#extra#")]
        public string ExtraValue { get; set; }

        [JsonPropertyName("#value#")]
        public object Value { get; set; }

		[JsonPropertyName("category_sort")]
		public string CategorySort { get; set; } // "value", 

		[JsonPropertyName("colnum")]
		public int ColumnNumber { get; set; } // 3, 

		[JsonPropertyName("column")]
		public string Column { get; set; } // "value", 

		[JsonPropertyName("datatype")]
		public string Datatype { get; set; } // "datetime", 

		[JsonPropertyName("display")]
		public Dictionary<string, object> Display { get; set; } // {}, 

		[JsonPropertyName("is_category")]
		public bool IsCategory { get; set; } // false, 

		[JsonPropertyName("is_csp")]
		public bool IsCsp { get; set; } // false, 

		[JsonPropertyName("is_custom")]
		public bool IsCustom { get; set; } // true, 

		[JsonPropertyName("is_editable")]
		public bool IsEditable { get; set; } // true, 

		[JsonPropertyName("is_multiple")]
		public string IsMultiple { get; set; } // null, 

		[JsonPropertyName("is_multiple2")]
		public Dictionary<string, string> IsMultiple2 { get; set; } // {}, 

		[JsonPropertyName("kind")]
		public string Kind { get; set; } // "field", 

		[JsonPropertyName("label")]
		public string Label { get; set; } // "date_read", 

		[JsonPropertyName("link_column")]
		public string LinkColumn { get; set; } // "value", 

		[JsonPropertyName("name")]
		public string Name { get; set; } // "Date Read", 

		[JsonPropertyName("rec_index")]
		public int RecIndex { get; set; } // 22, 

		[JsonPropertyName("search_terms")]
		public string[] SearchTerms { get; set; } // [],

		[JsonPropertyName("table")]
		public string Table { get; set; } // "custom_column_3"

		[JsonIgnore]
		public string PropertyName { get; set; }
	}
}
