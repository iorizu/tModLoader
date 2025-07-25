using System;
using System.Collections.Generic;
using System.IO;
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
			GetItemsJson();
			Terraria.Lang.InitializeLegacyLocalization();

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

		private static void GetItemsJson()
		{
			string cwd = Directory.GetCurrentDirectory();
			DirectoryInfo parent = Directory.GetParent(cwd);

			while (!parent.FullName.EndsWith("tModLoader")) {
				parent = Directory.GetParent(parent.FullName);
			}

			var path = Path.Combine(parent.FullName, "src\\Terraria\\Terraria\\Localization\\Content\\en-US\\Items.json");
			var contents = File.ReadAllText(path);
			Terraria.Localization.LanguageManager.Instance.LoadLanguageFromFileTextJson(contents, true);	
		}
	}

	internal class ItemDef
	{
		[JsonInclude]
		internal readonly short id;
		[JsonInclude]
		internal readonly string name;
		[JsonInclude]
		internal readonly string displayName;
		[JsonInclude]
		internal readonly string tooltip;

		// Mode
		[JsonInclude]
		internal bool modeExpert;
		// Common
		[JsonInclude]
		internal int maxStack;
		[JsonInclude]
		internal int value;
		[JsonInclude]
		internal int rarityId;
		[JsonInclude]
		internal int projectileId;
		// Catagorization
		[JsonInclude]
		internal bool isVanity;
		[JsonInclude]
		internal bool isMaterial;
		[JsonInclude]
		internal bool isConsumable;
		[JsonInclude]
		internal bool isAccessory;
		[JsonInclude]
		internal bool isArmor;
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
		// Stats
		[JsonInclude]
		internal int damage;
		[JsonInclude]
		internal float knockback;
		[JsonInclude]
		internal float shootSpeed;
		[JsonInclude]
		internal int defense;
		[JsonInclude]
		internal int manaCost;
		[JsonInclude]
		internal int regenLife;
		[JsonInclude]
		internal int powerPick;
		[JsonInclude]
		internal int powerAxe;
		[JsonInclude]
		internal int powerHammer;
		[JsonInclude]
		internal int powerFishing;

		#region Unused properties
		// Slots
		//internal sbyte slotHead;
		//internal sbyte slotFace;
		//internal sbyte slotNeck;
		//internal sbyte slotBeard;
		//internal sbyte slotBody;
		//internal sbyte slotFront;
		//internal sbyte slotBack;
		//internal sbyte slotWaist;
		//internal sbyte slotHandOn;
		//internal sbyte slotHandOff;
		//internal sbyte slotLeg;
		//internal sbyte slotShoe;
		//internal sbyte slotShield;
		//internal sbyte slotWing;
		//internal sbyte slotBalloon;
		#endregion

		private ItemDef(short id, string name, string displayName, string tooltip)
		{
			this.id = id;
			this.name = name;
			this.displayName = displayName;
			this.tooltip = tooltip;
		}

		internal static ItemDef CreateNewItemDef(FieldInfo fieldInfo)
		{
			var type = (short)fieldInfo.GetRawConstantValue();
			var itemSlot = Terraria.Item.NewItem(null, 0, 0, 0, 0, type);
			var item = Terraria.Main.item[itemSlot];
			item.active = false; // Set to false so that next call to NewItem() reuses the same slot
			var displayName = Terraria.Lang.GetItemName(type);
			item.RebuildTooltip();
			var tooltipLines = new List<string>(); // More than enough for any tooltip
			for (int i = 0; i < item.ToolTip.Lines; i++) {
				tooltipLines.Add(item.ToolTip.GetLine(i));
			}
			var tooltip = string.Join("\n", tooltipLines.ToArray()); // TODO: Tooltips needs to have $-vars replaced

			return new ItemDef(type, fieldInfo.Name, displayName.Value, tooltip) {
				modeExpert = item.expert, // Ignoring Item.expertOnly because it is never set to true AFAICT
				maxStack = item.maxStack,
				value = item.value,
				rarityId = item.expert ? -12 : item.rare, // See Terraria.Main.DrawMouseOver()
				projectileId = item.shoot, // 0 = No projectile
				// Categorization
				isVanity = item.vanity,
				isMaterial = item.material, // TODO: Investigate this, it's not being set in the places I would expect
				isConsumable = item.consumable,
				isAccessory = item.accessory,
				isArmor = item.wornArmor,
				isPotion = item.potion,
				isMelee = item.melee,
				isRanged = item.ranged,
				isMagic = item.magic,
				isSummon = item.summon,
				// Stats
				damage = item.damage,
				knockback = item.knockBack,
				shootSpeed = item.shootSpeed,
				defense = item.defense,
				manaCost = item.mana,
				regenLife = item.lifeRegen,
				powerPick = item.pick,
				powerAxe = item.axe,
				powerHammer = item.hammer,
				powerFishing = item.fishingPole,
			};
		}
	}
}
