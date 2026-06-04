namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static bool TriedRollingLastStand()
        {
            var body = GameUtil.GetBody();
            return body != null && body.triedRollingLastStand;
        }

        public static bool SuccessfullyRolledLastStand()
        {
            var body = GameUtil.GetBody();
            return body != null && body.succesfullyRolledLastStand;
        }

        public static float GetLastStandTime()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.lastStandTime : 0f;
        }

        public static float GetAntibioticImmunityTime()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.antibioticImmunityTime : 0f;
        }
    }
}
