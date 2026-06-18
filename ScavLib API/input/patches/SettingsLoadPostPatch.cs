using HarmonyLib;
using UnityEngine;

namespace ScavLib.input.patches
{

    [HarmonyPatch(typeof(Settings), nameof(Settings.LoadSettings))]
    internal static class SettingsLoadPostPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            if (Settings.settings == null) return;

            KeyBindRegistry.SuppressApplySave = true;
            try
            {
                var registry = KeyBindRegistry.Internal_RegistryView;
                if (registry == null) return;

                foreach (var kv in registry)
                {
                    var def = kv.Value;
                    if (!ModKeybindStore.TryGet(def.OwnerModName, def.LocalId, out KeyCode truth))
                        continue;

                    foreach (var s in Settings.settings)
                    {
                        if (s is SettingKeybind sk &&
                            string.Equals(sk.name, def.FullId,
                                System.StringComparison.OrdinalIgnoreCase))
                        {
                            sk.value = truth;
                            sk.Apply();
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SettingsLoadPostPatch] Failed to overlay truth values: {ex}");
            }
            finally
            {
                KeyBindRegistry.SuppressApplySave = false;
            }
        }
    }
}
