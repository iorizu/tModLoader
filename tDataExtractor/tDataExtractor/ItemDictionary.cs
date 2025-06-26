using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Terraria;

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
				.Select(ItemDef.CreateNewItemDef);
		}
	}

	internal class ItemDef
	{
		[JsonInclude]
		internal readonly short id;
		[JsonInclude]
		internal readonly string name;

		[JsonInclude]
		internal bool isAccessory;
		[JsonInclude]
		internal bool isPotion;
		[JsonInclude]
		internal bool isMelee;
		[JsonInclude]
		internal bool isRanged;
		[JsonInclude]
		internal bool isMagic;
		[JsonInclude]
		internal bool isSummon;

		private ItemDef(short id, string name)
		{
			this.id = id;
			this.name = name;
		}

		internal static ItemDef CreateNewItemDef(FieldInfo fieldInfo)
		{
			var type = (short)fieldInfo.GetRawConstantValue();
			var itemSlot = Terraria.Item.NewItem(null, 0, 0, 0, 0, type);
			var item = Main.item[itemSlot];
			item.active = false; // Set to false so that next call to NewItem reuses the same slot

			return new ItemDef(type, fieldInfo.Name) {
				isAccessory = item.accessory,
				isPotion = item.potion,
				// Weapon props
				isMelee = item.melee,
				isRanged = item.ranged,
				isMagic = item.magic,
				isSummon = item.summon,
			};
		}
	}
}
