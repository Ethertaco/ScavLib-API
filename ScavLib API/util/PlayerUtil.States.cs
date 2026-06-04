namespace ScavLib.util
{
    public static partial class PlayerUtil
    {

        public static bool IsAlive()
        {
            var body = GameUtil.GetBody();
            return body != null && body.alive;
        }

        public static bool IsConscious()
        {
            var body = GameUtil.GetBody();
            return body != null && body.conscious;
        }

        public static bool IsDying()
        {
            var body = GameUtil.GetBody();
            return body != null && body.isDying;
        }

        public static bool IsCriticallyDying()
        {
            var body = GameUtil.GetBody();
            return body != null && body.isCriticallyDying;
        }

        public static bool IsInCardiacArrest()
        {
            var body = GameUtil.GetBody();
            return body != null && body.inCardiacArrest;
        }

        public static bool IsSleeping()
        {
            var body = GameUtil.GetBody();
            return body != null && body.sleeping;
        }

        public static bool IsExercising()
        {
            var body = GameUtil.GetBody();
            return body != null && body.exercising;
        }

        public static bool IsBreathing()
        {
            var body = GameUtil.GetBody();
            return body != null && body.breathing;
        }

        public static bool IsInWater()
        {
            var body = GameUtil.GetBody();
            return body != null && body.inWater;
        }

        public static bool HasScubaGear()
        {
            var body = GameUtil.GetBody();
            return body != null && body.hasScubaGear;
        }

        public static bool IsStanding()
        {
            var body = GameUtil.GetBody();
            return body != null && body.standing;
        }

        public static bool IsCrouching()
        {
            var body = GameUtil.GetBody();
            return body != null && body.crouching;
        }

        public static bool IsOnHardStimulants()
        {
            var body = GameUtil.GetBody();
            return body != null && body.onHardStimulants;
        }

        public static bool UsedNeuralBooster()
        {
            var body = GameUtil.GetBody();
            return body != null && body.usedNeuralBooster;
        }

        public static bool IsUsingSleepingBag()
        {
            var body = GameUtil.GetBody();
            return body != null && body.usingSleepingBag;
        }

        public static bool IsBothHandsUnusable()
        {
            var body = GameUtil.GetBody();
            return body != null && body.bothHandsUnusable;
        }

        public static bool IsAboveMedicalCutoff()
        {
            var body = GameUtil.GetBody();
            return body != null && body.aboveMedicalCutoff;
        }

        public static bool CanTakeNap()
        {
            var body = GameUtil.GetBody();
            return body != null && body.canTakeNap;
        }

        public static bool AllowUseItem()
        {
            var body = GameUtil.GetBody();
            return body != null && body.allowUseItem;
        }

        public static bool HasPulmonaryEmbolism()
        {
            var body = GameUtil.GetBody();
            return body != null && body.hasPulmonaryEmbolism;
        }

        public static bool IsFibrillationForced()
        {
            var body = GameUtil.GetBody();
            return body != null && body.fibrillationForced;
        }

        public static bool HasTriedLastStand()
        {
            var body = GameUtil.GetBody();
            return body != null && body.triedRollingLastStand;
        }

        public static bool HasSuccessfullyRolledLastStand()
        {
            var body = GameUtil.GetBody();
            return body != null && body.succesfullyRolledLastStand;
        }
    }
}
