using HarmonyLib;
using ScavLib.event_bus.events;

namespace ScavLib.event_bus.patches
{

    internal static class WorldUnloadingState
    {
        internal static bool PendingSaveAndExit = false;
    }

    internal static class WorldUnloadingPatch
    {
        internal static bool PendingSaveAndExit
        {
            get => WorldUnloadingState.PendingSaveAndExit;
            set => WorldUnloadingState.PendingSaveAndExit = value;
        }
    }

    [HarmonyPatch(typeof(WorldGeneration), "ContinueRun")]
    internal static class WorldUnloadingContinueRunPatch
    {
        [HarmonyPrefix]
        private static void Prefix(WorldGeneration __instance)
        {
            if (__instance == null) return;

            bool doingRegen = Traverse.Create(__instance)
                                      .Field<bool>("doingRegen")
                                      .Value;

            if (doingRegen) return;
            if (__instance.generatingWorld) return;
            if (!__instance.worldExists) return;

            try
            {
                EventBus.Post(new WorldUnloadingEvent(
                    __instance.biomeDepth,
                    __instance.biomeDepth + 1));
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[WorldUnloadingPatch] Failed to post on ContinueRun: {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(WorldGeneration), "SaveAndExit")]
    internal static class WorldUnloadingSaveAndExitPatch
    {
        [HarmonyPrefix]
        private static void Prefix(WorldGeneration __instance)
        {
            if (__instance == null) return;

            WorldUnloadingState.PendingSaveAndExit = true;

            try
            {
                EventBus.Post(new WorldUnloadingEvent(
                    __instance.biomeDepth,
                    -1));
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[WorldUnloadingPatch] Failed to post on SaveAndExit: {ex}");
            }
        }
    }
}
