namespace ScavLib.item
{

    public enum VanillaLimb
    {
        Head, UpTorso, DownTorso,
        UpArmF, DownArmF, HandF,
        UpArmB, DownArmB, HandB,
        ThighF, CrusF, FootF,
        ThighB, CrusB, FootB,
    }

    public enum VanillaWearSlot
    {
        Hat, Eyes, Mouth, Blindfold, Balaclava, Neck,
        OuterTorso, Torso, TorsoFront, Back, Bandolier, Belt,
        Arms, Wraps, Hands,
        Thigh, ThighBack, Knees, Feet,
    }

    public static class VanillaSlotsExtensions
    {
        public static string ToName(this VanillaLimb limb)
        {
            switch (limb)
            {
                case VanillaLimb.Head: return "Head";
                case VanillaLimb.UpTorso: return "UpTorso";
                case VanillaLimb.DownTorso: return "DownTorso";
                case VanillaLimb.UpArmF: return "UpArmF";
                case VanillaLimb.DownArmF: return "DownArmF";
                case VanillaLimb.HandF: return "HandF";
                case VanillaLimb.UpArmB: return "UpArmB";
                case VanillaLimb.DownArmB: return "DownArmB";
                case VanillaLimb.HandB: return "HandB";
                case VanillaLimb.ThighF: return "ThighF";
                case VanillaLimb.CrusF: return "CrusF";
                case VanillaLimb.FootF: return "FootF";
                case VanillaLimb.ThighB: return "ThighB";
                case VanillaLimb.CrusB: return "CrusB";
                case VanillaLimb.FootB: return "FootB";
                default: return limb.ToString();
            }
        }

        public static string ToSlotId(this VanillaWearSlot slot)
        {
            switch (slot)
            {
                case VanillaWearSlot.Hat: return "hat";
                case VanillaWearSlot.Eyes: return "eyes";
                case VanillaWearSlot.Mouth: return "mouth";
                case VanillaWearSlot.Blindfold: return "blindfold";
                case VanillaWearSlot.Balaclava: return "balaclava";
                case VanillaWearSlot.Neck: return "neck";
                case VanillaWearSlot.OuterTorso: return "outertorso";
                case VanillaWearSlot.Torso: return "torso";
                case VanillaWearSlot.TorsoFront: return "torsofront";
                case VanillaWearSlot.Back: return "back";
                case VanillaWearSlot.Bandolier: return "bandolier";
                case VanillaWearSlot.Belt: return "belt";
                case VanillaWearSlot.Arms: return "arms";
                case VanillaWearSlot.Wraps: return "wraps";
                case VanillaWearSlot.Hands: return "hands";
                case VanillaWearSlot.Thigh: return "thigh";
                case VanillaWearSlot.ThighBack: return "thighback";
                case VanillaWearSlot.Knees: return "knees";
                case VanillaWearSlot.Feet: return "feet";
                default: return slot.ToString().ToLowerInvariant();
            }
        }
    }
}
