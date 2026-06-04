namespace ScavLib.event_bus.events
{

    public class WorldDestroyedEvent : BusEvent
    {

        public int LastBiomeDepth { get; }

        public bool WasSaveAndExit { get; }

        public WorldDestroyedEvent(int lastBiomeDepth, bool wasSaveAndExit)
        {
            LastBiomeDepth = lastBiomeDepth;
            WasSaveAndExit = wasSaveAndExit;
        }
    }
}
