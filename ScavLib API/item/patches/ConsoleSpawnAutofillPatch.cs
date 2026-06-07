using System.Collections.Generic;
using HarmonyLib;

namespace ScavLib.item.patches
{

    [HarmonyPatch(typeof(ConsoleScript), nameof(ConsoleScript.RegisterSpawnEntities))]
    internal static class ConsoleSpawnAutofillPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                var cmd = ConsoleScript.SearchExact("spawn");
                if (cmd == null) return;

                if (cmd.argAutofill == null)
                    cmd.argAutofill = new System.Collections.Generic.Dictionary<int, List<string>>();

                if (!cmd.argAutofill.TryGetValue(0, out var fills) || fills == null)
                {
                    fills = new List<string>();
                    cmd.argAutofill[0] = fills;
                }

                foreach (var id in CustomItemRegistry.GetAllIds())
                    if (!fills.Contains(id))
                        fills.Add(id);
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ConsoleSpawnAutofillPatch] Failed to add custom ids to spawn autofill: {ex}");
            }
        }
    }
}
