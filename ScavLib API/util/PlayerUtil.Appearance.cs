namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static bool IsDisfigured()
        {
            var body = GameUtil.GetBody();
            return body != null && body.disfigured;
        }

        public static bool IsEyeGone()
        {
            var body = GameUtil.GetBody();
            return body != null && body.eyeGone;
        }

        public static bool IsBothEyesGone()
        {
            var body = GameUtil.GetBody();
            return body != null && body.bothEyesGone;
        }

        public static float GetHorrifiedLevel()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.horrifiedLevel : 0f;
        }

        public static bool IsMindWiped()
        {
            var body = GameUtil.GetBody();
            return body != null && body.mindWipe != null;
        }

        public static float GetClawHealth()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.clawHealth : 0f;
        }

        public static float GetWeightOffset()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.weightOffset : 0f;
        }
    }
}
