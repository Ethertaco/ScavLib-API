using System;
using System.Reflection;
using HarmonyLib;
using KrokoshaCasualtiesMP;
using ScavLib.item;
using UnityEngine;

namespace ScavLib.compat.krokmp
{

    [HarmonyPatch]
    internal static class KrokMpGoSyncPatch
    {
        private static MethodBase TargetMethod()
        {
            var t = AccessTools.TypeByName(
                "KrokoshaCasualtiesMP.NewCoolerObjectPacketWriteReadSystem");
            if (t == null)
            {
                ScavLibPlugin.Log.LogWarning(
                    "[KrokMpGoSyncPatch] NewCoolerObjectPacketWriteReadSystem not found; " +
                    "custom items will not be visible to remote KrokMP clients.");
                return null;
            }
            var m = AccessTools.Method(t, "LoadObjectResource");
            if (m == null)
            {
                ScavLibPlugin.Log.LogWarning(
                    "[KrokMpGoSyncPatch] LoadObjectResource(string, in Vector2) not found.");
            }
            return m;
        }

        private static bool Prefix(string resourceid, ref Vector2 pos,
                                   ref GameObject __result)
        {
            if (!CustomItemRegistry.TryGet(resourceid, out CustomItem custom))
                return true;

            var template = PrefabTemplateCache.Resolve(custom.TemplateId);
            if (template == null)
            {
                ScavLibPlugin.Log.LogError(
                    $"[KrokMpGoSyncPatch] Cannot resolve template " +
                    $"'{custom.TemplateId}' for custom id '{resourceid}'.");
                __result = null;
                return false;
            }

            var go = UnityEngine.Object.Instantiate(template, pos, Quaternion.identity) as GameObject;
            if (go == null) { __result = null; return false; }

            if (go.TryGetComponent<Item>(out var itemComp))
                itemComp.id = resourceid;

            var tag = go.AddComponent<CustomItemTag>();
            tag.CustomItemId = resourceid;
            tag.Owner = custom.Owner;

            if (custom.OnSpawn != null)
            {
                try { custom.OnSpawn(go); }
                catch (Exception e)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[KrokMpGoSyncPatch] OnSpawn for '{resourceid}' threw: {e}");
                }
            }

            __result = go;
            return false;
        }
    }
}
