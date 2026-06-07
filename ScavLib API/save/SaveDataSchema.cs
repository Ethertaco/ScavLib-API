using System.Collections.Generic;

namespace ScavLib.save
{

    public class SaveCompanionData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;

        public List<SavedCustomItem> items = new List<SavedCustomItem>();
        public List<SavedRecipeProgress> recipes = new List<SavedRecipeProgress>();
    }

    public enum ParentLocation
    {

        Ground = 0,

        Container = 1,

        PlayerHand = 2,

        PlayerWearSlot = 3,

        Unknown = 99,
    }

    public class SavedCustomItem
    {

        public string customItemId;
        public string owner;

        public float worldX, worldY;
        public float rotZ;

        public float condition = 1f;
        public bool favourited;

        public ParentLocation parentLocation = ParentLocation.Ground;

        public string parentContainerRef;

        public string parentWearSlotId;

        public string instanceBlob;
    }

    public class SavedRecipeProgress
    {
        public string resultId;
        public bool isRepair;
        public bool hasMadeBefore;
    }
}
