using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.item
{

    public sealed class CustomItem
    {
        public string Id { get; }
        public string Owner { get; }
        public ItemInfo Info { get; }
        public Sprite Sprite { get; }
        public string TemplateId { get; }
        public Sprite LiquidFillSprite { get; }
        public Action<GameObject> OnSpawn { get; }
        public IReadOnlyDictionary<string, string> DisplayNames { get; }
        public IReadOnlyDictionary<string, string> Descriptions { get; }

        internal CustomItem(
            string id, string owner, ItemInfo info, Sprite sprite,
            string templateId, Sprite liquidFillSprite,
            Action<GameObject> onSpawn,
            Dictionary<string, string> displayNames,
            Dictionary<string, string> descriptions)
        {
            Id = id;
            Owner = owner;
            Info = info;
            Sprite = sprite;
            TemplateId = templateId;
            LiquidFillSprite = liquidFillSprite;
            OnSpawn = onSpawn;
            DisplayNames = displayNames ?? new Dictionary<string, string>();
            Descriptions = descriptions ?? new Dictionary<string, string>();
        }

        public override string ToString()
            => $"{Id} (owner: {Owner ?? "<none>"}, template: {TemplateId})";
    }
}
