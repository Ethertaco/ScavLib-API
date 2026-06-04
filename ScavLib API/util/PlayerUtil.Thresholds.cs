namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static class Thresholds
        {

            public const float BLOOD_PRESSURE_CRITICAL_LOW = 60f;
            public const float BLOOD_PRESSURE_LOW_3 = 83f;
            public const float BLOOD_PRESSURE_LOW_2 = 96f;
            public const float BLOOD_PRESSURE_LOW_1 = 110f;
            public const float BLOOD_PRESSURE_HIGH_1 = 130f;
            public const float BLOOD_PRESSURE_HIGH_2 = 145f;
            public const float BLOOD_PRESSURE_HIGH_3 = 162f;
            public const float BLOOD_PRESSURE_CRITICAL_HIGH = 180f;

            public const float BLOOD_OXYGEN_CRITICAL = 45f;
            public const float BLOOD_OXYGEN_SEVERE = 60f;
            public const float BLOOD_OXYGEN_LOW = 75f;
            public const float BLOOD_OXYGEN_MILD = 90f;

            public const float HEART_RATE_CARDIAC_ARREST = 20f;
            public const float HEART_RATE_BRADYCARDIA_SEVERE = 40f;
            public const float HEART_RATE_BRADYCARDIA_MILD = 60f;
            public const float HEART_RATE_TACHYCARDIA_MILD = 110f;
            public const float HEART_RATE_TACHYCARDIA_SEVERE = 160f;
            public const float HEART_RATE_TACHYCARDIA_CRITICAL = 200f;

            public const float TEMPERATURE_HYPOTHERMIA_CRITICAL = 28.0f;
            public const float TEMPERATURE_HYPOTHERMIA_SEVERE = 32.5f;
            public const float TEMPERATURE_HYPOTHERMIA_MILD = 34.0f;
            public const float TEMPERATURE_COLD = 35.5f;
            public const float TEMPERATURE_NORMAL = 37.0f;
            public const float TEMPERATURE_WARM = 38.0f;
            public const float TEMPERATURE_HYPERTHERMIA_MILD = 39.0f;
            public const float TEMPERATURE_HYPERTHERMIA_SEVERE = 40.25f;
            public const float TEMPERATURE_HYPERTHERMIA_CRITICAL = 41.5f;

            public const float BLEED_SPEED_CRITICAL = 0.30f;
            public const float BLEED_SPEED_HEAVY = 0.15f;
            public const float BLEED_SPEED_MEDIUM = 0.06f;

            public const float HUNGER_STARVING = 15f;
            public const float HUNGER_VERY_HUNGRY = 35f;
            public const float HUNGER_HUNGRY = 50f;
            public const float HUNGER_PECKISH = 75f;
            public const float HUNGER_OVERFED_1 = 100f;
            public const float HUNGER_OVERFED_2 = 120f;

            public const float THIRST_CRITICAL = 20f;
            public const float THIRST_VERY_THIRSTY = 35f;
            public const float THIRST_THIRSTY = 55f;
            public const float THIRST_MILDLY_THIRSTY = 75f;
            public const float THIRST_OVERHYDRATED_1 = 100f;
            public const float THIRST_OVERHYDRATED_2 = 125f;
            public const float THIRST_OVERHYDRATED_3 = 175f;

            public const float STAMINA_EXHAUSTED = 15f;
            public const float STAMINA_VERY_TIRED = 35f;
            public const float STAMINA_TIRED = 50f;
            public const float STAMINA_MILDLY_TIRED = 70f;

            public const float ENERGY_EXHAUSTED = 7f;
            public const float ENERGY_VERY_TIRED = 15f;
            public const float ENERGY_TIRED = 25f;
            public const float ENERGY_MILDLY_TIRED = 35f;

            public const float CONSCIOUSNESS_UNCONSCIOUS = 20f;
            public const float CONSCIOUSNESS_INCAPACITATED = 30f;
            public const float CONSCIOUSNESS_CONFUSED_3 = 55f;
            public const float CONSCIOUSNESS_CONFUSED_2 = 72f;
            public const float CONSCIOUSNESS_CONFUSED_1 = 90f;

            public const float PAIN_AGONY = 80f;
            public const float PAIN_SEVERE = 55f;
            public const float PAIN_MODERATE = 30f;
            public const float PAIN_MILD = 10f;

            public const float BRAIN_DAMAGE_SEVERE = 30f;
            public const float BRAIN_DAMAGE_MODERATE = 60f;
            public const float BRAIN_DAMAGE_MILD = 80f;
            public const float BRAIN_DAMAGE_SLIGHT = 95f;

            public const float STROKE_CRITICAL = 70f;
            public const float SEPSIS_CRITICAL = 80f;
            public const float SEPSIS_MODERATE = 50f;
            public const float SEPSIS_MILD = 10f;
            public const float RADIATION_CRITICAL = 80f;
            public const float RADIATION_SEVERE = 50f;
            public const float RADIATION_MODERATE = 30f;
            public const float RADIATION_MILD = 10f;

            public const float INTERNAL_BLEEDING_CRITICAL = 50f;
            public const float HEMOTHORAX_HEAVY = 70f;
            public const float HEMOTHORAX_PRESENT = 40f;

            public const float HAPPINESS_MISERABLE = -75f;
            public const float HAPPINESS_DEPRESSED = -50f;
            public const float HAPPINESS_GLOOMY = -30f;
            public const float HAPPINESS_SAD = -10f;
            public const float HAPPINESS_HAPPY_1 = 10f;
            public const float HAPPINESS_HAPPY_2 = 30f;
            public const float HAPPINESS_HAPPY_3 = 50f;
            public const float HAPPINESS_HAPPY_4 = 75f;

            public const float WEIGHT_UNDERWEIGHT_4 = -50f;
            public const float WEIGHT_UNDERWEIGHT_3 = -40f;
            public const float WEIGHT_UNDERWEIGHT_2 = -30f;
            public const float WEIGHT_UNDERWEIGHT_1 = -15f;
            public const float WEIGHT_OVERWEIGHT_1 = 15f;
            public const float WEIGHT_OVERWEIGHT_2 = 30f;
            public const float WEIGHT_OVERWEIGHT_3 = 40f;
            public const float WEIGHT_OVERWEIGHT_4 = 50f;
        }
    }
}
