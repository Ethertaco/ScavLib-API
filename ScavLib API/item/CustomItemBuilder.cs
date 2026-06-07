using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.item
{

    public class CustomItemBuilder
    {
        private readonly string _id;
        private readonly string _owner;
        private readonly ItemInfo _info;
        private readonly LiquidItemInfo _liquidInfo;

        private Sprite _sprite;
        private string _templateId;
        private Sprite _liquidFillSprite;
        private Action<GameObject> _onSpawn;

        private readonly Dictionary<string, string> _names = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _descs = new Dictionary<string, string>();

        private const string DefaultLang = "EN";

        private CustomItemBuilder(string id, string owner, bool liquid)
        {
            _id = id;
            _owner = owner;

            if (liquid)
            {
                _liquidInfo = new LiquidItemInfo();

                _liquidInfo.defaultContents = new List<LiquidStack>();
                _info = _liquidInfo;
                _templateId = ItemTemplate.WaterContainer.ToResourceId();
            }
            else
            {
                _info = new ItemInfo();
                _templateId = ItemTemplate.SimpleItem.ToResourceId();
            }

            _info.category = "custom";
            _info.slotRotation = 0f;
            _info.weight = 1f;
            _info.value = 1;
            _info.tags = "";
            _info.rec = new Recognition(0);
            _info.destroyAtZeroCondition = false;
            _info.combineable = false;
        }

        public static CustomItemBuilder Create(string id, string owner)
            => new CustomItemBuilder(id, owner, liquid: false);

        public static CustomItemBuilder CreateLiquid(string id, string owner)
            => new CustomItemBuilder(id, owner, liquid: true);

        public CustomItemBuilder Sprite(Sprite sprite) { _sprite = sprite; return this; }

        public CustomItemBuilder Template(ItemTemplate template)
        { _templateId = template.ToResourceId(); return this; }

        public CustomItemBuilder Template(string resourceId)
        { _templateId = resourceId; return this; }

        public CustomItemBuilder LiquidFillSprite(Sprite fill)
        { _liquidFillSprite = fill; return this; }

        public CustomItemBuilder Category(string category) { _info.category = category; return this; }
        public CustomItemBuilder Weight(float w) { _info.weight = w; return this; }
        public CustomItemBuilder Value(int v) { _info.value = v; return this; }
        public CustomItemBuilder SlotRotation(float r) { _info.slotRotation = r; return this; }
        public CustomItemBuilder Recognition(int level) { _info.rec = new Recognition(level); return this; }
        public CustomItemBuilder Combineable(bool b = true) { _info.combineable = b; return this; }
        public CustomItemBuilder DestroyAtZeroCondition(bool b = true) { _info.destroyAtZeroCondition = b; return this; }
        public CustomItemBuilder ScaleWeightWithCondition(bool b = true) { _info.scaleWeightWithCondition = b; return this; }
        public CustomItemBuilder DecayMinutes(float m) { _info.decayMinutes = m; return this; }

        public CustomItemBuilder Tags(params string[] tags)
        {
            _info.tags = (tags == null) ? "" : string.Join(",", tags);
            return this;
        }

        public CustomItemBuilder Quality(string id, float amount = 1f)
        {

            if (_info.qualities == null) _info.qualities = new List<CraftingQuality>();
            _info.qualities.Add(new CraftingQuality(id, amount));
            return this;
        }

        public CustomItemBuilder DecayInfo(ItemInfo.DecayType flags)
        {
            _info.decayInfo = (byte)flags;
            return this;
        }

        public CustomItemBuilder DecayInfo(byte flags)
        {
            _info.decayInfo = flags;
            return this;
        }

        public CustomItemBuilder RotSpeed(float rotPerSecond)
        {
            _info.rotSpeed = rotPerSecond;
            return this;
        }

        public CustomItemBuilder IgnoreDepression(bool b = true)
        {
            _info.ignoreDepression = b;
            return this;
        }

        public CustomItemBuilder OnlyHoldInHands(bool b = true)
        {
            _info.onlyHoldInHands = b;
            return this;
        }

        public CustomItemBuilder AutoAttack(bool b = true)
        {
            _info.autoAttack = b;
            return this;
        }

        public CustomItemBuilder UsableWithLMB(bool b = true)
        {
            _info.usableWithLMB = b;
            return this;
        }

        public CustomItemBuilder Usable(ItemInfo.Use action)
        {
            _info.usable = true;
            _info.useAction = action;
            return this;
        }

        public CustomItemBuilder UsableOnLimb(ItemInfo.UseLimb action)
        {
            _info.usableOnLimb = true;
            _info.useLimbAction = action;
            return this;
        }

        public CustomItemBuilder Wearable(string desiredWearLimb, string wearSlotId,
                                          float armor = 0f, float isolation = 0f)
        {
            _info.wearable = true;
            _info.desiredWearLimb = desiredWearLimb;
            _info.wearSlotId = wearSlotId;
            _info.wearableArmor = armor;
            _info.wearableIsolation = isolation;
            return this;
        }

        public CustomItemBuilder WearableCanBeHeld(bool b = true)
        {
            _info.wearableCanBeHeld = b;
            return this;
        }

        public CustomItemBuilder WearableHitDurabilityLossMultiplier(float mult)
        {
            _info.wearableHitDurabilityLossMultiplier = mult;
            return this;
        }

        public CustomItemBuilder JumpHeightMultChange(float change)
        {
            _info.jumpHeightMultChange = change;
            return this;
        }

        public CustomItemBuilder WearableVisualOffset(int offset)
        {
            _info.wearableVisualOffset = offset;
            return this;
        }

        public CustomItemBuilder Capacity(float capacity)
        {
            RequireLiquid(nameof(Capacity));
            if (_liquidInfo != null) _liquidInfo.capacity = capacity;
            return this;
        }

        public CustomItemBuilder AutoFill(bool b = true)
        {
            RequireLiquid(nameof(AutoFill));
            if (_liquidInfo != null) _liquidInfo.autoFill = b;
            return this;
        }

        public CustomItemBuilder DefaultContents(params (string liquidId, float amount)[] contents)
        {
            RequireLiquid(nameof(DefaultContents));
            if (_liquidInfo == null) return this;
            var list = new List<LiquidStack>();
            if (contents != null)
                foreach (var c in contents)
                    list.Add(new LiquidStack(c.liquidId, c.amount));
            _liquidInfo.defaultContents = list;
            return this;
        }

        public CustomItemBuilder DisplayName(string en)
        { _names[DefaultLang] = en; return this; }

        public CustomItemBuilder DisplayName(IDictionary<string, string> byLang)
        { CopyInto(byLang, _names); return this; }

        public CustomItemBuilder Description(string en)
        { _descs[DefaultLang] = en; return this; }

        public CustomItemBuilder Description(IDictionary<string, string> byLang)
        { CopyInto(byLang, _descs); return this; }

        public CustomItemBuilder OnSpawn(Action<GameObject> hook)
        { _onSpawn = hook; return this; }

        public bool Register() => Register(out _);

        public bool Register(out string error)
        {
            _info.fullName = _names.TryGetValue(DefaultLang, out var en) ? en : _id;
            _info.description = _descs.TryGetValue(DefaultLang, out var d) ? d : "";

            var item = new CustomItem(
                _id, _owner, _info, _sprite, _templateId,
                _liquidFillSprite, _onSpawn,
                new Dictionary<string, string>(_names),
                new Dictionary<string, string>(_descs));

            return CustomItemRegistry.TryRegister(item, out error);
        }

        private void RequireLiquid(string member)
        {
            if (_liquidInfo == null)
                ScavLibPlugin.Log.LogWarning(
                    $"[CustomItemBuilder] '{member}' called on a non-liquid item " +
                    $"'{_id}'. Use CustomItemBuilder.CreateLiquid(...) instead. Ignored.");
        }

        private static void CopyInto(IDictionary<string, string> src, Dictionary<string, string> dst)
        {
            if (src == null) return;
            foreach (var kv in src) dst[kv.Key] = kv.Value;
        }
    }
}
