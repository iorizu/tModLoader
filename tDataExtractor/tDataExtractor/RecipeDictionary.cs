using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tDataExtractor
{
	internal class RecipeDictionary
	{
		internal static void InitializeTerrariaMain()
		{
			//Terraria.Main.netMode = 0; // Single player (https://docs.tmodloader.net/docs/stable/class_netmode_i_d.html)
			//Terraria.Main.SavePath = "";
		}

		internal static IEnumerable<RecipeDef> GetRecipes()
		{
			Terraria.Recipe.SetupRecipes();
			foreach (var recipe in Terraria.Main.recipe.Where((recipe) => !(recipe is null))) {
				var recipeDef = new RecipeDef {
					needHoney = recipe.needHoney,
					needWater = recipe.needWater,
					needLava = recipe.needLava,
					anyWood = recipe.anyWood,
					anyIronBar = recipe.anyIronBar,
					anyPressurePlate = recipe.anyPressurePlate,
					anySand = recipe.anySand,
					anyFragment = recipe.anyFragment,
					alchemy = recipe.alchemy,
					needSnowBiome = recipe.needSnowBiome,
					needGraveyardBiome = recipe.needGraveyardBiome,
					needEverythingSeed = recipe.needEverythingSeed,
					notDecraftable = recipe.notDecraftable,
					crimson = recipe.crimson,
					corruption = recipe.corruption,
					itemId = recipe.createItem.type,
					stack = recipe.createItem.stack
				};
				foreach (var requiredItem in recipe.requiredItem.Where((reqItem) => reqItem.stack > 0)) {
					var recipeInput = new RecipeInput(requiredItem.type, requiredItem.stack);
					recipeDef.recipeInputs.Add(recipeInput);
				}
				foreach (var requiredTileId in recipe.requiredTile.Where((reqTile) => reqTile > 0)) {
					recipeDef.requiredTiles.Add(requiredTileId);
				}
				if (recipeDef.requiredTiles.Count > 1) {
					Console.WriteLine($"Found one with more than one required tile: {recipeDef.requiredTiles.Count}");
				}
				yield return recipeDef;
			}
		}
	}

	internal class RecipeDef
	{
		internal bool needHoney = false;
		internal bool needWater = false;
		internal bool needLava = false;
		internal bool anyWood = false;
		internal bool anyIronBar = false;
		internal bool anyPressurePlate = false;
		internal bool anySand = false;
		internal bool anyFragment = false;
		internal bool alchemy = false;
		internal bool needSnowBiome = false;
		internal bool needGraveyardBiome = false;
		internal bool needEverythingSeed = false;
		internal bool notDecraftable = false;
		internal bool crimson = false;
		internal bool corruption = false;

		internal int itemId = 0;
		internal int stack = 1;

		internal List<RecipeInput> recipeInputs = null;
		internal List<int> requiredTiles = null;

		internal RecipeDef()
		{
			recipeInputs = new List<RecipeInput>();
			requiredTiles = new List<int>();
		}

		public override string ToString()
		{
			string[] envReqArray = new string[] {
				needHoney ? "Honey" : "",
				needWater ? "Water" : "",
				needLava ? "Lava" : "",
				anyWood ? "Any Wood" : "",
				anyIronBar ? "Any Iron Bar" : "",
				anyPressurePlate ? "Any Pressure Plate" : "",
				anySand ? "Any Sand" : "",
				anyFragment ? "Any Fragment" : "",
				alchemy ? "Alchemy" : "",
				needSnowBiome ? "Snow Biome" : "",
				needGraveyardBiome ? "Graveyard Biome" : "",
				needEverythingSeed ? "Everything Seed" : "",
				crimson ? "Crimson" : "",
				corruption ? "Corruption" : ""
			};
			string envRequirements = string.Join(", ", envReqArray.Where((s) => !string.IsNullOrEmpty(s)));
			string envRequirementsString = !string.IsNullOrEmpty(envRequirements) ? $"(Needs: {envRequirements})" : "";

			string notDecraftableString = notDecraftable ? "(Not Decraftable)" : "";

			string recipeInputsString = $"[{string.Join(", ", recipeInputs.Select((recipeInput) => recipeInput.ToString()))}]";

			string reqTilesString = $"[{string.Join(", ", requiredTiles.Select((rt) => rt.ToString()))}]";

			return string.Join(" ", $"Item {itemId} (Stack: {stack})", envRequirementsString, notDecraftableString)
				+ "\n" + recipeInputsString
				+ "\n" + reqTilesString;
		}
	}

	internal class RecipeInput
	{
		internal int itemId;
		internal int stack;

		internal RecipeInput(int itemId, int stack)
		{
			this.itemId = itemId;
			this.stack = stack;
		}

		public override string ToString()
		{
			return $"Item {itemId} (Stack: {stack})";
		}
	}
}
