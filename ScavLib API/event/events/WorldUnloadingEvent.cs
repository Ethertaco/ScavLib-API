namespace ScavLib.event_bus.events
{

    public class WorldUnloadingEvent : BusEvent
    {

        public int CurrentBiomeDepth { get; }

        public int NextBiomeDepth { get; }

        public bool IsExitToMenu => NextBiomeDepth < 0;

        public WorldUnloadingEvent(int currentBiomeDepth, int nextBiomeDepth)
        {
            CurrentBiomeDepth = currentBiomeDepth;
            NextBiomeDepth = nextBiomeDepth;
        }
    }
}
