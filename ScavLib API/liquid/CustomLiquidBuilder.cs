using System.Collections.Generic;

namespace ScavLib.liquid
{

    public class CustomLiquidBuilder
    {
        private readonly string _id;
        private readonly string _owner;
        private readonly LiquidType _type = new LiquidType();
        private bool _danger;

        private CustomLiquidBuilder(string id, string owner)
        {
            _id = id;
            _owner = owner;
            _type.localeName = id;
            _type.color = UnityEngine.Color.white;
            _type.valuePerLiter = 1f;
        }

        public static CustomLiquidBuilder Create(string id, string owner)
            => new CustomLiquidBuilder(id, owner);

        public CustomLiquidBuilder LocaleName(string n) { _type.localeName = n; return this; }

        public CustomLiquidBuilder Tint(UnityEngine.Color c) { _type.color = c; return this; }

        public CustomLiquidBuilder ValuePerLiter(float v) { _type.valuePerLiter = v; return this; }
        public CustomLiquidBuilder InjectionSickness(float s) { _type.injectionSickness = s; return this; }
        public CustomLiquidBuilder Injectable(bool b = true) { _type.injectable = b; return this; }
        public CustomLiquidBuilder HealthUsable(bool b = true) { _type.healthUsable = b; return this; }
        public CustomLiquidBuilder LocaleFromItem(bool b = true) { _type.localeFromItem = b; return this; }
        public CustomLiquidBuilder MarkDangerous(bool b = true) { _danger = b; return this; }

        public CustomLiquidBuilder OnDrink(LiquidType.OnDrink action) { _type.onDrink = action; return this; }
        public CustomLiquidBuilder OnHealthUse(LiquidType.OnHealthUse action) { _type.onHealthUse = action; return this; }

        public CustomLiquidBuilder Quality(string id, float amount = 1f)
        {
            if (_type.qualities == null) _type.qualities = new List<CraftingQuality>();
            _type.qualities.Add(new CraftingQuality(id, amount));
            return this;
        }

        public bool Register() => Register(out _);

        public bool Register(out string error)
        {
            if (!CustomLiquidRegistry.TryRegister(_id, _type, _owner, out error))
                return false;

            if (_danger && Liquids.DangerList != null && !Liquids.DangerList.Contains(_id))
                Liquids.DangerList.Add(_id);

            return true;
        }
    }
}
