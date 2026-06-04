using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.util
{

    public static partial class PlayerUtil
    {

        public static GameObject GiveItem(string id)
        {
            return GameUtil.SpawnAtPlayer(id);
        }

        public static bool TakeItem(string id)
        {
            var body = GameUtil.GetBody();
            if (body == null) return false;

            Item found;
            if (!body.FindByIdSurface(id, out found)) return false;

            body.DropItem(found);
            return true;
        }

        public static bool HasItem(string id)
        {
            var body = GameUtil.GetBody();
            if (body == null) return false;

            Item _;
            return body.FindByIdSurface(id, out _);
        }

        public static List<Item> GetAllItems()
        {
            var body = GameUtil.GetBody();
            if (body == null) return new List<Item>();
            return body.GetAllItems();
        }

        public static bool FindItemById(string id, out Item item)
        {
            item = null;
            var body = GameUtil.GetBody();
            if (body == null) return false;
            return body.FindByIdThorough(id, out item);
        }

        public static bool FindItemByTag(string tag, out Item item)
        {
            item = null;
            var body = GameUtil.GetBody();
            if (body == null) return false;
            return body.FindByTagThorough(tag, out item);
        }
    }
}
