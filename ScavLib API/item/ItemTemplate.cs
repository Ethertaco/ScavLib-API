using System;

namespace ScavLib.item
{
    public enum ItemTemplate
    {
        SimpleItem,
        ScrapMaterial,
        FoodSimple,
        Bandage,
        Tool,
        Cleaver,
        Knife,
        WaterContainer,
        Canteen,
        CraftingBottle,
        Pistol,
        Rifle,
        Shotgun,
        MakeshiftRifle,
        SmallMagazine,
        RifleMagazine,
        Round9mm,
        Round556,
        Gauge12,
        BoxOf12Gauge,
        SmallBattery,
        MediumBattery,
        LargeBattery,
        SmallPack,
        Duffelbag,
        SlingBag,
        BigPack,
        LegPouch,
        FoliageBag,
        Hoodie,
        Helmet,
        BikeHelmet,
        RiotHelmet,
        DustMask,
        Goggles,
        Balaclava,
        Scarf,
        Gloves,
        TacticalGloves,
        ArmWarmers,
        LimbWraps,
        Shirt,
        Sneakers,
        Boots,
        BellyArmor,
        KneePads,
        TorsoArmor,
        Belt,
        Bandolier,
        FannyPack,
        MaterialPouch,
        Flashlight,
        Lantern,
        MakeshiftHeadlamp,
        Explosive,
        Plushie,
        Watch,
        MP3Player,
        GeigerCounter,
        GrapplingHook,
        Blueprint,
        Sleepingbag,
        Plastic,
        Wood,
        ScrapCube,
    }

    public static class ItemTemplateExtensions
    {
        public static string ToResourceId(this ItemTemplate template)
        {
            switch (template)
            {
                case ItemTemplate.SimpleItem: return "geofruit";
                case ItemTemplate.ScrapMaterial: return "scrapmetal";

                case ItemTemplate.FoodSimple: return "nutrientbar";
                case ItemTemplate.Bandage: return "bandage";

                case ItemTemplate.Tool: return "wrench";
                case ItemTemplate.Cleaver: return "machete";
                case ItemTemplate.Knife: return "flimsyknife";

                case ItemTemplate.WaterContainer: return "canteen";
                case ItemTemplate.Canteen: return "canteen";
                case ItemTemplate.CraftingBottle: return "craftingbottle";

                case ItemTemplate.Pistol: return "pistol";
                case ItemTemplate.Rifle: return "rifle";
                case ItemTemplate.Shotgun: return "shotgun";
                case ItemTemplate.MakeshiftRifle: return "makeshiftrifle";

                case ItemTemplate.SmallMagazine: return "smallmagazine";
                case ItemTemplate.RifleMagazine: return "riflemagazine";
                case ItemTemplate.Round9mm: return "9mmround";
                case ItemTemplate.Round556: return "556round";
                case ItemTemplate.Gauge12: return "12gauge";
                case ItemTemplate.BoxOf12Gauge: return "boxof12gauge";

                case ItemTemplate.SmallBattery: return "smallbattery";
                case ItemTemplate.MediumBattery: return "mediumbattery";
                case ItemTemplate.LargeBattery: return "largebattery";

                case ItemTemplate.SmallPack: return "smallpack";
                case ItemTemplate.Duffelbag: return "duffelbag";
                case ItemTemplate.SlingBag: return "slingbag";
                case ItemTemplate.BigPack: return "bigpack";
                case ItemTemplate.LegPouch: return "legpouch";
                case ItemTemplate.FoliageBag: return "foliagebag";

                case ItemTemplate.Hoodie: return "hoodie";
                case ItemTemplate.Helmet: return "bikehelmet";
                case ItemTemplate.BikeHelmet: return "bikehelmet";
                case ItemTemplate.RiotHelmet: return "riothelmet";
                case ItemTemplate.DustMask: return "dustmask";
                case ItemTemplate.Goggles: return "goggles";
                case ItemTemplate.Balaclava: return "balaclava";
                case ItemTemplate.Scarf: return "scarf";
                case ItemTemplate.Gloves: return "latexgloves";
                case ItemTemplate.TacticalGloves: return "tacticalgloves";
                case ItemTemplate.ArmWarmers: return "armwarmers";
                case ItemTemplate.LimbWraps: return "limbwraps";
                case ItemTemplate.Shirt: return "tornshirt";
                case ItemTemplate.Sneakers: return "sneakers";
                case ItemTemplate.Boots: return "sneakers";
                case ItemTemplate.BellyArmor: return "bellyarmor";
                case ItemTemplate.KneePads: return "kneepads";
                case ItemTemplate.TorsoArmor: return "carapace";
                case ItemTemplate.Belt: return "belt";
                case ItemTemplate.Bandolier: return "bandolier";
                case ItemTemplate.FannyPack: return "fannypack";
                case ItemTemplate.MaterialPouch: return "materialpouch";

                case ItemTemplate.Flashlight: return "flashlight";
                case ItemTemplate.Lantern: return "lantern";
                case ItemTemplate.MakeshiftHeadlamp: return "makeshiftheadlamp";

                case ItemTemplate.Explosive: return "dynamite";

                case ItemTemplate.Plushie: return "plushie";
                case ItemTemplate.Watch: return "watch";
                case ItemTemplate.MP3Player: return "mp3player";
                case ItemTemplate.GeigerCounter: return "geigercounter";
                case ItemTemplate.GrapplingHook: return "grapplinghook";
                case ItemTemplate.Blueprint: return "blueprint";
                case ItemTemplate.Sleepingbag: return "sleepingbag";

                case ItemTemplate.Plastic: return "plasticchunk";
                case ItemTemplate.Wood: return "woodcube";
                case ItemTemplate.ScrapCube: return "scrapcube";

                default:
                    throw new ArgumentOutOfRangeException(nameof(template), template,
                        $"ItemTemplate '{template}' has no Resources id mapping.");
            }
        }
    }
}
