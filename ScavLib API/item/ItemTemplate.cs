namespace ScavLib.item
{

    public enum ItemTemplate
    {

        SimpleItem,

        Bandage,

        Wearable,

        WaterContainer,

        Canteen,

        Pistol,

        Magazine,
    }

    internal static class ItemTemplateExtensions
    {

        internal static string ToResourceId(this ItemTemplate t)
        {
            switch (t)
            {
                case ItemTemplate.Bandage: return "bandage";
                case ItemTemplate.Wearable: return "limbwraps";
                case ItemTemplate.WaterContainer: return "canteen";
                case ItemTemplate.Canteen: return "canteen";
                case ItemTemplate.Pistol: return "makeshiftrifle";
                case ItemTemplate.Magazine: return "smallmagazine";
                case ItemTemplate.SimpleItem:
                default:
                    return "scrapmetal";
            }
        }
    }
}
