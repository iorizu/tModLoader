using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;

namespace tDataExtractor
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length == 0) {
				return;
			}

			var extractOptsRaw = args.Take(args.Length - 1);
			var extractOpts = ExtractOpts.Parse(extractOptsRaw);

			var outputPath = args.Last();
			if (!Directory.Exists(outputPath)) {
				Directory.CreateDirectory(outputPath);
			}

			SetupContext(outputPath);

			var jsonOpts = new JsonSerializerOptions() {
				WriteIndented = true
			};

			if (extractOpts.extractItems) {
				var items = JsonSerializer.Serialize(new ItemDictionary(), jsonOpts);
				File.WriteAllText(Path.Combine(outputPath, "items.json"), items);
			}
			if (extractOpts.extractRecipes) {
				var recipes = JsonSerializer.Serialize(new RecipeDictionary(), jsonOpts);
				File.WriteAllText(Path.Combine(outputPath, "recipes.json"), recipes);
			}
			if (extractOpts.extractTiles) {
				var tiles = JsonSerializer.Serialize(new TileDictionary(), jsonOpts);
				File.WriteAllText(Path.Combine(outputPath, "tiles.json"), tiles);
			}
		}

		private static void SetupContext(string outputPath)
		{
			Terraria.Program.SavePath = outputPath;
			// Program.SavePath must be set in a seperate method from any code that references
			// Terraria.Main because Terraria.Main's init ctor is called before the first line
			// in the method that references it is executed. If you do happen to reference Main
			// in a method when Program.SavePath has not been set, the ctor crashes because when
			// trying to initialize Main.WorldPath because Main.SavePath (which returns
			// Program.SavePath) is still null.
			SetupMain();
		}

		private static void SetupMain()
		{
			Terraria.Main.netMode = 0; // netMode.SinglePlayer = 0 https://docs.tmodloader.net/docs/stable/class_netmode_i_d.html
			Terraria.Main.myPlayer = 0; // Index into player array
			Terraria.Main.player[0] = new Terraria.Player {
				hairColor = new Color(0, 0, 0),
				skinColor = new Color(0, 0, 0),
				shirtColor = new Color(0, 0, 0),
				pantsColor = new Color(0, 0, 0)
			};
			// Initialize one Item so that, when we create Items in ItemDictionary to extract their properties, the program doesn't crash
			Terraria.Main.item[0] = new Terraria.Item();

			// Initialize all recipes (like it is done in Main.Initialize_AlmostEverything()) so that Recipe.SetupRecipes()->ShimmerTransforms.UpdateRecipeSets() doesn't crash
			for (int j = 0; j < Recipe.maxRecipes; j++) {
				Terraria.Main.recipe[j] = new Recipe();
			}
		}
	}

	internal struct ExtractOpts
	{
		internal bool extractItems;
		internal bool extractRecipes;
		internal bool extractTiles;

		internal static ExtractOpts Parse(IEnumerable<string> input)
		{
			bool extractItems = false;
			bool extractRecipes = false;
			bool extractTiles = false;

			foreach (string option in input) {
				switch (option) {
					case "-i":
						extractItems = true;
						break;
					case "-r":
						extractRecipes = true;
						break;
					case "-t":
						extractTiles = true;
						break;
					default:
						throw new ArgumentException($"Invalid type specifier found: {option}");
				}
			}

			return new ExtractOpts() {
				extractItems = extractItems,
				extractRecipes = extractRecipes,
				extractTiles = extractTiles
			};
		}
	}
}
