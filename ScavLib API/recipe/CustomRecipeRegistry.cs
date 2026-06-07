
using System.Collections.Generic;

namespace ScavLib.recipe
{

    public static class CustomRecipeRegistry
    {
        private static readonly List<(Recipe recipe, string owner)> _pending
            = new List<(Recipe, string)>();

        internal static void Enqueue(Recipe recipe, string owner)
        {
            if (recipe == null) return;
            _pending.Add((recipe, owner));
        }

        internal static void FlushIntoRecipes()
        {
            if (Recipes.recipes == null) return;

            foreach (var (recipe, owner) in _pending)
            {
                if (recipe == null) continue;

                recipe.index = Recipes.recipes.Count;

                if (recipe.items != null)
                {
                    string ignoredId;
                    if (recipe.isRepair || recipe.result == null)
                        ignoredId = "";
                    else
                        ignoredId = recipe.result.id ?? "";

                    foreach (var ri in recipe.items)
                    {
                        if (ri == null) continue;

                        if (!string.IsNullOrEmpty(ri.specificId))
                            ri.specific = true;

                        ri.ignoredId = ignoredId;
                    }
                }

                Recipes.recipes.Add(recipe);

                ScavLibPlugin.Log.LogInfo(
                    $"[CustomRecipeRegistry] Added recipe for '{recipe.result?.id}' " +
                    $"(owner: {owner ?? "<none>"}, index: {recipe.index}).");
            }

            _pending.Clear();
        }

        public static int PendingCount => _pending.Count;
    }
}
