using UnityEngine;

namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static void SetHungerRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.hunger = Mathf.Clamp(value, -50f, 125f);
        }

        public static void SetThirstRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.thirst = Mathf.Clamp(value, -50f, 250f);
        }

        public static void SetStaminaRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.stamina = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetEnergyRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.energy = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetBloodVolumeRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.bloodVolume = Mathf.Clamp(value, -100f, 200f);
        }

        public static void SetBloodOxygenRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.bloodOxygen = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetHeartRateRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.heartRate = Mathf.Clamp(value, 0f, 300f);
        }

        public static void SetBloodPressureRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.bloodPressure = Mathf.Clamp(value, 0f, 250f);
        }

        public static void SetRespiratoryRateRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.respiratoryRate = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetTemperatureRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.temperature = Mathf.Clamp(value, 20f, 50f);
        }

        public static void SetConsciousnessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.consciousness = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetBrainHealthRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.brainHealth = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetHappinessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.happiness = Mathf.Clamp(value, -100f, 100f);
        }

        public static void SetBloodViscosityRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.bloodViscosity = Mathf.Clamp(value, -100f, 100f);
        }

        public static void SetBloodVesselSizeRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.bloodVesselSize = Mathf.Clamp(value, 0.85f, 1.15f);
        }

        public static void SetFibrillationProgressRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.fibrillationProgress = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetHeartRatePressureOffsetRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.heartRatePressureOffset = Mathf.Clamp(value, -30f, 80f);
        }

        public static void SetAdrenalineRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.adrenaline = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetCurAdrenalineRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.curAdrenaline = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetSepticShockRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.septicShock = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetSicknessAmountRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.sicknessAmount = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetVenomTotalRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.venomTotal = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetVenomCurrentRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.venomCurrent = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetInternalBleedingRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.internalBleeding = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetHemothoraxRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.hemothorax = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetShockRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.shock = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetPainShockRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.painShock = Mathf.Clamp01(value);
        }

        public static void SetTraumaAmountRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.traumaAmount = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetRadiationSicknessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.radiationSickness = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetStrokeAmountRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.strokeAmount = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetFocusedLevelRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.focusedLevel = Mathf.Clamp(value, 0f, 200f);
        }

        public static void SetHorrifiedLevelRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.horrifiedLevel = Mathf.Clamp(value, 0f, 200f);
        }

        public static void SetBrainGrowSicknessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.brainGrowSickness = Mathf.Max(0f, value);
        }

        public static void SetWetnessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.wetness = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetDirtynessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.dirtyness = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetSnowAmountRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.snowAmount = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetHearingLossRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.hearingLoss = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetClawRegrowTimeRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.clawRegrowTime = Mathf.Max(0f, value);
        }

        public static void SetClawHealthRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.clawHealth = Mathf.Clamp(value, 0f, 100f);
        }

        public static void SetImmunityRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.immunity = Mathf.Clamp(value, 0f, 200f);
        }

        public static void SetAntibioticImmunityTimeRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.antibioticImmunityTime = Mathf.Max(0f, value);
        }

        public static void SetCaffeinatedRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.caffeinated = Mathf.Max(0f, value);
        }

        public static void SetWeightOffsetRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.weightOffset = Mathf.Clamp(value, -80f, 100f);
        }

        public static void SetBadSleepAmountRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.badSleepAmount = Mathf.Max(0f, value);
        }

        public static void SetLastStandTimeRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.lastStandTime = Mathf.Clamp(value, -10000f, 300f);
        }

        public static void SetGoodSleepTimeRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.goodSleepTime = value;
        }

        public static void SetOpiateHappinessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.opiateHappiness = value;
        }

        public static void SetAntidepressantHappinessRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.antidepressantHappiness = value;
        }

        public static void SetBloodPressureChangeFromMedicineRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.bloodPressureChangeFromMedicine = value;
        }

        public static void SetDesensitizedMultRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.desensitizedMult = Mathf.Clamp01(value);
        }

        public static void SetCorpsesSeenRaw(int value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.corpsesSeen = Mathf.Max(0, value);
        }

        public static void SetSleepingRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.sleeping = value;
        }

        public static void SetDisfiguredRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.disfigured = value;
        }

        public static void SetEyeGoneRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.eyeGone = value;
        }

        public static void SetBothEyesGoneRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.bothEyesGone = value;
        }

        public static void SetFibrillationForcedRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.fibrillationForced = value;
        }

        public static void SetHasPulmonaryEmbolismRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.hasPulmonaryEmbolism = value;
        }

        public static void SetUsedNeuralBoosterRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.usedNeuralBooster = value;
        }

        public static void SetTriedRollingLastStandRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.triedRollingLastStand = value;
        }

        public static void SetSuccessfullyRolledLastStandRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.succesfullyRolledLastStand = value;
        }

        public static void SetUsingSleepingBagRaw(bool value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.usingSleepingBag = value;
        }

        public static void SetHappinessBaseRaw(float value)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.happiness = Mathf.Clamp(value, -100f, 100f);
        }

    }
}
