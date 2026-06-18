using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using ScavLib.event_bus;
using ScavLib.input.events_;

namespace ScavLib.input
{
    internal class KeyBindHost : MonoBehaviour
    {
        private static GameObject _hostObject;

        private static FieldInfo _consoleOpenedField;
        private static bool _consoleOpenedResolved;

        private static FieldInfo _pausedField;
        private static bool _pausedResolved;

        internal static void EnsureSpawned()
        {
            if (_hostObject != null) return;
            _hostObject = new GameObject("ScavLibKeyBindHost");
            UnityEngine.Object.DontDestroyOnLoad(_hostObject);
            _hostObject.AddComponent<KeyBindHost>();
            ScavLibPlugin.Log.LogInfo("[KeyBindHost] Spawned.");
        }

        private void Update()
        {
            if (FocusBlocked()) return;

            var registry = KeyBindRegistry.Internal_RegistryView;
            if (registry == null || registry.Count == 0) return;

            KeyBindDefinition[] snapshot;
            try
            {
                snapshot = new KeyBindDefinition[registry.Count];
                int idx = 0;
                foreach (var kv in registry)
                {
                    if (idx >= snapshot.Length) break;
                    snapshot[idx++] = kv.Value;
                }
                if (idx < snapshot.Length) Array.Resize(ref snapshot, idx);
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError($"[KeyBindHost] Snapshot failed: {ex}");
                return;
            }

            foreach (var def in snapshot)
            {
                if (def == null) continue;
                KeyCode kc = KeyBinds.GetBind(def.FullId);
                if (kc == KeyCode.None) continue;

                try
                {
                    if (Input.GetKeyDown(kc))
                    {

                        def.OnPressed?.Invoke();
                        EventBus.Post(new KeyBindPressedEvent(def));
                    }
                    if (Input.GetKey(kc))
                        EventBus.Post(new KeyBindHeldEvent(def));
                    if (Input.GetKeyUp(kc))
                        EventBus.Post(new KeyBindReleasedEvent(def));
                }
                catch (Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[KeyBindHost] Dispatch loop crashed on '{def.FullId}': {ex}");
                }
            }
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
                var es = EventSystem.current;
                if (es != null && es.currentSelectedGameObject != null)
                {
                    var go = es.currentSelectedGameObject;
                    if (go.GetComponent<TMPro.TMP_InputField>() != null) return true;
                    if (go.GetComponent<UnityEngine.UI.InputField>() != null) return true;
                }
            }
            catch { }

            try
            {
                if (!_consoleOpenedResolved)
                {
                    var t = typeof(ConsoleScript);
                    _consoleOpenedField = t.GetField("opened",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _consoleOpenedResolved = true;
                }
                if (_consoleOpenedField != null && ConsoleScript.instance != null)
                {
                    object raw = _consoleOpenedField.GetValue(ConsoleScript.instance);
                    if (raw is bool b && b) return true;
                }
            }
            catch { }

            try
            {
                if (!_pausedResolved)
                {
                    var t = Type.GetType("PauseHandler");
                    if (t != null)
                    {
                        _pausedField = t.GetField("paused",
                            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    }
                    _pausedResolved = true;
                }
                if (_pausedField != null)
                {
                    object raw = _pausedField.GetValue(null);
                    if (raw is bool b && b) return true;
                }
            }
            catch { }

            return false;
        }
    }
}
