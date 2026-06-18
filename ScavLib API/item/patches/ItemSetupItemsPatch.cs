using HarmonyLib;
using ScavLib.liquid;

namespace ScavLib.item.patches
{

    [HarmonyPatch(typeof(Item), nameof(Item.SetupItems))]
    internal static class ItemSetupItemsPatch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            try { CustomLiquidRegistry.FlushIntoRegistry(); }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ItemSetupItemsPatch] Liquid flush failed: {ex}");
            }
        }

        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {

                CustomItemBuilder.FlushPending();

                CustomItemRegistry.FlushIntoGlobalItems();
                ItemLootPool.InitializePool();
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ItemSetupItemsPatch] Item/loot-pool flush failed: {ex}");
            }
        }
    }
}
