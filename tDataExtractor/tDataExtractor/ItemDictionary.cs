using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace tDataExtractor
{
	internal class ItemDictionary
	{
		internal List<ItemDef> itemDefs;

		internal ItemDictionary()
		{
			itemDefs = LoadItems().ToList();
		}

		internal ItemDef GetItem(ItemId itemId)
		{
			return itemDefs.Find((itemDef) => itemDef.id == itemId.id);
		}

		private static IEnumerable<ItemDef> LoadItems()
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
		internal readonly short id;
		internal readonly string name;

		internal ItemDef(short id, string name)
		{
			this.id = id;
			this.name = name;
		}

		internal ItemId GetId()
		{
			return new ItemId(id);
		}
	}

	internal class ItemId
	{
		internal short id;

		internal ItemId(short id)
		{
			this.id = id;
		}
	}
}
