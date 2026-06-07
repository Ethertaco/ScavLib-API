using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using ScavLib.item;
using ScavLib.util;

namespace ScavLib.save.patches
{

    [HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.TryLoadGame))]
    internal static class TryLoadGamePatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {

            try
            {
                RestoreFromCompanion();
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[TryLoadGamePatch] Postfix threw: {ex}");
            }
            finally
            {
                SaveCompanionFile.DeleteIfExists();
            }
        }

        private static void RestoreFromCompanion()
        {
            SaveCompanionData data;
            try
            {
                data = SaveCompanionFile.Read();
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[TryLoadGamePatch] Companion file read failed: {ex}");
                return;
            }

            if (data == null || data.items == null || data.items.Count == 0)
            {
                ScavLibPlugin.Log.LogInfo(
                    "[TryLoadGamePatch] No companion records to restore.");
                return;
            }

            var ordered = data.items
                .OrderBy(r => r.parentLocation == ParentLocation.PlayerHand ||
                              r.parentLocation == ParentLocation.PlayerWearSlot ? 1 : 0)
                .ToList();

            int ok = 0, fail = 0, fallback = 0;
            foreach (var rec in ordered)
            {
                try
                {
                    if (RestoreOne(rec)) ok++;
                    else fallback++;
                }
                catch (Exception ex)
                {
                    fail++;
                    ScavLibPlugin.Log.LogError(
                        $"[TryLoadGamePatch] Restore failed for '{rec?.customItemId}': {ex}");
                }
            }

            ScavLibPlugin.Log.LogInfo(
                $"[TryLoadGamePatch] Restored {ok} item(s), {fallback} placeholder/fallback, " +
                $"{fail} failed (out of {data.items.Count} total).");
        }

        private static bool RestoreOne(SavedCustomItem rec)
        {
            if (rec == null || string.IsNullOrEmpty(rec.customItemId)) return false;

            if (!CustomItemRegistry.Contains(rec.customItemId))
            {
                SpawnMissingPlaceholder(rec);
                return false;
            }

            var pos = new Vector2(rec.worldX, rec.worldY);
            GameObject go;
            try
            {
                go = Utils.Create(rec.customItemId, pos, rec.rotZ);
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[TryLoadGamePatch] Utils.Create('{rec.customItemId}') threw: {ex}");
                return false;
            }

            if (go == null)
            {
                ScavLibPlugin.Log.LogError(
                    $"[TryLoadGamePatch] Utils.Create returned null for '{rec.customItemId}'.");
                return false;
            }

            if (go.TryGetComponent<Item>(out var item))
            {
                item.condition = rec.condition;
                item.favourited = rec.favourited;
            }

            switch (rec.parentLocation)
            {
                case ParentLocation.Container:
                    {
                        var container = SaveGamePatch.ResolveContainerRef(rec.parentContainerRef);
                        if (container != null && go.TryGetComponent<Item>(out var modItem))
                        {

                            modItem.transform.position = container.transform.position;

                            int childBefore = container.transform.childCount;
                            container.LoadItem(modItem);
                            int childAfter = container.transform.childCount;

                            if (childAfter > childBefore &&
                                modItem.transform.parent == container.transform)
                            {
                                ScavLibPlugin.Log.LogInfo(
                                    $"[TryLoadGamePatch] Restored '{rec.customItemId}' into " +
                                    $"container '{rec.parentContainerRef}'.");
                            }
                            else
                            {
                                ScavLibPlugin.Log.LogWarning(
                                    $"[TryLoadGamePatch] LoadItem rejected '{rec.customItemId}' " +
                                    $"into '{rec.parentContainerRef}' " +
                                    $"(CanHoldItem={container.CanHoldItem(modItem)}, " +
                                    $"weight={modItem.totalWeight}, capacity={container.maxWeight}). " +
                                    $"Delivering to player as fallback.");
                                TryDeliverToPlayer(go);
                            }
                        }
                        else
                        {

                            ScavLibPlugin.Log.LogWarning(
                                $"[TryLoadGamePatch] Container '{rec.parentContainerRef}' " +
                                $"not found for '{rec.customItemId}'. Delivering to player.");
                            TryDeliverToPlayer(go);
                        }
                        break;
                    }

                case ParentLocation.PlayerHand:
                case ParentLocation.PlayerWearSlot:
                    {

                        TryDeliverToPlayer(go);
                        break;
                    }

                case ParentLocation.Ground:
                case ParentLocation.Unknown:
                default:

                    break;
            }

            if (!string.IsNullOrEmpty(rec.instanceBlob))
            {
                try
                {
                    foreach (var mb in go.GetComponents<MonoBehaviour>())
                    {
                        if (mb is ICustomItemSaveable s) s.Load(rec.instanceBlob);
                    }
                }
                catch (Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[TryLoadGamePatch] ICustomItemSaveable.Load() threw for " +
                        $"'{rec.customItemId}': {ex}");
                }
            }

            return true;
        }

        private static void TryDeliverToPlayer(GameObject go)
        {
            try
            {
                var body = GameUtil.GetBody();
                if (body == null) return;
                go.transform.position = body.transform.position;

                if (go.TryGetComponent<Item>(out var item))
                    body.AutoPickUpItem(item);
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogWarning(
                    $"[TryLoadGamePatch] TryDeliverToPlayer failed: {ex.Message}");
            }
        }

        private static void SpawnMissingPlaceholder(SavedCustomItem rec)
        {
            try
            {
                var pos = new Vector2(rec.worldX, rec.worldY);
                var go = Utils.Create("scrapmetal", pos, rec.rotZ);
                if (go == null) return;

                if (go.TryGetComponent<SpriteRenderer>(out var sr))
                    sr.color = Color.magenta;

                var missing = go.AddComponent<MissingItemTag>();
                missing.OriginalCustomItemId = rec.customItemId;
                missing.OriginalOwner = rec.owner;
                missing.OriginalRecord = rec;

                ScavLibPlugin.Log.LogWarning(
                    $"[TryLoadGamePatch] Mod item '{rec.customItemId}' " +
                    $"(owner '{rec.owner}') not registered — spawned magenta placeholder. " +
                    $"Reinstall the mod to recover.");
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[TryLoadGamePatch] Failed to spawn missing placeholder: {ex}");
            }
        }
    }
}
