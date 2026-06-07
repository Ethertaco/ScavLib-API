using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using ScavLib.item;

namespace ScavLib.save.patches
{

    [HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.SaveGame))]
    internal static class SaveGamePatch
    {
        private class DetachedEntry
        {
            public CustomItemTag tag;
            public Transform originalParent;
            public Vector3 originalLocalPos;
            public Quaternion originalLocalRot;
            public Vector3 originalLocalScale;
            public bool wasActive;
            public SavedCustomItem record;
        }

        private static List<DetachedEntry> _detached;

        [HarmonyPrefix]
        private static void Prefix()
        {
            _detached = new List<DetachedEntry>();

            try
            {
                var tags = UnityEngine.Object.FindObjectsOfType<CustomItemTag>(true);
                if (tags == null || tags.Length == 0)
                {
                    ScavLibPlugin.Log.LogInfo(
                        "[SaveGamePatch] No CustomItemTag instances found; nothing to detach.");
                    return;
                }

                foreach (var tag in tags)
                {
                    if (tag == null || tag.gameObject == null) continue;

                    var go = tag.gameObject;
                    var entry = new DetachedEntry
                    {
                        tag = tag,
                        originalParent = go.transform.parent,
                        originalLocalPos = go.transform.localPosition,
                        originalLocalRot = go.transform.localRotation,
                        originalLocalScale = go.transform.localScale,
                        wasActive = go.activeSelf,
                        record = BuildRecord(tag),
                    };

                    go.SetActive(false);
                    go.transform.SetParent(null, worldPositionStays: true);

                    _detached.Add(entry);
                }

                ScavLibPlugin.Log.LogInfo(
                    $"[SaveGamePatch] Prefix detached {_detached.Count} custom item(s) from scene.");
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SaveGamePatch] Prefix failed (vanilla save will proceed): {ex}");
            }
        }

        [HarmonyPostfix]
        private static void Postfix()
        {
            if (_detached == null) return;

            int reattached = 0, reattachFailed = 0;
            foreach (var entry in _detached)
            {
                try
                {
                    if (entry?.tag == null || entry.tag.gameObject == null)
                    {
                        reattachFailed++;
                        continue;
                    }

                    var go = entry.tag.gameObject;
                    go.transform.SetParent(entry.originalParent, worldPositionStays: false);
                    go.transform.localPosition = entry.originalLocalPos;
                    go.transform.localRotation = entry.originalLocalRot;
                    go.transform.localScale = entry.originalLocalScale;
                    go.SetActive(entry.wasActive);
                    reattached++;
                }
                catch (Exception ex)
                {
                    reattachFailed++;
                    ScavLibPlugin.Log.LogError(
                        $"[SaveGamePatch] Reattach failed for one item: {ex}");
                }
            }

            ScavLibPlugin.Log.LogInfo(
                $"[SaveGamePatch] Postfix reattached {reattached} item(s) " +
                $"({reattachFailed} failed).");

            try
            {
                var data = new SaveCompanionData();
                foreach (var entry in _detached)
                    if (entry?.record != null) data.items.Add(entry.record);

                SaveCompanionFile.Write(data);
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SaveGamePatch] Companion file write failed: {ex}");
            }
            finally
            {
                _detached = null;
            }
        }

        private static SavedCustomItem BuildRecord(CustomItemTag tag)
        {
            var go = tag.gameObject;
            var record = new SavedCustomItem
            {
                customItemId = tag.CustomItemId,
                owner = tag.Owner,
                worldX = go.transform.position.x,
                worldY = go.transform.position.y,
                rotZ = go.transform.eulerAngles.z,
            };

            if (go.TryGetComponent<Item>(out var item))
            {
                record.condition = item.condition;
                record.favourited = item.favourited;
            }

            ResolveParent(tag, record);

            try
            {
                ICustomItemSaveable saveable = null;
                foreach (var mb in go.GetComponents<MonoBehaviour>())
                {
                    if (mb is ICustomItemSaveable s) { saveable = s; break; }
                }

                if (saveable != null)
                {
                    var blob = saveable.Save();
                    if (!string.IsNullOrEmpty(blob))
                        record.instanceBlob = blob;
                }
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[SaveGamePatch] ICustomItemSaveable.Save() threw for " +
                    $"'{tag.CustomItemId}': {ex}");
            }

            return record;
        }

        private static void ResolveParent(CustomItemTag tag, SavedCustomItem record)
        {
            var parent = tag.transform.parent;
            if (parent == null)
            {
                record.parentLocation = ParentLocation.Ground;
                return;
            }

            if (parent.TryGetComponent<Container>(out var container))
            {
                record.parentLocation = ParentLocation.Container;
                record.parentContainerRef = BuildContainerRef(container);
                return;
            }

            var body = parent.GetComponentInParent<Body>();
            if (body != null)
            {

                bool isWorn = tag.TryGetComponent<Wearable>(out _);

                if (isWorn && tag.TryGetComponent<Item>(out var modItem))
                {
                    ItemInfo info = null;
                    if (Item.GlobalItems != null)
                        Item.GlobalItems.TryGetValue(modItem.id, out info);

                    if (info != null && !string.IsNullOrEmpty(info.wearSlotId))
                    {
                        record.parentLocation = ParentLocation.PlayerWearSlot;
                        record.parentWearSlotId = info.wearSlotId;
                        return;
                    }
                }

                record.parentLocation = ParentLocation.PlayerHand;
                return;
            }

            record.parentLocation = ParentLocation.Unknown;
        }

        private static string BuildContainerRef(Container container)
        {
            if (container == null) return null;
            if (!container.TryGetComponent<Item>(out var containerItem)) return null;

            var allContainers = UnityEngine.Object.FindObjectsOfType<Container>(true);
            int index = 0;
            foreach (var c in allContainers)
            {
                if (c == container) break;
                if (c.TryGetComponent<Item>(out var ci) && ci.id == containerItem.id)
                    index++;
            }
            return $"{containerItem.id}:{index}";
        }

        internal static Container ResolveContainerRef(string parentRef)
        {
            if (string.IsNullOrEmpty(parentRef)) return null;
            var split = parentRef.Split(':');
            if (split.Length != 2) return null;
            var wantedId = split[0];
            if (!int.TryParse(split[1], out var wantedIndex)) return null;

            var allContainers = UnityEngine.Object.FindObjectsOfType<Container>(true);
            int seen = 0;
            foreach (var c in allContainers)
            {
                if (c == null) continue;
                if (!c.TryGetComponent<Item>(out var ci)) continue;
                if (ci.id != wantedId) continue;
                if (seen == wantedIndex) return c;
                seen++;
            }
            return null;
        }
    }
}
