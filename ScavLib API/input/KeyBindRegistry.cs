using System;
using System.Collections.Generic;
using UnityEngine;
using ScavLib.i18n;

namespace ScavLib.input
{
    public static class KeyBindRegistry
    {
        private static readonly Dictionary<string, KeyBindDefinition> _registry
            = new Dictionary<string, KeyBindDefinition>(StringComparer.OrdinalIgnoreCase);

        internal static bool SuppressApplySave = false;

        public static bool TryRegister(KeyBindDefinition def, out string error)
        {
            error = null;
            if (def == null)
            {
                error = "KeyBindDefinition is null.";
                Err(error);
                return false;
            }
            if (string.IsNullOrEmpty(def.FullId))
            {
                error = "KeyBindDefinition has empty FullId.";
                Err(error);
                return false;
            }

            if (_registry.TryGetValue(def.FullId, out var old))
            {
                foreach (var h in def.Handlers) old.AddHandler(h);

                if (def.DisplayNames != null && def.DisplayNames.Count > 0)
                    LocaleManager.RegisterString("gameset" + def.FullId, def.DisplayNames);
                if (def.Descriptions != null && def.Descriptions.Count > 0)
                    LocaleManager.RegisterString("gameset" + def.FullId + "dsc", def.Descriptions);

                ScavLibPlugin.Log.LogInfo(
                    $"[KeyBindRegistry] Merged into existing keybind '{def.FullId}' " +
                    $"(owner: {old.OwnerModName}, total handlers: {old.Handlers.Count}). " +
                    $"Default key / category remain as originally registered.");
                return true;
            }

            _registry[def.FullId] = def;

            if (def.DisplayNames != null && def.DisplayNames.Count > 0)
                LocaleManager.RegisterString("gameset" + def.FullId, def.DisplayNames);
            if (def.Descriptions != null && def.Descriptions.Count > 0)
                LocaleManager.RegisterString("gameset" + def.FullId + "dsc", def.Descriptions);

            if (Settings.settings != null)
                InjectIntoLoadedSettings(def);

            ScavLibPlugin.Log.LogInfo(
                $"[KeyBindRegistry] Registered keybind '{def.FullId}' " +
                $"(owner: {def.OwnerModName}, default: {def.DefaultKey}).");
            return true;
        }

        public static bool Unregister(string ownerModName, string localId)
        {
            string fullId = BuildFullId(ownerModName, localId);
            if (fullId == null) return false;

            if (!_registry.Remove(fullId)) return false;

            if (Settings.settings != null)
            {
                Settings.settings.RemoveAll(s => s is SettingKeybind &&
                    string.Equals(s.name, fullId, StringComparison.OrdinalIgnoreCase));
            }

            ScavLibPlugin.Log.LogInfo($"[KeyBindRegistry] Unregistered '{fullId}'.");
            return true;
        }

        public static bool ClearHandlers(string ownerModName, string localId)
        {
            string fullId = BuildFullId(ownerModName, localId);
            if (fullId == null) return false;
            if (!_registry.TryGetValue(fullId, out var d)) return false;
            d.ClearHandlers();
            ScavLibPlugin.Log.LogInfo($"[KeyBindRegistry] Cleared all handlers on '{fullId}'.");
            return true;
        }

        public static bool RemoveHandler(string ownerModName, string localId, Action handler)
        {
            if (handler == null) return false;
            string fullId = BuildFullId(ownerModName, localId);
            if (fullId == null) return false;
            if (!_registry.TryGetValue(fullId, out var d)) return false;
            return d.RemoveHandler(handler);
        }

        public static KeyCode GetKeyCode(string ownerModName, string localId)
        {
            string fullId = BuildFullId(ownerModName, localId);
            if (fullId == null) return KeyCode.None;
            return KeyBinds.GetBind(fullId);
        }

        public static KeyCode GetKeyCodeRaw(string fullId)
        {
            if (string.IsNullOrEmpty(fullId)) return KeyCode.None;
            return KeyBinds.GetBind(fullId);
        }

        public static bool IsDown(string ownerModName, string localId)
            => !FocusBlocked() && Input.GetKeyDown(GetKeyCode(ownerModName, localId));

        public static bool IsHeld(string ownerModName, string localId)
            => !FocusBlocked() && Input.GetKey(GetKeyCode(ownerModName, localId));

        public static bool IsUp(string ownerModName, string localId)
            => !FocusBlocked() && Input.GetKeyUp(GetKeyCode(ownerModName, localId));

        public static IReadOnlyCollection<KeyBindDefinition> GetAllRegistered()
            => _registry.Values;

        internal static IReadOnlyDictionary<string, KeyBindDefinition> Internal_RegistryView
            => _registry;

        internal static string BuildFullId(string ownerModName, string localId)
        {
            string sOwner = ScavLibPaths.SafeOwnerName(ownerModName);
            string sLocal = ScavLibPaths.SafeLocalId(localId);
            if (sOwner == null || sLocal == null) return null;
            return sOwner + "_" + sLocal;
        }

        private static void InjectIntoLoadedSettings(KeyBindDefinition def)
        {
            SettingKeybind existing = null;
            foreach (var s in Settings.settings)
            {
                if (s is SettingKeybind sk &&
                    string.Equals(sk.name, def.FullId, StringComparison.OrdinalIgnoreCase))
                {
                    existing = sk;
                    break;
                }
            }

            if (existing == null)
            {
                existing = BuildSettingKeybind(def);
                Settings.settings.Add(existing);
            }

            KeyCode effective = def.DefaultKey;
            if (ModKeybindStore.TryGet(def.OwnerModName, def.LocalId, out var fromStore))
                effective = fromStore;
            else
                effective = existing.value != KeyCode.None ? existing.value : def.DefaultKey;

            existing.value = effective;
            existing.Apply();
        }

        internal static SettingKeybind BuildSettingKeybind(KeyBindDefinition def)
        {
            var sk = new SettingKeybind
            {
                name = def.FullId,
                value = def.DefaultKey,
                category = Setting.SettingCategory.Input,
            };

            string owner = def.OwnerModName;
            string localId = def.LocalId;
            sk.apply = () =>
            {
                if (SuppressApplySave) return;
                try
                {
                    var kc = KeyBinds.GetBind(def.FullId);
                    ModKeybindStore.Save(owner, localId, kc);
                }
                catch (Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[KeyBindRegistry] apply-save failed for '{def.FullId}': {ex.Message}");
                }
            };
            return sk;
        }

        private static bool FocusBlocked()
        {
            try
            {
                if (SettingsMenu.instance != null) return true;
            }
            catch { }
            try
            {
                var es = UnityEngine.EventSystems.EventSystem.current;
                if (es != null && es.currentSelectedGameObject != null)
                {
                    var go = es.currentSelectedGameObject;
                    if (go.GetComponent<TMPro.TMP_InputField>() != null) return true;
                    if (go.GetComponent<UnityEngine.UI.InputField>() != null) return true;
                }
            }
            catch { }
            return false;
        }

        private static void Err(string s)
            => ScavLibPlugin.Log.LogError($"[KeyBindRegistry] {s}");
    }
}
