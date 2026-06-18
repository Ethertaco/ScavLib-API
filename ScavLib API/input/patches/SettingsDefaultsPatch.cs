using System.Collections.Generic;
using HarmonyLib;

namespace ScavLib.input.patches
{

    [HarmonyPatch(typeof(Settings), nameof(Settings.DefaultSettings))]
    internal static class SettingsDefaultsPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ref List<Setting> __result)
        {
            if (__result == null) return;

            try
            {
                var registry = KeyBindRegistry.Internal_RegistryView;
                if (registry == null || registry.Count == 0) return;

                foreach (var kv in registry)
                {
                    var def = kv.Value;

                    bool exists = false;
                    for (int i = 0; i < __result.Count; i++)
                    {
                        var s = __result[i];
                        if (s is SettingKeybind &&
                            string.Equals(s.name, def.FullId,
                                System.StringComparison.OrdinalIgnoreCase))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (exists) continue;

                    __result.Add(KeyBindRegistry.BuildSettingKeybind(def));
                }
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SettingsDefaultsPatch] Failed to inject keybinds: {ex}");
            }
        }
    }
}
