using System.Collections.Generic;

namespace ScavLib.recipe
{

    public class RecipeBuilder
    {
        private readonly string _owner;
        private readonly Recipe _recipe;

        private RecipeBuilder(string resultId, string owner, float resultCondition, int amount, bool isLiquid)
        {
            _owner = owner;
            _recipe = new Recipe
            {
                INT = 0,
                items = new List<RecipeItem>(),
                category = Recipes.RecipeCategory.Materials,
                result = new RecipeResult
                {
                    id = resultId,
                    amount = amount,
                    resultCondition = resultCondition,
                    isLiquid = isLiquid
                }
            };
        }

        public static RecipeBuilder Create(string resultId, string owner)
            => new RecipeBuilder(resultId, owner, 1f, 1, false);

        public static RecipeBuilder CreateLiquid(string resultId, string owner, float resultCondition)
            => new RecipeBuilder(resultId, owner, resultCondition, 1, true);

        public RecipeBuilder RequireINT(int level) { _recipe.INT = level; return this; }
        public RecipeBuilder Category(Recipes.RecipeCategory c) { _recipe.category = c; return this; }
        public RecipeBuilder Amount(int n) { _recipe.result.amount = n; return this; }
        public RecipeBuilder IsRepair(bool b = true) { _recipe.isRepair = b; return this; }

        public RecipeBuilder SpecialKnown(bool b = true)
        {
            _recipe.specialKnown = b;
            return this;
        }

        public RecipeBuilder DontDrainResultLiquid(bool b = true)
        {
            _recipe.result.dontDrainResultLiquid = b;
            return this;
        }

        public RecipeBuilder Ingredient(string specificId, float minCondition = 0.9f,
                                        bool destroy = true, bool isLiquid = false)
        {
            _recipe.items.Add(new RecipeItem(minCondition)
            {
                specific = true,
                specificId = specificId,
                minimumCondition = minCondition,
                destroyItem = destroy,
                isLiquid = isLiquid
            });
            return this;
        }

        public RecipeBuilder IngredientByQuality(string qualityId, float amount = 1f,
                                                 bool destroy = true, bool isLiquid = false,
                                                 float minCondition = 0f)
        {
            _recipe.items.Add(new RecipeItem(amount)
            {
                specific = false,
                quality = new CraftingQuality(qualityId, amount),
                minimumCondition = minCondition,
                destroyItem = destroy,
                isLiquid = isLiquid
            });
            return this;
        }

        public bool Register()
        {
            if (_recipe.result == null || string.IsNullOrEmpty(_recipe.result.id))
            {
                ScavLibPlugin.Log.LogError("[RecipeBuilder] Recipe has no result id.");
                return false;
            }
            CustomRecipeRegistry.Enqueue(_recipe, _owner);
            return true;
        }
    }
}
