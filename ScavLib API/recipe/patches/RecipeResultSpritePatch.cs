using HarmonyLib;
using ScavLib.item;
using UnityEngine;

namespace ScavLib.recipe.patches
{

    [HarmonyPatch(typeof(Recipe), nameof(Recipe.resultSprite), MethodType.Getter)]
    internal static class RecipeResultSpritePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Recipe __instance, ref (Sprite, Color) __result)
        {
            if (__instance?.result == null) return true;
            if (__instance.result.isLiquid) return true;

            if (CustomItemRegistry.TryGet(__instance.result.id, out var custom) &&
                custom.Sprite != null)
            {
                __result = (custom.Sprite, Color.white);
                return false;
            }
            return true;
        }
    }
}
