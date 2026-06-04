using UnityEngine;

namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static float GetBloodOxygen()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bloodOxygen : 0f;
        }

        public static float GetBloodVolume()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bloodVolume : 0f;
        }

        public static float GetHeartRate()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.heartRate : 0f;
        }

        public static float GetBloodPressure()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bloodPressure : 0f;
        }

        public static float GetRespiratoryRate()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.respiratoryRate : 0f;
        }

        public static float GetTemperature()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.temperature : 0f;
        }

        public static float GetHunger()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.hunger : 0f;
        }

        public static float GetThirst()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.thirst : 0f;
        }

        public static float GetStamina()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.stamina : 0f;
        }

        public static float GetEnergy()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.energy : 0f;
        }

        public static float GetConsciousness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.consciousness : 0f;
        }

        public static float GetBrainHealth()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.brainHealth : 0f;
        }

        public static float GetHappiness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.totalHappiness : 0f;
        }

        public static float GetBloodViscosity()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bloodViscosity : 0f;
        }

        public static float GetBloodVesselSize()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bloodVesselSize : 1f;
        }

        public static float GetFibrillationProgress()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.fibrillationProgress : 0f;
        }

        public static float GetHeartRatePressureOffset()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.heartRatePressureOffset : 0f;
        }

        public static float GetAdrenaline()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.adrenaline : 0f;
        }

        public static float GetCurAdrenaline()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.curAdrenaline : 0f;
        }

        public static float GetSepticShock()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.septicShock : 0f;
        }

        public static float GetSicknessAmount()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.sicknessAmount : 0f;
        }

        public static float GetVenomTotal()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.venomTotal : 0f;
        }

        public static float GetVenomCurrent()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.venomCurrent : 0f;
        }

        public static float GetInternalBleeding()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.internalBleeding : 0f;
        }

        public static float GetHemothorax()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.hemothorax : 0f;
        }

        public static float GetShock()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.shock : 0f;
        }

        public static float GetPainShock()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.painShock : 0f;
        }

        public static float GetTraumaAmount()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.traumaAmount : 0f;
        }

        public static float GetRadiationSickness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.radiationSickness : 0f;
        }

        public static float GetStrokeAmount()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.strokeAmount : 0f;
        }

        public static float GetFocusedLevel()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.focusedLevel : 0f;
        }

        public static float GetBrainGrowSickness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.brainGrowSickness : 0f;
        }

        public static float GetHappinessBase()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.happiness : 0f;
        }

        public static float GetWetness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.wetness : 0f;
        }

        public static float GetDirtyness()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.dirtyness : 0f;
        }

        public static float GetSnowAmount()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.snowAmount : 0f;
        }

        public static float GetHearingLoss()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.hearingLoss : 0f;
        }

        public static float GetClawRegrowTime()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.clawRegrowTime : 0f;
        }

        public static float GetImmunity()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.immunity : 0f;
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

        public static float GetTempDiffFromNormal()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.tempDiffFromNormal : 0f;
        }

        public static float GetBloodVolumePercentage()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bloodVolumePercentage : 0.5f;
        }

        public static bool IsFibrillationRising()
        {
            var body = GameUtil.GetBody();
            return body != null && body.fibrillationRising;
        }

        public static bool IsBrainDying()
        {
            var body = GameUtil.GetBody();
            return body != null && body.brainDying;
        }

        public static float GetCurrentWeightMovementMult()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.currentWeightMovementMult : 1f;
        }

        public static float GetCurrentTemperatureMovementMult()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.currentTemperatureMovementMult : 1f;
        }

        public static float GetBleedClottingSpeed()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bleedClottingSpeed : 0.025f;
        }

        public static float GetBleedingSpeedMultiplier()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bleedingSpeedMultiplier : 0.01f;
        }

        public static float GetThirstBloodPressure()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.thirstBloodPressure : 1f;
        }

        public static float GetHungerLimbHealCurrent()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.hungerLimbHealCurrent : 0f;
        }

        public static float GetCurImmunityMult()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.curImmunityMult : 1f;
        }

        public static float GetDesensitizedMult()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.desensitizedMult : 1f;
        }

        public static int GetCorpsesSeen()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.corpsesSeen : 0;
        }

        public static string GetBloodPressureReadout()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.bloodPressureReadout : "120/80";
        }

        public static string GetRespiratoryRateReadout()
        {
            var body = GameUtil.GetBody();
            return body != null ? body.respiratoryRateReadout : "25/m";
        }
    }
}
