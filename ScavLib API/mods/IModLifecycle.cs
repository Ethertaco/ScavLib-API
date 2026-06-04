using ScavLib.event_bus.events;

namespace ScavLib.mods
{

    public interface IModLifecycle
    {

        void OnEnabled();

        void OnDisabled();

        void OnWorldLoaded(WorldLoadedEvent e);

        void OnLayerLoaded(LayerLoadedEvent e);

        void OnWorldUnloading(WorldUnloadingEvent e);

        void OnWorldDestroyed(WorldDestroyedEvent e);
    }
}
