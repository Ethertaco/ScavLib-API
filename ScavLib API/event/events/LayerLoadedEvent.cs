namespace ScavLib.event_bus.events
{

    public class LayerLoadedEvent : BusEvent
    {

        public int BiomeDepth { get; }

        public bool IsFirstLoad { get; }

        public LayerLoadedEvent(int biomeDepth, bool isFirstLoad)
        {
            BiomeDepth = biomeDepth;
            IsFirstLoad = isFirstLoad;
        }
    }
}
