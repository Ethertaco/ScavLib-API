using UnityEngine;

namespace ScavLib.util
{

    public enum SkillType
    {

        Strength = 0,

        Resilience = 1,

        Intelligence = 2,
    }

    public static class SkillUtil
    {

        private static Skills GetSkills()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.skills : null;
        }

        public static int GetLevel(SkillType skill)
        {
            var skills = GetSkills();
            if (skills == null) return 0;

            switch (skill)
            {
                case SkillType.Strength: return skills.STR;
                case SkillType.Resilience: return skills.RES;
                case SkillType.Intelligence: return skills.INT;
                default: return 0;
            }
        }

        public static float GetExperience(SkillType skill)
        {
            var skills = GetSkills();
            if (skills == null) return 0f;

            switch (skill)
            {
                case SkillType.Strength: return skills.expSTR;
                case SkillType.Resilience: return skills.expRES;
                case SkillType.Intelligence: return skills.expINT;
                default: return 0f;
            }
        }

        public static float GetProgress(SkillType skill)
        {
            var skills = GetSkills();
            if (skills == null) return 0f;

            switch (skill)
            {
                case SkillType.Strength:
                    return skills.ToNextNormalized(skills.expSTR, skills.minSTR, skills.maxSTR);
                case SkillType.Resilience:
                    return skills.ToNextNormalized(skills.expRES, skills.minRES, skills.maxRES);
                case SkillType.Intelligence:
                    return skills.ToNextNormalized(skills.expINT, skills.minINT, skills.maxINT);
                default:
                    return 0f;
            }
        }

        public static float GetExperienceInLevel(SkillType skill)
        {
            var skills = GetSkills();
            if (skills == null) return 0f;

            switch (skill)
            {
                case SkillType.Strength: return skills.expSTR - skills.minSTR;
                case SkillType.Resilience: return skills.expRES - skills.minRES;
                case SkillType.Intelligence: return skills.expINT - skills.minINT;
                default: return 0f;
            }
        }

        public static float GetExperienceForNextLevel(SkillType skill)
        {
            var skills = GetSkills();
            if (skills == null) return 0f;

            switch (skill)
            {
                case SkillType.Strength: return skills.maxSTR - skills.minSTR;
                case SkillType.Resilience: return skills.maxRES - skills.minRES;
                case SkillType.Intelligence: return skills.maxINT - skills.minINT;
                default: return 0f;
            }
        }

        public static void AddExperience(SkillType skill, float xp)
        {
            var skills = GetSkills();
            if (skills == null) return;
            skills.AddExp((int)skill, xp);
        }

        public static void SetLevelRaw(SkillType skill, int level)
        {
            var skills = GetSkills();
            if (skills == null) return;

            level = Mathf.Max(0, level);

            switch (skill)
            {
                case SkillType.Strength: skills.STR = level; break;
                case SkillType.Resilience: skills.RES = level; break;
                case SkillType.Intelligence: skills.INT = level; break;
            }

            skills.UpdateExpBoundaries();

            switch (skill)
            {
                case SkillType.Strength: skills.expSTR = skills.minSTR; break;
                case SkillType.Resilience: skills.expRES = skills.minRES; break;
                case SkillType.Intelligence: skills.expINT = skills.minINT; break;
            }
        }

        public static float XpMultiplier
        {
            get
            {
                if (WorldGeneration.runSettings == null ||
                    !WorldGeneration.runSettings.ContainsKey("xpgain"))
                    return 1f;
                return Skills.xpGainMult;
            }
            set
            {
                if (WorldGeneration.runSettings == null)
                {
                    ScavLibPlugin.Log.LogWarning(
                        "[SkillUtil] Cannot set XpMultiplier: run settings not " +
                        "initialized yet (no world loaded).");
                    return;
                }
                WorldGeneration.runSettings["xpgain"] = Mathf.Max(0f, value);
            }
        }

        public static int GetExperienceForLevel(int targetLevel)
        {
            return Skills.GetExperienceForLevel(targetLevel);
        }
    }
}
