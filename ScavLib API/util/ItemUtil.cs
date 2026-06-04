using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.util
{

    public static class ItemUtil
    {

        public static List<Item> FindNearby(Vector2 center, float radius, bool includeContained = false)
        {
            var result = new List<Item>();
            if (radius <= 0f) return result;

            float sqr = radius * radius;
            var all = Object.FindObjectsOfType<Item>();
            foreach (var item in all)
            {
                if (item == null) continue;
                if (!includeContained && item.ParentContainer() != null) continue;

                float d = ((Vector2)item.transform.position - center).sqrMagnitude;
                if (d <= sqr) result.Add(item);
            }
            return result;
        }

        public static Item FindClosest(Vector2 center, float maxRadius = float.MaxValue, bool includeContained = false)
        {
            Item best = null;
            float bestSqr = maxRadius * maxRadius;

            var all = Object.FindObjectsOfType<Item>();
            foreach (var item in all)
            {
                if (item == null) continue;
                if (!includeContained && item.ParentContainer() != null) continue;

                float d = ((Vector2)item.transform.position - center).sqrMagnitude;
                if (d < bestSqr)
                {
                    bestSqr = d;
                    best = item;
                }
            }
            return best;
        }

        public static void SetCondition(Item item, float condition)
        {
            if (item == null) return;
            item.SetCondition(Mathf.Clamp01(condition));
        }

        public static void Repair(Item item)
        {
            SetCondition(item, 1f);
        }

        public static void SetFavourited(Item item, bool favourited)
        {
            if (item == null) return;
            item.favourited = favourited;
        }

        public static void Destroy(Item item)
        {
            if (item == null) return;

            var parent = item.ParentContainer();
            if (parent != null)
            {

                item.transform.SetParent(null, true);
            }

            Object.Destroy(item.gameObject);
        }

        public static ItemInfo GetInfo(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (Item.GlobalItems == null) return null;
            return Item.GlobalItems.TryGetValue(id, out var info) ? info : null;
        }

        public static bool IsKnownId(string id)
        {
            return GetInfo(id) != null;
        }

        public static IEnumerable<string> GetAllIds()
        {
            if (Item.GlobalItems == null) return System.Linq.Enumerable.Empty<string>();
            return new List<string>(Item.GlobalItems.Keys);
        }
    }
}
