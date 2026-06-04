using HarmonyLib;
using ScavLib.event_bus.events;

namespace ScavLib.event_bus.patches
{

    [HarmonyPatch(typeof(Body), nameof(Body.DropItem), new[] { typeof(Item) })]
    internal static class ItemDropPatch
    {
        [HarmonyPrefix]
        private static void Prefix(Body __instance, Item item, out int __state)
        {
            __state = -1;
            if (__instance == null || item == null) return;

            for (int i = 0; i < __instance.slots.Length; i++)
            {
                if (__instance.GetItem(i) == item)
                {
                    __state = i;
                    return;
                }
            }
        }

        [HarmonyPostfix]
        private static void Postfix(Body __instance, Item item, int __state)
        {
            if (__instance == null || item == null) return;
            if (__state < 0) return;

            try
            {

                var slotItem = __instance.GetItem(__state);
                if (slotItem == item) return;

                EventBus.Post(new ItemDroppedEvent(item, __state, __instance));
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ItemDropPatch] Failed to post ItemDroppedEvent: {ex}");
            }
        }
    }
}
