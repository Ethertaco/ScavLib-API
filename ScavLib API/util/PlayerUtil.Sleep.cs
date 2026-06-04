namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static float GetBadSleepAmount()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.badSleepAmount : 0f;
        }

        public static float GetGoodSleepTime()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.goodSleepTime : 0f;
        }
    }
}
