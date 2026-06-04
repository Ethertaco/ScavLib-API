using ScavLib.event_bus.events;

namespace ScavLib.mods
{

    public abstract class ModLifecycleBase : IModLifecycle
    {
        public virtual void OnEnabled() { }
        public virtual void OnDisabled() { }
        public virtual void OnWorldLoaded(WorldLoadedEvent e) { }
        public virtual void OnLayerLoaded(LayerLoadedEvent e) { }
        public virtual void OnWorldUnloading(WorldUnloadingEvent e) { }
        public virtual void OnWorldDestroyed(WorldDestroyedEvent e) { }
    }
}
