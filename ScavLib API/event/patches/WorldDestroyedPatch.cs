using HarmonyLib;
using ScavLib.event_bus.events;

namespace ScavLib.event_bus.patches
{

    [HarmonyPatch(typeof(WorldGeneration), "OnDestroy")]
    internal static class WorldDestroyedPatch
    {
        [HarmonyPrefix]
        private static void Prefix(WorldGeneration __instance, out int __state)
        {
            __state = __instance != null ? __instance.biomeDepth : -1;
        }

        [HarmonyPostfix]
        private static void Postfix(int __state)
        {
            bool wasSaveAndExit = WorldUnloadingPatch.PendingSaveAndExit;
            WorldUnloadingPatch.PendingSaveAndExit = false;

            LayerLoadedPatch.ResetState();

            try
            {
                EventBus.Post(new WorldDestroyedEvent(__state, wasSaveAndExit));
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[WorldDestroyedPatch] Failed to post WorldDestroyedEvent: {ex}");
            }
        }
    }
}
