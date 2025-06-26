using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace tDataExtractor
{
	internal class TileDictionary
	{
		[JsonInclude]
		internal List<TileDef> tileDefs;

		internal TileDictionary()
		{
			tileDefs = GetTiles().ToList();
		}

		internal IEnumerable<TileDef> GetTiles()
		{
			return typeof(Terraria.ID.TileID)
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where((fieldInfo) => fieldInfo.FieldType.Name == nameof(UInt16) && fieldInfo.IsLiteral)
				.Where((fieldInfo) => {
					object value = fieldInfo.GetRawConstantValue();
					if (value is ushort valueUshort) {
						return valueUshort >= 0 && valueUshort < 693;
					}
					return false;
				})
				.Select(TileDef.CreateNewTileDef);
		}
	}

	internal class TileDef
	{
		[JsonInclude]
		internal readonly ushort id;
		[JsonInclude]
		internal readonly string name;

		private TileDef(ushort id, string name)
		{
			this.id = id;
			this.name = name;
		}

		internal static TileDef CreateNewTileDef(FieldInfo fieldInfo)
		{
			return new TileDef((ushort)fieldInfo.GetRawConstantValue(), fieldInfo.Name);
		}
	}
}
