using UnityEngine;

namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static void Feed(float amount)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.hunger = Mathf.Clamp(body.hunger + amount, -100f, 100f);
        }

        public static void Hydrate(float amount)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.thirst = Mathf.Clamp(body.thirst + amount, 0f, 200f);
        }

        public static void RestoreStamina(float amount)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.stamina = Mathf.Clamp(body.stamina + amount, 0f, 100f);
        }

        public static void RestoreEnergy(float amount)
        {
            var body = GameUtil.GetBody();
            if (body == null) return;
            body.energy = Mathf.Clamp(body.energy + amount, 0f, 100f);
        }

        public static void HealAll(Body body)
        {

            foreach (Limb limb in body.limbs)
            {
                limb.muscleHealth = 100f;
                limb.skinHealth = 100f;
                limb.boneHealTimer = 0f;
                limb.dislocationTimer = 0f;
                limb.infectionAmount = 0f;
                limb.bleedAmount = 0f;
                limb.pain = 0f;
                limb.shrapnel = 0;
                limb.infected = false;
            }
            body.brainHealth = 100f;
            body.bloodVolume = 100f;
            body.bloodOxygen = 100f;
            body.bloodPressure = 120f;
            body.heartRate = 70f;
            body.bloodVesselSize = 1f;
            body.bloodViscosity = 0f;
            body.respiratoryRate = 100f;
            body.strokeAmount = 0f;
            body.hasPulmonaryEmbolism = false;
            body.fibrillationProgress = 0f;
            body.hunger = 100f;
            body.thirst = 100f;
            body.septicShock = 0f;
            body.temperature = 37f;
            body.sicknessAmount = 0f;
            body.consciousness = 100f;
            body.stamina = 100f;
            body.energy = 100f;
            body.happiness = 0f;
            body.radiationSickness = 0f;
            body.internalBleeding = 0f;
            body.hemothorax = 0f;
            body.traumaAmount = 0f;
            body.dirtyness = 0f;
            body.wetness = 0f;
            body.badSleepAmount = 0f;
            body.hearingLoss = 0f;
            body.antidepressantHappiness = 0f;
            body.opiateHappiness = 0f;
            body.antibioticImmunityTime = 0f;
            body.adrenaline = 0f;
            body.curAdrenaline = 0f;
            body.venomCurrent = 0f;
            body.venomTotal = 0f;
            body.clawHealth = 100f;

            body.focusedLevel = 0f;
            body.horrifiedLevel = 0f;
            body.snowAmount = 0f;

            Painkillers pk;
            if (body.TryGetComponent<Painkillers>(out pk))
                UnityEngine.Object.Destroy(pk);
            SleepingPills sp;
            if (body.TryGetComponent<SleepingPills>(out sp))
                UnityEngine.Object.Destroy(sp);
            Antidepressants ad;
            if (body.TryGetComponent<Antidepressants>(out ad))
                UnityEngine.Object.Destroy(ad);

            body.triedRollingLastStand = false;
            body.succesfullyRolledLastStand = false;

            body.lastStandTime = -1000f;

            CoUtils.instance.CancelAll();

            body.weightOffset = 0f;

            body.disfigured = false;

            body.eyeGone = false;
            body.bothEyesGone = false;

            body.bloodPressureChangeFromMedicine = 0f;

            body.caffeinated = 0f;

            body.brainGrowSickness = 0f;

            body.clawRegrowTime = 0f;
        }
    }
}
