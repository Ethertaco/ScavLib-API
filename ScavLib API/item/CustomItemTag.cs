using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.item
{

    public class CustomItemTag : MonoBehaviour
    {

        public string CustomItemId;

        public string Owner;

        public readonly Dictionary<string, object> InstanceData
            = new Dictionary<string, object>();

        public bool TryGet<T>(string key, out T value)
        {
            value = default;
            if (InstanceData.TryGetValue(key, out var raw) && raw is T t)
            {
                value = t;
                return true;
            }
            return false;
        }

        public void Set(string key, object value) => InstanceData[key] = value;
    }
}
