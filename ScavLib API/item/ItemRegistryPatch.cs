using HarmonyLib;

namespace ScavLib.item
{

    [HarmonyPatch(typeof(Item), nameof(Item.SetupItems))]
    internal static class ItemRegistryPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                CustomItemRegistry.FlushPending();
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ItemRegistryPatch] Failed to flush custom items: {ex}");
            }
        }
    }
}
