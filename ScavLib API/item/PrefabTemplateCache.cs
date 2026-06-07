using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.item
{

    internal static class PrefabTemplateCache
    {
        private static readonly Dictionary<string, Object> _cache
            = new Dictionary<string, Object>();

        internal static Object Resolve(string templateId)
        {
            if (string.IsNullOrEmpty(templateId)) return null;

            if (_cache.TryGetValue(templateId, out var cached) && cached != null)
                return cached;

            var loaded = Resources.Load(templateId);
            if (loaded != null)
                _cache[templateId] = loaded;
            else
                ScavLibPlugin.Log.LogError(
                    $"[PrefabTemplateCache] Resources.Load('{templateId}') returned null. " +
                    $"A custom item using this template cannot be spawned.");

            return loaded;
        }

        internal static void Clear() => _cache.Clear();
    }
}
