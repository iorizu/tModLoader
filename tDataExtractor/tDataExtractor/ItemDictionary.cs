using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace tDataExtractor
{
	internal class ItemDictionary
	{
		[JsonInclude]
		internal List<ItemDef> itemDefs;

		internal ItemDictionary()
		{
			itemDefs = GetItems().ToList();
		}

		private static IEnumerable<ItemDef> GetItems()
		{
			return typeof(Terraria.ID.ItemID)
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where((fieldInfo) => fieldInfo.FieldType.Name == nameof(Int16) && fieldInfo.IsLiteral)
				.Where((fieldInfo) => {
					object value = fieldInfo.GetRawConstantValue();
					if (value is short valueShort) {
						return valueShort > 0;
					}
					return false;
				})
				.Select(fieldInfo => new ItemDef((short)fieldInfo.GetRawConstantValue(), fieldInfo.Name));
		}
	}

	internal class ItemDef
	{
		[JsonInclude]
		internal readonly short id;
		[JsonInclude]
		internal readonly string name;

		internal ItemDef(short id, string name)
		{
			this.id = id;
			this.name = name;
		}
	}
}
