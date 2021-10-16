using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Willowcat.EbookCreator.Models
{
    public class CalibreCustomFields : Dictionary<string, CalibreCustomFieldModel>
    {
		public static CalibreCustomFieldModel CreateDateReadField()
        {
			return new CalibreCustomFieldModel()
			{
				//ExtraValue = null,
				//Value = null,
				CategorySort = "value",
				ColumnNumber = 3,
				Column = "value",
				Datatype = "datetime",
				Display = new Dictionary<string, object>
				{
					{ "date_format", null },
					{ "description", "" }
				},
				//IsCategory = false,
				//IsCsp = false,
				IsCustom = true,
				IsEditable = true,
				//IsMultiple = null,
				IsMultiple2 = new Dictionary<string, string>(),
				Kind = "field",
				Label = "date_read",
				LinkColumn = "value",
				Name = "Date Read",
				RecIndex = 22,
				SearchTerms = new string[] {"#date_read"},
				Table = "custom_column_3",
				PropertyName = "#date_read"
			};
		}

		public static CalibreCustomFieldModel CreateIsReadField(bool? value = null)
		{
			return new CalibreCustomFieldModel()
			{
                //ExtraValue = null,
                Value = value,
                CategorySort = "value",
				ColumnNumber = 2,
				Column = "value",
				Datatype = "bool",
				Display = new Dictionary<string, object>
				{
					{ "description", "" }
				},
				//IsCategory = false,
				//IsCsp = false,
				IsCustom = true,
				IsEditable = true,
				//IsMultiple = null,
				IsMultiple2 = new Dictionary<string, string>(),
				Kind = "field",
				Label = "is_read",
				LinkColumn = "value",
				Name = "Read",
				RecIndex = 25,
				SearchTerms = new string[] { "#is_read" },
				Table = "custom_column_2",
				PropertyName = "#is_read"
			};
		}

        public static CalibreCustomFieldModel CreateTimeToReadField(TimeSpan? timeSpan = null)
		{
			return new CalibreCustomFieldModel()
			{
                //ExtraValue = null,
                Value = timeSpan?.ToString(),
                CategorySort = "value",
				ColumnNumber = 9,
				Column = "value",
				Datatype = "comments",
				Display = new Dictionary<string, object>
				{
					{ "description", "Length of time to read book" },
					{ "heading_position", "side" },
					{ "interpret_as", "short-text" }
				},
                IsCategory = false,
                //IsCsp = false,
                IsCustom = true,
				IsEditable = true,
				//IsMultiple = null,
				IsMultiple2 = new Dictionary<string, string>(),
				Kind = "field",
				Label = "read_time",
				LinkColumn = "value",
				Name = "Time To Read",
				RecIndex = 27,
				SearchTerms = new string[] { "#read_time" },
				Table = "custom_column_9",
				PropertyName = "#read_time"
			};
		}

        public static CalibreCustomFieldModel CreateSyncBookField(bool? value = null)
		{
			return new CalibreCustomFieldModel()
			{
				//ExtraValue = null,
				Value = value,
				CategorySort = "value",
				ColumnNumber = 4,
				Column = "value",
				Datatype = "bool",
				Display = new Dictionary<string, object>
				{
					{ "description", "If the book should be synced to a device" }
				},
				//IsCategory = false,
				//IsCsp = false,
				IsCustom = true,
				IsEditable = true,
				//IsMultiple = null,
				IsMultiple2 = new Dictionary<string, string>(),
				Kind = "field",
				Label = "sync_book",
				LinkColumn = "value",
				Name = "Sync Book",
				RecIndex = 28,
				SearchTerms = new string[] { "#sync_book" },
				Table = "custom_column_4",
				PropertyName = "#sync_book"
			};
		}

		internal string SerializeToJson()
		{
			return JsonSerializer.Serialize(this, new JsonSerializerOptions()
			{
				WriteIndented = true
			});
		}
	}
}
