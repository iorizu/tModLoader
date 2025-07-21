using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace tDataExtractor
{
	internal class RarityDictionary
	{
		internal static Dictionary<string, int> rarityNameRankMap = new Dictionary<string, int>() {
			{ "Amber", -11 },
			{ "Trash", -1 },
			{ "Normal", 0 },
			{ "Blue", 1 },
			{ "Green", 2 },
			{ "Orange", 3 },
			{ "Red", 4 },
			{ "Pink", 5 },
			{ "Purple", 6 },
			{ "Lime", 7 },
			{ "Yellow", 8 },
			{ "Cyan", 9 },
		};
		internal static Dictionary<int, string> rarityRankTierMap = new Dictionary<int, string>() {
			{ -13, "Master" },
			{ -12, "Expert" },
			{ -11, "Quest" },
			{ -1, "-1" },
			{ 0, "0" },
			{ 1, "1" },
			{ 2, "2" },
			{ 3, "3" },
			{ 4, "4" },
			{ 5, "5" },
			{ 6, "6" },
			{ 7, "7" },
			{ 8, "8" },
			{ 9, "9" },
			{ 10, "10" },
			{ 11, "11" },
		};
		internal static Dictionary<int, string> rarityRankColorMap = new Dictionary<int, string>() {
			{ -13, "Fiery Red" },
			{ -12, "Rainbow" },
			{ -11, "Amber" },
			{ -1, "Gray" },
			{ 0, "White" },
			{ 1, "Blue" },
			{ 2, "Green" },
			{ 3, "Orange" },
			{ 4, "Light Red" },
			{ 5, "Pink" },
			{ 6, "Light Purple" },
			{ 7, "Lime" },
			{ 8, "Yellow" },
			{ 9, "Cyan" },
			{ 10, "Red" },
			{ 11, "Purple" },
		};

		[JsonInclude]
		internal List<RarityDef> rarityDefs;

		internal RarityDictionary()
		{
			rarityDefs = GetRarityDefs().ToList();
		}

		internal static IEnumerable<RarityDef> GetRarityDefs()
		{
			var colors = typeof(Terraria.ID.Colors)
				.GetFields(BindingFlags.Static | BindingFlags.Public)
				.Where((fieldInfo) => fieldInfo.FieldType.Name == nameof(Color) && fieldInfo.IsInitOnly)
				.Where((fieldInfo) => fieldInfo.Name.StartsWith("Rarity"))
				.Select(RarityDef.CreateRarityDef);

			yield return new RarityDef(-13, rarityRankTierMap[-13], null);
			yield return new RarityDef(-12, rarityRankTierMap[-12], null);

			foreach (var rarityDef in colors)
			{
				yield return rarityDef;
			}
		}
	}

	internal class RarityDef
	{
		[JsonInclude]
		internal readonly int tier;
		[JsonInclude]
		internal readonly string tierName;
		[JsonInclude]
		internal readonly Color? color;

		internal RarityDef(int tier, string tierName, Color? color)
		{
			this.tier = tier;
			this.tierName = tierName;
			this.color = color;
		}

		internal static RarityDef CreateRarityDef(FieldInfo fieldInfo)
		{
			var color = (Color)fieldInfo.GetValue(null);

			var rarityNameStartIdx = "Rarity".Length;
			var colorName = fieldInfo.Name.Substring(rarityNameStartIdx);

			int rank;
			string tierName;
			if (RarityDictionary.rarityNameRankMap.ContainsKey(colorName))
			{
				rank = RarityDictionary.rarityNameRankMap[colorName];
				tierName = rank < -1 ? RarityDictionary.rarityRankColorMap[rank] : RarityDictionary.rarityRankTierMap[rank];
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(colorName));
			}

			return new RarityDef(rank, tierName, color);
		}
	}
}
