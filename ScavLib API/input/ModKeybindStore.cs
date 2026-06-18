using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ScavLib.input
{

    internal static class ModKeybindStore
    {

        private static readonly Dictionary<string, Dictionary<string, KeyCode>> _cache
            = new Dictionary<string, Dictionary<string, KeyCode>>();

        internal static Dictionary<string, KeyCode> Load(string owner)
        {
            string safe = ScavLibPaths.SafeOwnerName(owner);
            if (safe == null) return new Dictionary<string, KeyCode>();

            if (_cache.TryGetValue(safe, out var cached)) return cached;

            var dict = new Dictionary<string, KeyCode>();
            string path = ScavLibPaths.KeybindsFile(owner);
            if (path != null && File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    if (parsed != null)
                    {
                        foreach (var kv in parsed)
                        {
                            if (System.Enum.TryParse<KeyCode>(kv.Value, out var kc))
                                dict[kv.Key] = kc;
                            else
                                dict[kv.Key] = KeyCode.None;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[ModKeybindStore] Failed to parse '{path}': {ex.Message}. " +
                        $"Treating as empty; next save will rewrite.");
                }
            }

            _cache[safe] = dict;
            return dict;
        }

        internal static bool TryGet(string owner, string localId, out KeyCode value)
        {
            value = KeyCode.None;
            string safeLocal = ScavLibPaths.SafeLocalId(localId);
            if (safeLocal == null) return false;

            var dict = Load(owner);
            return dict.TryGetValue(safeLocal, out value);
        }

        internal static void Save(string owner, string localId, KeyCode value)
        {
            string safeLocal = ScavLibPaths.SafeLocalId(localId);
            if (safeLocal == null) return;

            var dict = Load(owner);
            dict[safeLocal] = value;

            string dir = ScavLibPaths.EnsureModDir(owner);
            if (dir == null) return;
            string path = ScavLibPaths.KeybindsFile(owner);
            if (path == null) return;

            var serializable = new Dictionary<string, string>(dict.Count);
            foreach (var kv in dict) serializable[kv.Key] = kv.Value.ToString();

            try
            {
                string json = JsonConvert.SerializeObject(serializable, Formatting.Indented);
                string tmp = path + ".tmp";
                File.WriteAllText(tmp, json);

                if (File.Exists(path))
                    File.Replace(tmp, path, null);
                else
                    File.Move(tmp, path);
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ModKeybindStore] Failed to write '{path}': {ex.Message}");
            }
        }
    }
}
