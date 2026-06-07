using HarmonyLib;

namespace ScavLib.recipe.patches
{

    [HarmonyPatch(typeof(Recipes), nameof(Recipes.SetUpRecipes))]
    internal static class RecipesSetUpRecipesPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                CustomRecipeRegistry.FlushIntoRecipes();
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[RecipesSetUpRecipesPatch] Failed to flush custom recipes: {ex}");
            }
        }
    }
}
