using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScavLib.input.patches
{

    [HarmonyPatch(typeof(SettingsMenu), nameof(SettingsMenu.SelectTab),
        new[] { typeof(Setting.SettingCategory) })]
    internal static class SettingsMenuTabPatch
    {
        internal class ScavLibTabEntry : MonoBehaviour { }

        private static readonly FieldInfo _spawnedField = typeof(SettingsMenu).GetField(
            "spawnedSettings",
            BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPostfix]
        private static void Postfix(SettingsMenu __instance, Setting.SettingCategory category)
        {
            if (__instance == null) return;

            DestroyOurEntries(__instance.content);

            if (category != Setting.SettingCategory.Input) return;

            try
            {
                AppendModSections(__instance);
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SettingsMenuTabPatch] Failed to append mod sections: {ex}");
            }
        }

        private static void DestroyOurEntries(RectTransform content)
        {
            if (content == null) return;
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                var child = content.GetChild(i);
                if (child != null && child.GetComponent<ScavLibTabEntry>() != null)
                    UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        private static void AppendModSections(SettingsMenu menu)
        {
            var registry = KeyBindRegistry.Internal_RegistryView;
            if (registry == null || registry.Count == 0) return;

            var groups = new List<KeyValuePair<string, List<KeyBindDefinition>>>();
            var indexOf = new Dictionary<string, int>(StringComparer.Ordinal);

            foreach (var kv in registry)
            {
                var def = kv.Value;
                if (string.IsNullOrEmpty(def.Category)) continue;

                if (!indexOf.TryGetValue(def.Category, out int slot))
                {
                    slot = groups.Count;
                    indexOf[def.Category] = slot;
                    groups.Add(new KeyValuePair<string, List<KeyBindDefinition>>(
                        def.Category, new List<KeyBindDefinition>()));
                }
                groups[slot].Value.Add(def);
            }

            if (groups.Count == 0) return;

            HideAndCompactNativeRows(menu, groups);

            float num = menu.content.sizeDelta.y;
            foreach (var grp in groups)
            {
                num += SpawnHeader(menu, grp.Key, num);
                foreach (var def in grp.Value)
                    num += SpawnKeybindRow(menu, def, num);
            }
            menu.content.sizeDelta = new Vector2(menu.content.sizeDelta.x, num);
        }

        private static void HideAndCompactNativeRows(
            SettingsMenu menu,
            List<KeyValuePair<string, List<KeyBindDefinition>>> groups)
        {
            if (_spawnedField == null) return;
            var spawned = _spawnedField.GetValue(menu) as List<GameObject>;
            if (spawned == null) return;

            var toRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var grp in groups)
                foreach (var def in grp.Value)
                    toRemove.Add(def.FullId);

            var nativeOrder = new List<Setting>();
            if (Settings.settings != null)
            {
                foreach (var s in Settings.settings)
                    if (s != null && s.category == Setting.SettingCategory.Input)
                        nativeOrder.Add(s);
            }

            int n = Math.Min(nativeOrder.Count, spawned.Count);
            float shift = 0f;

            for (int i = 0; i < n; i++)
            {
                var go = spawned[i];
                if (go == null) continue;
                var rt = go.GetComponent<RectTransform>();
                if (rt == null) continue;

                bool kill = nativeOrder[i] is SettingKeybind sk &&
                            toRemove.Contains(sk.name);

                if (kill)
                {
                    shift += rt.sizeDelta.y;
                    UnityEngine.Object.Destroy(go);
                    spawned[i] = null;
                }
                else if (shift > 0f)
                {
                    rt.anchoredPosition = new Vector2(
                        rt.anchoredPosition.x,
                        rt.anchoredPosition.y + shift);
                }
            }

            if (shift > 0f)
            {
                menu.content.sizeDelta = new Vector2(
                    menu.content.sizeDelta.x,
                    Mathf.Max(0f, menu.content.sizeDelta.y - shift));
            }
        }

        private static float SpawnHeader(SettingsMenu menu, string label, float yOffset)
        {
            GameObject g;
            try
            {
                g = Utils.Create("Special/GameSettingBool", menu.content);
            }
            catch
            {
                g = new GameObject("ScavLibHeader",
                    typeof(RectTransform), typeof(TextMeshProUGUI));
                g.transform.SetParent(menu.content, false);
                var rt = g.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(menu.content.sizeDelta.x, 24f);
                var t = g.GetComponent<TextMeshProUGUI>();
                t.text = label;
                MarkAndPlace(g, yOffset);
                return rt.sizeDelta.y;
            }

            try
            {
                var labelTxt = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (labelTxt != null) labelTxt.text = label;
            }
            catch { }

            try
            {
                if (g.transform.childCount > 1)
                {
                    var togglerChild = g.transform.GetChild(1);
                    var tog = togglerChild.GetComponent<Toggle>();
                    if (tog != null) tog.interactable = false;
                    togglerChild.gameObject.SetActive(false);
                }
            }
            catch { }

            MarkAndPlace(g, yOffset);
            var rt2 = g.GetComponent<RectTransform>();
            return rt2 != null ? rt2.sizeDelta.y : 24f;
        }

        private static float SpawnKeybindRow(SettingsMenu menu, KeyBindDefinition def, float yOffset)
        {
            GameObject g;
            try
            {
                g = Utils.Create("Special/GameSettingInput", menu.content);
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SettingsMenuTabPatch] Failed to instantiate row for '{def.FullId}': {ex.Message}");
                return 0f;
            }

            SettingKeybind keybind = null;
            if (Settings.settings != null)
            {
                foreach (var s in Settings.settings)
                {
                    if (s is SettingKeybind sk &&
                        string.Equals(sk.name, def.FullId,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        keybind = sk;
                        break;
                    }
                }
            }
            if (keybind == null)
            {
                UnityEngine.Object.Destroy(g);
                return 0f;
            }

            try
            {
                string locKey = "gameset" + def.FullId;
                string labelText = Locale.GetOther(locKey);
                if (string.IsNullOrEmpty(labelText) || labelText == locKey)
                {
                    labelText = def.DisplayNames != null &&
                                def.DisplayNames.TryGetValue("EN", out var en)
                        ? en
                        : def.LocalId;
                }
                var lbl = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (lbl != null) lbl.text = labelText;
            }
            catch { }

            try
            {
                var btn = g.transform.GetChild(1).GetComponent<Button>();
                var btnTxt = btn.GetComponentInChildren<TextMeshProUGUI>();
                btnTxt.text = keybind.GetDisplayString();

                btn.onClick.AddListener(() =>
                {
                    menu.StartCoroutine(WaitForKeyPress(keybind, btnTxt));
                });
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SettingsMenuTabPatch] Failed to wire button for '{def.FullId}': {ex.Message}");
            }

            MarkAndPlace(g, yOffset);
            var rt = g.GetComponent<RectTransform>();
            return rt != null ? rt.sizeDelta.y : 24f;
        }

        private static IEnumerator WaitForKeyPress(SettingKeybind keybind, TextMeshProUGUI txt)
        {
            txt.text = "...";

            yield return null;
            yield return null;

            while (true)
            {
                foreach (object obj in Enum.GetValues(typeof(KeyCode)))
                {
                    var kc = (KeyCode)obj;

                    if (kc >= KeyCode.Mouse0 && kc <= KeyCode.Mouse6) continue;

                    if (Input.GetKeyDown(kc))
                    {
                        keybind.SetValue(kc);
                        keybind.Apply();
                        txt.text = keybind.GetDisplayString();
                        yield break;
                    }
                }
                yield return null;
            }
        }

        private static void MarkAndPlace(GameObject g, float yOffset)
        {
            if (g == null) return;
            g.AddComponent<ScavLibTabEntry>();
            var rt = g.GetComponent<RectTransform>();
            if (rt == null) return;
            rt.anchoredPosition = new Vector2(
                0f,
                -yOffset - rt.sizeDelta.y * 0.5f);
        }
    }
}
