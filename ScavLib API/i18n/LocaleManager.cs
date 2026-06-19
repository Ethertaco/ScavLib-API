using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace ScavLib.i18n
{
    public static class LocaleManager
    {
        private static readonly Dictionary<string, JObject> JsonLocaleCache = new Dictionary<string, JObject>();
        private static readonly Dictionary<string, IReadOnlyDictionary<string, string>> ManualItems = new Dictionary<string, IReadOnlyDictionary<string, string>>();
        private static readonly Dictionary<string, IReadOnlyDictionary<string, string>> ManualOthers = new Dictionary<string, IReadOnlyDictionary<string, string>>();

        public static readonly HashSet<string> RegisteredItemIds = new HashSet<string>();

        public static void AutoRegister(string modId, string modFolderPath)
        {
            string langPath = Path.Combine(modFolderPath, "lang");
            if (!Directory.Exists(langPath)) return;

            if (!JsonLocaleCache.ContainsKey(modId)) JsonLocaleCache[modId] = new JObject();

            LoadSmart(modId, langPath, "EN");

            string current = GetGameLanguageCode();
            if (current != "EN")
            {
                if (!LoadSmart(modId, langPath, current))
                {
                    if (current.Contains("-"))
                    {
                        string shortCode = current.Split('-')[0];
                        LoadSmart(modId, langPath, shortCode);
                    }
                }
            }
        }

        private static bool LoadSmart(string modId, string dir, string code)
        {
            var files = Directory.GetFiles(dir, "*.json");
            var targetFile = files.FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f).Equals(code, StringComparison.OrdinalIgnoreCase));

            if (targetFile != null)
            {
                try
                {
                    string json = File.ReadAllText(targetFile);
                    JsonLocaleCache[modId].Merge(JObject.Parse(json), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union,
                        MergeNullValueHandling = MergeNullValueHandling.Ignore
                    });
                    return true;
                }
                catch (Exception ex)
                {
                    ScavLibPlugin.Log.LogError($"[ScavLib.i18n] Failed to load {code}.json for {modId}: {ex.Message}");
                }
            }
            return false;
        }

        public static void PerformInjection()
        {
            if (Locale.currentLang == null) return;

            string langCode = GetGameLanguageCode();

            InjectManualToNative(ManualItems, Locale.currentLang.main, langCode);
            InjectManualToNative(ManualOthers, Locale.currentLang.other, langCode);

            foreach (var mod in JsonLocaleCache)
            {
                JObject root = mod.Value;

                InjectJsonToDictionary(root["items"], Locale.currentLang.main);
                InjectJsonToDictionary(root["buildings"], Locale.currentLang.buildings);
                InjectJsonToDictionary(root["moodles"], Locale.currentLang.moodles);
                InjectJsonToDictionary(root["other"], Locale.currentLang.other);

                InjectComplexStructures(root);

                InjectJsonToDictionary(root, Locale.currentLang.other, ignoreReserved: true);
            }
        }

        private static void InjectComplexStructures(JObject root)
        {

            if (root["character"] is JArray charArray)
            {
                for (int i = 0; i < charArray.Count && i < Locale.currentLang.character.Length; i++)
                {
                    if (charArray[i] is JObject charData)
                        InjectJsonToCharDictionary(charData, Locale.currentLang.character[i]);
                }
            }

            if (root["pauseQuotes"] is JArray quotes)
            {
                foreach (var q in quotes)
                {
                    string s = q.ToString();
                    if (!Locale.currentLang.pauseQuotes.Contains(s))
                        Locale.currentLang.pauseQuotes.Add(s);
                }
            }
        }

        private static void InjectJsonToCharDictionary(JObject source, Dictionary<string, List<string>> target)
        {
            if (source == null) return;
            foreach (var property in source.Properties())
            {

                if (property.Value.Type == JTokenType.Array)
                {
                    target[property.Name] = property.Value.Select(t => t.ToString()).ToList();
                }
                else 
                {
                    target[property.Name] = new List<string> { property.Value.ToString() };
                }
            }
        }

        private static void InjectJsonToDictionary(JToken source, Dictionary<string, string> target, bool ignoreReserved = false)
        {
            if (source == null || !source.HasValues) return;
            var reserved = new[] { "items", "buildings", "moodles", "other", "character", "notes", "pauseQuotes", "pdaNotes" };

            foreach (var pair in Flatten(source as JObject))
            {
                if (ignoreReserved && reserved.Any(r => pair.Key.Equals(r, StringComparison.OrdinalIgnoreCase))) continue;
                target[pair.Key] = pair.Value;
            }
        }

        private static void InjectManualToNative(Dictionary<string, IReadOnlyDictionary<string, string>> source, Dictionary<string, string> target, string langCode)
        {
            foreach (var entry in source)
            {
                if (entry.Value.TryGetValue(langCode, out string text) ||
                    entry.Value.TryGetValue("EN", out text) ||
                    entry.Value.TryGetValue("English", out text))
                {
                    target[entry.Key] = text;
                }
                else if (entry.Value.Count > 0)
                {
                    target[entry.Key] = entry.Value.Values.First();
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> Flatten(JObject jObject, string prefix = "")
        {
            if (jObject == null) yield break;
            foreach (var property in jObject.Properties())
            {
                string key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                if (property.Value is JObject nested)
                    foreach (var pair in Flatten(nested, key)) yield return pair;
                else if (property.Value.Type != JTokenType.Array)
                    yield return new KeyValuePair<string, string>(key, property.Value.ToString());
            }
        }

        public static string GetGameLanguageCode() => Locale.currentLangName ?? PlayerPrefs.GetString("locale", "EN");

        public static void RegisterItem(string id, IReadOnlyDictionary<string, string> names, IReadOnlyDictionary<string, string> descs = null)
        {
            RegisteredItemIds.Add(id);
            if (names != null) Locale.currentLang.main[id] = names[GetGameLanguageCode()];
            if (descs != null) ManualOthers[id + "dsc"] = descs;
        }

        public static void RegisterString(string key, IReadOnlyDictionary<string, string> translations)
        {
            if (translations != null) ManualOthers[key] = translations;
        }

        public static void RefreshAllUI()
        {
            var localizers = UnityEngine.Object.FindObjectsOfType<UILocalizer>();
            foreach (var ui in localizers)
            {
                if (ui.TryGetComponent<TextMeshProUGUI>(out var textComp))
                {
                    string translated = Locale.GetOther(ui.key);
                    textComp.text = ui.upper ? translated.ToUpper() : translated;
                }
            }
        }
    }
}
