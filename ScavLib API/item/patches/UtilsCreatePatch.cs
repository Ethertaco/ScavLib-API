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

                __result = Object.Instantiate(template, pos, Quaternion.Euler(0f, 0f, rot)) as GameObject;
                FinishSpawn(__result, custom, id);
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

                __result = Object.Instantiate(template, trans) as GameObject;
                FinishSpawn(__result, custom, id);
                return false;
            }
        }

        private static void FinishSpawn(GameObject go, CustomItem custom, string id)
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
