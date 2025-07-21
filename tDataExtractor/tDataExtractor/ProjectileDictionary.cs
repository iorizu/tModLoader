using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace tDataExtractor
{
	internal class ProjectileDictionary
	{
		[JsonInclude]
		internal List<ProjectileDef> projectileDefs;

		internal ProjectileDictionary()
		{
			projectileDefs = GetProjectiles().ToList();
		}

		internal static IEnumerable<ProjectileDef> GetProjectiles()
		{
			return typeof(Terraria.ID.ProjectileID)
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where((fieldInfo) => fieldInfo.FieldType.Name == nameof(Int16) && fieldInfo.IsLiteral)
				.Where((fieldInfo) => {
					object value = fieldInfo.GetRawConstantValue();
					if (value is short valueShort) {
						return valueShort > 0 && valueShort < 1022;
					}
					return false;
				})
				.Select(ProjectileDef.CreateProjectileDef);
		}
	}

	internal class ProjectileDef
	{
		[JsonInclude]
		internal readonly short id;
		[JsonInclude]
		internal readonly string name;

		internal ProjectileDef(short id, string name)
		{
			this.id = id;
			this.name = name;
		}

		internal static ProjectileDef CreateProjectileDef(FieldInfo fieldInfo)
		{
			var id = (short)fieldInfo.GetRawConstantValue();
			return new ProjectileDef(id, fieldInfo.Name);
		}
	}
}
