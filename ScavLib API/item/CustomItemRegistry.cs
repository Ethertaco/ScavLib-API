using System.Collections.Generic;

namespace ScavLib.item
{

    public static class CustomItemRegistry
    {
        private static readonly List<(string id, ItemInfo info, bool overwrite)> _pending
            = new List<(string, ItemInfo, bool)>();

        internal static bool ItemsInitialized { get; set; } = false;

        public static void RegisterItem(string id, ItemInfo info, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(id))
            {
                ScavLibPlugin.Log.LogError("[CustomItemRegistry] Cannot register item with null/empty ID.");
                return;
            }
            if (info == null)
            {
                ScavLibPlugin.Log.LogError($"[CustomItemRegistry] Cannot register '{id}' with null ItemInfo.");
                return;
            }

            if (ItemsInitialized)
            {
                Inject(id, info, overwrite);
            }
            else
            {
                _pending.Add((id, info, overwrite));
                ScavLibPlugin.Log.LogInfo($"[CustomItemRegistry] Queued item '{id}' for injection.");
            }
        }

        public static void RegisterSimpleItem(
            string id,
            string category = "custom",
            float weight = 1f,
            int value = 1,
            string tags = "")
        {
            var info = new ItemInfo
            {
                category = category,
                weight = weight,
                value = value,
                tags = tags,
                slotRotation = 0f,
                usable = false,
                usableOnLimb = false,
                destroyAtZeroCondition = false,
                combineable = false,
                rec = new Recognition(0),
            };
            RegisterItem(id, info);
        }

        internal static void FlushPending()
        {
            ItemsInitialized = true;
            foreach (var (id, info, overwrite) in _pending)
            {
                Inject(id, info, overwrite);
            }
            _pending.Clear();
        }

        private static void Inject(string id, ItemInfo info, bool overwrite)
        {
            if (Item.GlobalItems == null)
            {
                ScavLibPlugin.Log.LogError(
                    $"[CustomItemRegistry] Cannot inject '{id}' — Item.GlobalItems is null.");
                return;
            }

            if (Item.GlobalItems.ContainsKey(id))
            {
                if (overwrite)
                {
                    Item.GlobalItems[id] = info;
                    ScavLibPlugin.Log.LogInfo($"[CustomItemRegistry] Overwrote item '{id}'.");
                }
                else
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[CustomItemRegistry] Item '{id}' already registered. " +
                        $"Pass overwrite=true to replace it.");
                }
                return;
            }

            Item.GlobalItems.Add(id, info);
            ScavLibPlugin.Log.LogInfo($"[CustomItemRegistry] Registered item '{id}'.");
        }
    }
}
