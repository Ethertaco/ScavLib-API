using UnityEngine;

namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static bool HasPainkillers()
        {
            var body = GameUtil.GetBody();
            return body != null && body.GetComponent<Painkillers>() != null;
        }

        public static bool HasAntidepressants()
        {
            var body = GameUtil.GetBody();
            return body != null && body.GetComponent<Antidepressants>() != null;
        }

        public static bool HasSleepingPills()
        {
            var body = GameUtil.GetBody();
            return body != null && body.GetComponent<SleepingPills>() != null;
        }

        public static void RemovePainkillers()
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            Painkillers pk;
            if (body.TryGetComponent<Painkillers>(out pk))
                Object.Destroy(pk);
        }

        public static void RemoveAntidepressants()
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            Antidepressants ad;
            if (body.TryGetComponent<Antidepressants>(out ad))
                Object.Destroy(ad);
        }

        public static void RemoveSleepingPills()
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            SleepingPills sp;
            if (body.TryGetComponent<SleepingPills>(out sp))
                Object.Destroy(sp);
        }

        public static float GetOpiateHappiness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.opiateHappiness : 0f;
        }

        public static float GetAntidepressantHappiness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.antidepressantHappiness : 0f;
        }

        public static float GetCaffeinated()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.caffeinated : 0f;
        }
    }
}
