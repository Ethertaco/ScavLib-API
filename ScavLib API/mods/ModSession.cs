namespace ScavLib.mods
{

    public class ModSession
    {

        public ModInfo Info { get; }

        public IModLifecycle Lifecycle { get; }

        public bool IsEnabled { get; internal set; } = true;

        internal object LifecycleAdapter { get; set; }

        internal ModSession(ModInfo info, IModLifecycle lifecycle)
        {
            Info = info;
            Lifecycle = lifecycle;
        }
    }
}
