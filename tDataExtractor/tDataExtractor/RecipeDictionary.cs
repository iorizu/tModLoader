using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace tDataExtractor
{
	internal class RecipeDictionary
	{
		[JsonInclude]
		internal List<RecipeDef> recipeDefs;

		internal RecipeDictionary()
		{
			recipeDefs = GetRecipes().ToList();
		}

		internal static IEnumerable<RecipeDef> GetRecipes()
		{
			Terraria.Recipe.SetupRecipes();
			foreach (var recipe in Terraria.Main.recipe.Where((recipe) => recipe.createItem.type > 0)) {
				var recipeDef = new RecipeDef((short)recipe.createItem.type, recipe.createItem.stack) {
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
					corruption = recipe.corruption
				};
				foreach (var requiredItem in recipe.requiredItem.Where((reqItem) => reqItem.stack > 0)) {
					var recipeInput = new RecipeInput((short)requiredItem.type, requiredItem.stack);
					recipeDef.recipeInputs.Add(recipeInput);
				}
				foreach (var requiredTileId in recipe.requiredTile.Where((reqTile) => reqTile > 0)) {
					recipeDef.requiredTiles.Add(requiredTileId);
				}
				yield return recipeDef;
			}
		}
	}

	internal class RecipeDef
	{
		[JsonInclude]
		internal bool needHoney = false;
		[JsonInclude]
		internal bool needWater = false;
		[JsonInclude]
		internal bool needLava = false;
		[JsonInclude]
		internal bool anyWood = false;
		[JsonInclude]
		internal bool anyIronBar = false;
		[JsonInclude]
		internal bool anyPressurePlate = false;
		[JsonInclude]
		internal bool anySand = false;
		[JsonInclude]
		internal bool anyFragment = false;
		[JsonInclude]
		internal bool alchemy = false;
		[JsonInclude]
		internal bool needSnowBiome = false;
		[JsonInclude]
		internal bool needGraveyardBiome = false;
		[JsonInclude]
		internal bool needEverythingSeed = false;
		[JsonInclude]
		internal bool notDecraftable = false;
		[JsonInclude]
		internal bool crimson = false;
		[JsonInclude]
		internal bool corruption = false;

		[JsonInclude]
		internal short itemId;
		[JsonInclude]
		internal int stack = 1;

		[JsonInclude]
		internal List<RecipeInput> recipeInputs = null;
		[JsonInclude]
		internal List<int> requiredTiles = null;

		internal RecipeDef(short itemId, int stack)
		{
			this.itemId = itemId;
			this.stack = stack;
			recipeInputs = new List<RecipeInput>();
			requiredTiles = new List<int>();
		}
	}

	internal class RecipeInput
	{
		[JsonInclude]
		internal readonly short itemId;
		[JsonInclude]
		internal readonly int stack;

		internal RecipeInput(short itemId, int stack)
		{
			this.itemId = itemId;
			this.stack = stack;
		}
	}
}
