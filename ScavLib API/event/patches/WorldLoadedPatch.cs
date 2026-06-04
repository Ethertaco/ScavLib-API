using HarmonyLib;
using ScavLib.event_bus.events;

namespace ScavLib.event_bus.patches
{

    [HarmonyPatch(typeof(ConsoleScript), "Start")]
    internal static class WorldLoadedPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                EventBus.Post(new WorldLoadedEvent());
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[WorldLoadedPatch] Failed to post WorldLoadedEvent: {ex}");
            }
        }
    }
}
