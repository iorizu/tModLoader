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
			foreach (var recipeDef in RecipeDictionary.GetRecipes()) {
				//Console.WriteLine(recipeDef);
			}
		}

    }
}
