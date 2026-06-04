using HarmonyLib;
using ScavLib.event_bus.events;

namespace ScavLib.event_bus.patches
{

    [HarmonyPatch(typeof(Body), nameof(Body.PickUpItem))]
    internal static class ItemPickupPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Body __instance, Item item, int slot)
        {
            if (__instance == null || item == null) return;

            try
            {
                var slotItem = __instance.GetItem(slot);
                if (slotItem != item) return;

                EventBus.Post(new ItemPickedUpEvent(item, slot, __instance));
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ItemPickupPatch] Failed to post ItemPickedUpEvent: {ex}");
            }
        }
    }
}
