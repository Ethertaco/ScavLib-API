using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.util
{

    public enum LimbSlot
    {
        Head = 0,
        Thorax = 1,
        Pelvis = 2,
    }

    public static class LimbUtil
    {

        public static Limb GetLimb(int index)
        {
            var body = GameUtil.GetBody();
            if (body == null || body.limbs == null) return null;
            if (index < 0 || index >= body.limbs.Length) return null;
            return body.limbs[index];
        }

        public static Limb GetLimb(LimbSlot slot)
        {
            return GetLimb((int)slot);
        }

        public static Limb GetLimbByName(string name)
        {
            var body = GameUtil.GetBody();
            return body != null ? body.LimbByName(name) : null;
        }

        public static List<Limb> GetAllLimbs()
        {
            var body = GameUtil.GetBody();
            if (body == null || body.limbs == null) return new List<Limb>();
            return new List<Limb>(body.limbs);
        }

        public static bool HasBrokenBone()
        {
            var body = GameUtil.GetBody();
            if (body == null || body.limbs == null) return false;
            foreach (var limb in body.limbs)
                if (limb != null && !limb.dismembered && limb.broken) return true;
            return false;
        }

        public static bool HasDislocation()
        {
            var body = GameUtil.GetBody();
            if (body == null || body.limbs == null) return false;
            foreach (var limb in body.limbs)
                if (limb != null && !limb.dismembered && limb.dislocated) return true;
            return false;
        }

        public static bool HasInfection()
        {
            var body = GameUtil.GetBody();
            if (body == null || body.limbs == null) return false;
            foreach (var limb in body.limbs)
                if (limb != null && !limb.dismembered && limb.infected) return true;
            return false;
        }

        public static bool HasDismemberment()
        {
            var body = GameUtil.GetBody();
            if (body == null || body.limbs == null) return false;
            foreach (var limb in body.limbs)
                if (limb != null && limb.dismembered) return true;
            return false;
        }

        public static float GetMaxInfection()
        {
            var body = GameUtil.GetBody();
            if (body == null || body.limbs == null) return 0f;
            float max = 0f;
            foreach (var limb in body.limbs)
            {
                if (limb == null || limb.dismembered) continue;
                if (limb.infectionAmount > max) max = limb.infectionAmount;
            }
            return max;
        }

        public static float GetAveragePain()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.averagePain : 0f;
        }

        public static float GetTotalBleedSpeed()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.totalBleedSpeed : 0f;
        }

        public static void HealLimb(Limb limb)
        {
            if (limb == null || limb.dismembered) return;
            limb.skinHealth = 100f;
            limb.muscleHealth = 100f;
            limb.bleedAmount = 0f;
            limb.pain = 0f;
            limb.infectionAmount = 0f;
            limb.infected = false;
            limb.shrapnel = 0;
            if (limb.broken) limb.MendBone();
            if (limb.dislocated) limb.UnDislocate();
        }

        public static void HealLimb(int index) => HealLimb(GetLimb(index));

        public static void DamageSkin(Limb limb, float amount)
        {
            if (limb == null) return;
            limb.skinHealth = Mathf.Clamp(limb.skinHealth - amount, 0f, 100f);
        }

        public static void DamageMuscle(Limb limb, float amount)
        {
            if (limb == null) return;
            limb.muscleHealth = Mathf.Clamp(limb.muscleHealth - amount, 0f, 100f);
        }

        public static void SetSkinHealthRaw(Limb limb, float value)
        {
            if (limb == null) return;
            limb.skinHealth = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetMuscleHealthRaw(Limb limb, float value)
        {
            if (limb == null) return;
            limb.muscleHealth = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetBleedRaw(Limb limb, float value)
        {
            if (limb == null) return;
            limb.bleedAmount = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetPainRaw(Limb limb, float value)
        {
            if (limb == null) return;
            limb.pain = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetInfectionRaw(Limb limb, float value)
        {
            if (limb == null) return;
            limb.infectionAmount = Mathf.Clamp(value, 0f, 100f);
            if (value <= 0f) limb.infected = false;
            else if (value > 0f) limb.infected = true;
        }
    }
}
