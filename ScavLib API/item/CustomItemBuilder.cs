using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ScavLib.liquid;

namespace ScavLib.item
{

    public class CustomItemBuilder
    {

        private static readonly List<CustomItemBuilder> _pending
            = new List<CustomItemBuilder>();
        private bool _queued;

        private readonly string _id;
        private readonly string _owner;
        private string _templateId;
        private Sprite _sprite;
        private Sprite _liquidFillSprite;

        private readonly Dictionary<string, object> _overrides
            = new Dictionary<string, object>();

        private readonly List<Action<GameObject>> _onSpawnHooks
            = new List<Action<GameObject>>();
        private Action<GameObject> _userOnSpawn;

        private ItemInfo.Use _pendingUseActionAppend;
        private ItemInfo.UseLimb _pendingUseLimbActionAppend;

        private readonly Dictionary<string, string> _names = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _descs = new Dictionary<string, string>();

        private const string DefaultLang = "EN";

        private CustomItemBuilder(string id, string owner, ItemTemplate template)
        {
            _id = id;
            _owner = owner;
            _templateId = template.ToResourceId();
        }

        private CustomItemBuilder(string id, string owner, string templateResourceId)
        {
            _id = id;
            _owner = owner;
            _templateId = templateResourceId;
        }

        public static CustomItemBuilder Create(string id, string owner, ItemTemplate template)
            => new CustomItemBuilder(id, owner, template);

        public static CustomItemBuilder Create(string id, string owner, string templateResourceId)
            => new CustomItemBuilder(id, owner, templateResourceId);

        public static CustomItemBuilder Pistol(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Pistol);
        public static CustomItemBuilder Rifle(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Rifle);
        public static CustomItemBuilder Shotgun(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Shotgun);
        public static CustomItemBuilder Magazine(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.SmallMagazine);
        public static CustomItemBuilder SmallBattery(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.SmallBattery);
        public static CustomItemBuilder MediumBattery(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.MediumBattery);
        public static CustomItemBuilder LargeBattery(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.LargeBattery);
        public static CustomItemBuilder Canteen(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Canteen);
        public static CustomItemBuilder Flashlight(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Flashlight);
        public static CustomItemBuilder Backpack(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.SmallPack);
        public static CustomItemBuilder Explosive(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Explosive);

        public static CustomItemBuilder Helmet(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Helmet);
        public static CustomItemBuilder BikeHelmet(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.BikeHelmet);
        public static CustomItemBuilder RiotHelmet(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.RiotHelmet);
        public static CustomItemBuilder Hoodie(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Hoodie);
        public static CustomItemBuilder Shirt(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Shirt);
        public static CustomItemBuilder TorsoArmor(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.TorsoArmor);
        public static CustomItemBuilder BellyArmor(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.BellyArmor);
        public static CustomItemBuilder Belt(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Belt);
        public static CustomItemBuilder Bandolier(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Bandolier);
        public static CustomItemBuilder FannyPack(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.FannyPack);
        public static CustomItemBuilder LegPouch(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.LegPouch);
        public static CustomItemBuilder MaterialPouch(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.MaterialPouch);
        public static CustomItemBuilder KneePads(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.KneePads);
        public static CustomItemBuilder Sneakers(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Sneakers);
        public static CustomItemBuilder Boots(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Boots);
        public static CustomItemBuilder Gloves(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Gloves);
        public static CustomItemBuilder TacticalGloves(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.TacticalGloves);
        public static CustomItemBuilder ArmWarmers(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.ArmWarmers);
        public static CustomItemBuilder LimbWraps(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.LimbWraps);
        public static CustomItemBuilder DustMask(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.DustMask);
        public static CustomItemBuilder Goggles(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Goggles);
        public static CustomItemBuilder Balaclava(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Balaclava);
        public static CustomItemBuilder Scarf(string id, string owner) => new CustomItemBuilder(id, owner, ItemTemplate.Scarf);

        public static CustomItemBuilder Wearable(string id, string owner,
            ItemTemplate template, VanillaLimb limb, VanillaWearSlot slot)
        {
            var b = new CustomItemBuilder(id, owner, template);
            b._overrides[nameof(ItemInfo.wearable)] = true;
            b._overrides[nameof(ItemInfo.desiredWearLimb)] = limb.ToName();
            b._overrides[nameof(ItemInfo.wearSlotId)] = slot.ToSlotId();
            return b;
        }

        public CustomItemBuilder Template(ItemTemplate template) { _templateId = template.ToResourceId(); return this; }
        public CustomItemBuilder Template(string resourceId) { _templateId = resourceId; return this; }

        public CustomItemBuilder Sprite(Sprite sprite) { _sprite = sprite; return this; }
        public CustomItemBuilder LiquidFillSprite(Sprite fill) { _liquidFillSprite = fill; return this; }

        private CustomItemBuilder Set(string field, object value)
        {
            _overrides[field] = value;
            return this;
        }

        public CustomItemBuilder Category(string category) => Set(nameof(ItemInfo.category), category);
        public CustomItemBuilder Weight(float w) => Set(nameof(ItemInfo.weight), w);
        public CustomItemBuilder Value(int v) => Set(nameof(ItemInfo.value), v);
        public CustomItemBuilder SlotRotation(float r) => Set(nameof(ItemInfo.slotRotation), r);
        public CustomItemBuilder Combineable(bool b = true) => Set(nameof(ItemInfo.combineable), b);
        public CustomItemBuilder DestroyAtZeroCondition(bool b = true) => Set(nameof(ItemInfo.destroyAtZeroCondition), b);
        public CustomItemBuilder ScaleWeightWithCondition(bool b = true) => Set(nameof(ItemInfo.scaleWeightWithCondition), b);
        public CustomItemBuilder DecayMinutes(float m) => Set(nameof(ItemInfo.decayMinutes), m);
        public CustomItemBuilder DecayInfo(ItemInfo.DecayType flags) => Set(nameof(ItemInfo.decayInfo), (byte)flags);
        public CustomItemBuilder DecayInfo(byte flags) => Set(nameof(ItemInfo.decayInfo), flags);
        public CustomItemBuilder RotSpeed(float speed) => Set(nameof(ItemInfo.rotSpeed), speed);
        public CustomItemBuilder OnlyHoldInHands(bool b = true) => Set(nameof(ItemInfo.onlyHoldInHands), b);
        public CustomItemBuilder AutoAttack(bool b = true) => Set(nameof(ItemInfo.autoAttack), b);
        public CustomItemBuilder UsableWithLMB(bool b = true) => Set(nameof(ItemInfo.usableWithLMB), b);
        public CustomItemBuilder IgnoreDepression(bool b = true) => Set(nameof(ItemInfo.ignoreDepression), b);
        public CustomItemBuilder Tags(params string[] tags) => Set(nameof(ItemInfo.tags), tags == null ? "" : string.Join(",", tags));
        public CustomItemBuilder Recognition(int level) => Set(nameof(ItemInfo.rec), new Recognition(level));

        public CustomItemBuilder Quality(string id, float amount = 1f)
        {

            List<CraftingQuality> list;
            if (_overrides.TryGetValue(nameof(ItemInfo.qualities), out var existing)
                && existing is List<CraftingQuality> el)
            {
                list = el;
            }
            else
            {
                list = new List<CraftingQuality>();
                _overrides[nameof(ItemInfo.qualities)] = list;
            }
            list.Add(new CraftingQuality(id, amount));
            return this;
        }

        public CustomItemBuilder Usable(ItemInfo.Use action, bool replace = true)
        {
            _overrides[nameof(ItemInfo.usable)] = true;
            if (replace)
            {
                _overrides[nameof(ItemInfo.useAction)] = action;
            }
            else
            {

                _pendingUseActionAppend = action;
            }
            return this;
        }

        public CustomItemBuilder UsableOnLimb(ItemInfo.UseLimb action, bool replace = true)
        {
            _overrides[nameof(ItemInfo.usableOnLimb)] = true;
            if (replace)
            {
                _overrides[nameof(ItemInfo.useLimbAction)] = action;
            }
            else
            {
                _pendingUseLimbActionAppend = action;
            }
            return this;
        }

        public CustomItemBuilder WearableArmor(float armor) => Set(nameof(ItemInfo.wearableArmor), armor);
        public CustomItemBuilder WearableIsolation(float iso) => Set(nameof(ItemInfo.wearableIsolation), iso);
        public CustomItemBuilder WearableHitDurabilityLossMultiplier(float m) => Set(nameof(ItemInfo.wearableHitDurabilityLossMultiplier), m);
        public CustomItemBuilder WearableVisualOffset(int offset) => Set(nameof(ItemInfo.wearableVisualOffset), offset);
        public CustomItemBuilder WearableCanBeHeld(bool b = true) => Set(nameof(ItemInfo.wearableCanBeHeld), b);
        public CustomItemBuilder JumpHeightMultChange(float change) => Set(nameof(ItemInfo.jumpHeightMultChange), change);

        public CustomItemBuilder WearSlot(VanillaLimb limb, VanillaWearSlot slot)
        {
            _overrides[nameof(ItemInfo.wearable)] = true;
            _overrides[nameof(ItemInfo.desiredWearLimb)] = limb.ToName();
            _overrides[nameof(ItemInfo.wearSlotId)] = slot.ToSlotId();
            return this;
        }

        public CustomItemBuilder Wearable(
            VanillaLimb desiredWearLimb,
            VanillaWearSlot wearSlotId,
            float armor = 0f,
            float isolation = 0f)
        {
            _overrides[nameof(ItemInfo.wearable)] = true;
            _overrides[nameof(ItemInfo.desiredWearLimb)] = desiredWearLimb.ToName();
            _overrides[nameof(ItemInfo.wearSlotId)] = wearSlotId.ToSlotId();
            if (armor != 0f) _overrides[nameof(ItemInfo.wearableArmor)] = armor;
            if (isolation != 0f) _overrides[nameof(ItemInfo.wearableIsolation)] = isolation;
            return this;
        }

        public CustomItemBuilder Capacity(float capacity) => Set(nameof(LiquidItemInfo.capacity), capacity);
        public CustomItemBuilder AutoFill(bool b = true) => Set(nameof(LiquidItemInfo.autoFill), b);

        public CustomItemBuilder DefaultContents(params (string liquidId, float amount)[] contents)
        {
            var list = new List<LiquidStack>();
            if (contents != null)
                foreach (var c in contents)
                    list.Add(new LiquidStack(c.liquidId, c.amount));
            _overrides[nameof(LiquidItemInfo.defaultContents)] = list;
            return this;
        }

        public CustomItemBuilder MaxCharge(float maxCharge) => Set(nameof(BatteryInfo.maxCharge), maxCharge);

        public CustomItemBuilder AmmoType(GunScript.AmmoType ammoType)
        {
            _onSpawnHooks.Add(go => {
                var ammo = go?.GetComponent<AmmoScript>();
                if (ammo != null) ammo.ammoType = ammoType;
                var gun = go?.GetComponent<GunScript>();
                if (gun != null) gun.ammoType = ammoType;
            });
            return this;
        }

        public CustomItemBuilder AmmoMaxRounds(int maxRounds)
        {
            _onSpawnHooks.Add(go => {
                var ammo = go?.GetComponent<AmmoScript>();
                if (ammo != null) ammo.maxRounds = maxRounds;
            });
            return this;
        }

        public CustomItemBuilder GunMagCapacity(int cap)
        {
            _onSpawnHooks.Add(go => { var g = go?.GetComponent<GunScript>(); if (g != null) g.magCapacity = cap; });
            return this;
        }

        public CustomItemBuilder GunDamage(float structureDamage, float animalDamage)
        {
            _onSpawnHooks.Add(go => {
                var g = go?.GetComponent<GunScript>();
                if (g == null) return;
                g.structureDamage = structureDamage;
                g.animalDamage = animalDamage;
            });
            return this;
        }

        public CustomItemBuilder GunShotsPerFire(int shots)
        {
            _onSpawnHooks.Add(go => { var g = go?.GetComponent<GunScript>(); if (g != null) g.shotsPerFire = shots; });
            return this;
        }

        public CustomItemBuilder GunVerticalSpread(float spread)
        {
            _onSpawnHooks.Add(go => { var g = go?.GetComponent<GunScript>(); if (g != null) g.verticalSpread = spread; });
            return this;
        }

        public CustomItemBuilder GunSprites(Sprite normal, Sprite racked, Sprite normalNoMag, Sprite rackedNoMag)
        {
            _onSpawnHooks.Add(go => {
                var g = go?.GetComponent<GunScript>();
                if (g == null) return;
                g.normalSprite = normal;
                g.rackedSprite = racked;
                g.normalSpriteNoMag = normalNoMag;
                g.rackedSpriteNoMag = rackedNoMag;
            });
            return this;
        }

        public CustomItemBuilder ContainerCapacity(float maxWeight, float maxWeightPerItem)
        {
            _onSpawnHooks.Add(go => {
                var c = go?.GetComponent<Container>();
                if (c == null) return;
                c.maxWeight = maxWeight;
                c.maxWeightPerItem = maxWeightPerItem;
            });
            return this;
        }

        public CustomItemBuilder ContainerTagRestriction(params string[] tags)
        {
            _onSpawnHooks.Add(go => { var c = go?.GetComponent<Container>(); if (c != null) c.tagRestriction = tags ?? new string[0]; });
            return this;
        }

        public CustomItemBuilder ContainerItemsVisible(bool visible = true)
        {
            _onSpawnHooks.Add(go => { var c = go?.GetComponent<Container>(); if (c != null) c.itemsVisible = visible; });
            return this;
        }

        public CustomItemBuilder ContainerEncumberance(float multiplier)
        {
            _onSpawnHooks.Add(go => { var c = go?.GetComponent<Container>(); if (c != null) c.encumberanceMult = multiplier; });
            return this;
        }

        public CustomItemBuilder DisplayName(string en) { _names[DefaultLang] = en; return this; }
        public CustomItemBuilder DisplayName(IDictionary<string, string> byLang) { CopyInto(byLang, _names); return this; }
        public CustomItemBuilder Description(string en) { _descs[DefaultLang] = en; return this; }
        public CustomItemBuilder Description(IDictionary<string, string> byLang) { CopyInto(byLang, _descs); return this; }

        public CustomItemBuilder OnSpawn(Action<GameObject> hook) { _userOnSpawn = hook; return this; }

        public bool Register() => Register(out _);

        public bool Register(out string error)
        {
            error = null;

            var vanilla = ResolveVanillaInfo(_templateId);
            if (vanilla == null)
            {

                if (Item.GlobalItems == null && !_queued)
                {
                    _queued = true;
                    _pending.Add(this);
                    ScavLibPlugin.Log.LogInfo(
                        $"[CustomItemBuilder] Deferring registration of '{_id}' " +
                        $"(template '{_templateId}') until Item.SetupItems has run.");
                    return true;
                }

                error = $"Cannot resolve vanilla ItemInfo for template '{_templateId}'. " +
                        $"Either Item.SetupItems hasn't run yet AND the prefab has no " +
                        $"serialized info field, or the template id is wrong. " +
                        $"Defer registration to an OnWorldLoaded/OnEnabled callback.";
                ScavLibPlugin.Log.LogError($"[CustomItemBuilder] {error}");
                return false;
            }

            ItemInfo cloned = CloneItemInfo(vanilla);

            ApplyOverrides(cloned);

            if (_pendingUseActionAppend != null)
            {
                var vanillaUse = cloned.useAction;
                var append = _pendingUseActionAppend;
                cloned.useAction = (b, it) =>
                {
                    try { vanillaUse?.Invoke(b, it); }
                    catch (Exception ex)
                    {
                        ScavLibPlugin.Log.LogError(
                            $"[CustomItemBuilder] vanilla useAction on '{_id}' threw: {ex}");
                    }
                    try { append(b, it); }
                    catch (Exception ex)
                    {
                        ScavLibPlugin.Log.LogError(
                            $"[CustomItemBuilder] appended useAction on '{_id}' threw: {ex}");
                    }
                };
                cloned.usable = true;
            }

            if (_pendingUseLimbActionAppend != null)
            {
                var vanillaLimbUse = cloned.useLimbAction;
                var append = _pendingUseLimbActionAppend;
                cloned.useLimbAction = (l, it) =>
                {
                    try { vanillaLimbUse?.Invoke(l, it); }
                    catch (Exception ex)
                    {
                        ScavLibPlugin.Log.LogError(
                            $"[CustomItemBuilder] vanilla useLimbAction on '{_id}' threw: {ex}");
                    }
                    try { append(l, it); }
                    catch (Exception ex)
                    {
                        ScavLibPlugin.Log.LogError(
                            $"[CustomItemBuilder] appended useLimbAction on '{_id}' threw: {ex}");
                    }
                };
                cloned.usableOnLimb = true;
            }

            cloned.fullName = _names.TryGetValue(DefaultLang, out var en) ? en : _id;
            cloned.description = _descs.TryGetValue(DefaultLang, out var d) ? d : "";

            Action<GameObject> composedOnSpawn = ComposeOnSpawn();

            var item = new CustomItem(_id, _owner, cloned, _sprite, _templateId,
                _liquidFillSprite, composedOnSpawn,
                new Dictionary<string, string>(_names),
                new Dictionary<string, string>(_descs));

            return CustomItemRegistry.TryRegister(item, out error);
        }

        internal static void FlushPending()
        {
            if (_pending.Count == 0) return;

            var snapshot = new List<CustomItemBuilder>(_pending);
            _pending.Clear();

            int ok = 0, fail = 0;
            foreach (var b in snapshot)
            {
                b._queued = false;
                try
                {
                    if (b.Register(out var err)) ok++;
                    else
                    {
                        fail++;
                        ScavLibPlugin.Log.LogError(
                            $"[CustomItemBuilder] Deferred registration of '{b._id}' " +
                            $"failed after SetupItems: {err}");
                    }
                }
                catch (Exception ex)
                {
                    fail++;
                    ScavLibPlugin.Log.LogError(
                        $"[CustomItemBuilder] Deferred registration of '{b._id}' threw: {ex}");
                }
            }

            ScavLibPlugin.Log.LogInfo(
                $"[CustomItemBuilder] FlushPending: {ok} ok, {fail} failed " +
                $"(of {snapshot.Count} deferred).");
        }

        private static ItemInfo ResolveVanillaInfo(string templateId)
        {
            if (string.IsNullOrEmpty(templateId)) return null;

            if (Item.GlobalItems != null
                && Item.GlobalItems.TryGetValue(templateId, out var fromReg)
                && fromReg != null)
                return fromReg;

            try
            {
                var prefab = Resources.Load<GameObject>(templateId);
                if (prefab == null) return null;
                var itemComp = prefab.GetComponent<Item>();
                if (itemComp == null) return null;
                var f = typeof(Item).GetField("info",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return f?.GetValue(itemComp) as ItemInfo;
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogWarning(
                    $"[CustomItemBuilder] Prefab-fallback for '{templateId}' threw: {ex.Message}");
                return null;
            }
        }

        private static ItemInfo CloneItemInfo(ItemInfo src)
        {
            var type = src.GetType();

            ItemInfo dst;
            try
            {
                dst = (ItemInfo)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[CustomItemBuilder] CreateInstance({type.Name}) failed: {ex.Message}. " +
                    $"Falling back to plain ItemInfo — subtype-specific fields will be lost.");
                dst = new ItemInfo();
            }

            foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                try { f.SetValue(dst, f.GetValue(src)); }
                catch { }
            }

            if (dst.qualities != null)
                dst.qualities = new List<CraftingQuality>(dst.qualities);

            if (src is LiquidItemInfo srcLiquid && dst is LiquidItemInfo dstLiquid)
            {
                if (srcLiquid.defaultContents != null)
                    dstLiquid.defaultContents = new List<LiquidStack>(srcLiquid.defaultContents);
            }

            return dst;
        }

        private void ApplyOverrides(ItemInfo target)
        {
            var type = target.GetType();
            foreach (var kv in _overrides)
            {
                var field = type.GetField(kv.Key,
                    BindingFlags.Instance | BindingFlags.Public);

                if (field == null)
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[CustomItemBuilder] Override field '{kv.Key}' not found on " +
                        $"type '{type.Name}' for item '{_id}'. " +
                        $"(Did you call a Liquid-only setter on a non-liquid template, " +
                        $"or a Battery-only setter on a non-battery template?)");
                    continue;
                }

                try
                {
                    field.SetValue(target, kv.Value);
                }
                catch (Exception ex)
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[CustomItemBuilder] Failed to set override '{kv.Key}' on " +
                        $"'{_id}': {ex.Message}");
                }
            }
        }

        private Action<GameObject> ComposeOnSpawn()
        {
            if (_onSpawnHooks.Count == 0 && _userOnSpawn == null) return null;

            var hooks = new List<Action<GameObject>>(_onSpawnHooks);
            var userHook = _userOnSpawn;
            string id = _id;

            return go =>
            {
                for (int i = 0; i < hooks.Count; i++)
                {
                    try { hooks[i]?.Invoke(go); }
                    catch (Exception ex)
                    {
                        ScavLibPlugin.Log.LogError(
                            $"[CustomItemBuilder] Template OnSpawn hook #{i} on '{id}' threw: {ex}");
                    }
                }
                if (userHook != null)
                {
                    try { userHook(go); }
                    catch (Exception ex)
                    {
                        ScavLibPlugin.Log.LogError(
                            $"[CustomItemBuilder] User OnSpawn hook on '{id}' threw: {ex}");
                    }
                }
            };
        }

        private static void CopyInto(IDictionary<string, string> src,
                                     Dictionary<string, string> dst)
        {
            if (src == null) return;
            foreach (var kv in src) dst[kv.Key] = kv.Value;
        }
    }
}
