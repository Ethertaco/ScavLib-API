using HarmonyLib;
using UnityEngine;

namespace ScavLib.item.patches
{

    internal static class UtilsCreatePatch
    {
        [HarmonyPatch(typeof(Utils), nameof(Utils.Create),
            new[] { typeof(string), typeof(Vector2), typeof(float) })]
        internal static class PosRot
        {
            [HarmonyPrefix]
            [HarmonyPriority(Priority.Low)]
            private static bool Prefix(string id, Vector2 pos, float rot, ref GameObject __result)
            {
                if (!CustomItemRegistry.TryGet(id, out var custom)) return true;

                var template = PrefabTemplateCache.Resolve(custom.TemplateId);
                if (template == null) return true;

                __result = InstantiateDeferred(
                    template,
                    inst => Object.Instantiate(inst, pos, Quaternion.Euler(0f, 0f, rot)) as GameObject,
                    custom, id);
                return false;
            }
        }

        [HarmonyPatch(typeof(Utils), nameof(Utils.Create),
            new[] { typeof(string), typeof(Transform) })]
        internal static class Parented
        {
            [HarmonyPrefix]
            [HarmonyPriority(Priority.Low)]
            private static bool Prefix(string id, Transform trans, ref GameObject __result)
            {
                if (!CustomItemRegistry.TryGet(id, out var custom)) return true;

                var template = PrefabTemplateCache.Resolve(custom.TemplateId);
                if (template == null) return true;

                __result = InstantiateDeferred(
                    template,
                    inst => Object.Instantiate(inst, trans) as GameObject,
                    custom, id);
                return false;
            }
        }

        private static GameObject InstantiateDeferred(
            Object templateObj,
            System.Func<Object, GameObject> instantiator,
            CustomItem custom, string id)
        {

            var prefabGO = templateObj as GameObject;

            if (prefabGO == null)
            {
                var fallback = instantiator(templateObj);
                FinishSpawn(fallback, custom, id);
                return fallback;
            }

            bool prefabWasActive = prefabGO.activeSelf;
            GameObject clone = null;

            try
            {

                if (prefabWasActive) prefabGO.SetActive(false);

                clone = instantiator(prefabGO);
                if (clone == null)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[UtilsCreatePatch] Object.Instantiate returned null for '{id}'.");
                    return null;
                }

                FinishSpawn(clone, custom, id, runOnSpawn: false);

                clone.SetActive(true);

                try { custom.OnSpawn?.Invoke(clone); }
                catch (System.Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[UtilsCreatePatch] OnSpawn hook threw for '{id}': {ex}");
                }
            }
            finally
            {

                if (prefabWasActive && prefabGO != null) prefabGO.SetActive(true);
            }

            return clone;
        }

        private static void FinishSpawn(GameObject go, CustomItem custom, string id,
                                        bool runOnSpawn = true)
        {
            if (go == null) return;

            try
            {
                go.name = id;

                if (custom.Sprite != null &&
                    go.TryGetComponent<SpriteRenderer>(out var sr))
                    sr.sprite = custom.Sprite;

                if (go.TryGetComponent<Item>(out var item))
                    item.id = id;

                if (custom.LiquidFillSprite != null &&
                    go.TryGetComponent<WaterContainerItem>(out var wc))
                    wc.fillSprite = custom.LiquidFillSprite;

                var tag = go.AddComponent<CustomItemTag>();
                tag.CustomItemId = id;
                tag.Owner = custom.Owner;

                if (runOnSpawn)
                    custom.OnSpawn?.Invoke(go);
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[UtilsCreatePatch] Failed finishing spawn of '{id}': {ex}");
            }
        }
    }
}
