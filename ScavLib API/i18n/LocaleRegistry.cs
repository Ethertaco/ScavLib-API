
using System.Collections.Generic;

namespace ScavLib.i18n
{

    public static class LocaleRegistry
    {

        private static readonly Dictionary<string, Dictionary<string, string>> _names
            = new Dictionary<string, Dictionary<string, string>>();
        private static readonly Dictionary<string, Dictionary<string, string>> _descs
            = new Dictionary<string, Dictionary<string, string>>();

        private const string Fallback = "EN";

        internal static void RegisterItem(string id,
            IReadOnlyDictionary<string, string> names,
            IReadOnlyDictionary<string, string> descs)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (names != null) foreach (var kv in names) Put(_names, kv.Key, id, kv.Value);
            if (descs != null) foreach (var kv in descs) Put(_descs, kv.Key, id, kv.Value);
        }

        internal static void Flush()
        {
            var lang = Locale.currentLang;
            if (lang == null || lang.main == null) return;

            string code = Locale.currentLangName;

            WriteLayer(_names, code, lang.main, suffix: "");
            WriteLayer(_descs, code, lang.main, suffix: "dsc");

            ScavLibPlugin.Log.LogInfo(
                $"[LocaleRegistry] Flushed custom i18n for language '{code}'.");
        }

        private static void WriteLayer(
            Dictionary<string, Dictionary<string, string>> store,
            string code, Dictionary<string, string> target, string suffix)
        {

            ApplyLang(store, Fallback, target, suffix);
            if (!string.Equals(code, Fallback, System.StringComparison.OrdinalIgnoreCase))
                ApplyLang(store, code, target, suffix);
        }

        private static void ApplyLang(
            Dictionary<string, Dictionary<string, string>> store,
            string code, Dictionary<string, string> target, string suffix)
        {
            if (code == null || !store.TryGetValue(code, out var byId)) return;
            foreach (var kv in byId)
                target[kv.Key + suffix] = kv.Value;
        }

        private static void Put(
            Dictionary<string, Dictionary<string, string>> store,
            string lang, string id, string text)
        {
            if (!store.TryGetValue(lang, out var byId))
            {
                byId = new Dictionary<string, string>();
                store[lang] = byId;
            }
            byId[id] = text;
        }
    }
}
