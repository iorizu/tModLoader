using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace tDataExtractor
{
    internal class Program
    {
		public static void Main(string[] args)
		{
			var count = 0;
			foreach (ItemDef itemDef in GetItems()) {
				count++;
				Console.WriteLine(itemDef.ToString());
			}
			Console.WriteLine("Count: " + count);
		}

		private static IEnumerable<ItemDef> GetItems()
		{
			return typeof(ItemID)
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

	class ItemDef
	{
		internal readonly short Id;
		internal readonly string Name;

		internal ItemDef(short id, string name)
		{
			Id = id;
			Name = name;
		}

		public override string ToString()
		{
			return Name + " " + Id;
		}
	}
}
