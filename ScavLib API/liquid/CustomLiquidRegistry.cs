
using System;
using System.Collections.Generic;

namespace ScavLib.liquid
{

    public static class CustomLiquidRegistry
    {
        private static readonly Dictionary<string, (LiquidType type, string owner)> _pending
            = new Dictionary<string, (LiquidType, string)>(StringComparer.OrdinalIgnoreCase);

        internal static bool LiquidsInitialized { get; set; } = false;

        internal static bool TryRegister(string id, LiquidType type, string owner, out string error)
        {
            error = null;
            if (string.IsNullOrEmpty(id)) { error = "Liquid id is null/empty."; return Fail(error); }
            if (type == null) { error = $"Liquid '{id}' has null LiquidType."; return Fail(error); }

            if (_pending.TryGetValue(id, out var existing) &&
                !string.Equals(existing.owner, owner, StringComparison.OrdinalIgnoreCase))
            {
                error = $"Liquid id '{id}' already registered by '{existing.owner ?? "<unknown>"}'.";
                return Fail(error);
            }

            _pending[id] = (type, owner);

            if (LiquidsInitialized && Liquids.Registry != null)
            {
                Liquids.Registry[id] = type;
                ScavLibPlugin.Log.LogInfo(
                    $"[CustomLiquidRegistry] Late-registered liquid '{id}' into vanilla registry " +
                    $"(owner: {owner ?? "<none>"}).");
            }

            return true;
        }

        internal static void FlushIntoRegistry()
        {
            LiquidsInitialized = true;

            if (Liquids.Registry == null) return;
            foreach (var kv in _pending)
            {
                Liquids.Registry[kv.Key] = kv.Value.type;
                ScavLibPlugin.Log.LogInfo(
                    $"[CustomLiquidRegistry] Registered liquid '{kv.Key}' " +
                    $"(owner: {kv.Value.owner ?? "<none>"}).");
            }
        }

        public static bool Contains(string id)
            => !string.IsNullOrEmpty(id) && _pending.ContainsKey(id);

        private static bool Fail(string msg)
        {
            ScavLibPlugin.Log.LogError($"[CustomLiquidRegistry] {msg}");
            return false;
        }
    }
}
