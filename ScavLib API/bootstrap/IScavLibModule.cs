namespace ScavLib.bootstrap
{
    /// <summary>
    /// Contract a ScavLib feature module implements so the core can discover and
    /// initialize it by reflection instead of referencing it directly.
    ///
    /// <para>This is the single one-way edge that lets a module reference the core
    /// (for services and the context type) without the core having to reference the
    /// module back — i.e. it is what prevents a circular assembly reference once the
    /// module is split into its own DLL.</para>
    ///
    /// <para>Kept intentionally tiny: no Shutdown/Update, because BepInEx plugins
    /// have no clean unload point, and lifecycle hooks we cannot honor only invite
    /// fragile code. Implementations must be <c>public</c> so the host can
    /// instantiate them across the DLL boundary.</para>
    /// </summary>
    public interface IScavLibModule
    {
        /// <summary>Short stable name; used as the diagnostics key and in logs.</summary>
        string Name { get; }

        /// <summary>Init order, ascending. Core infrastructure is 0; feature modules sit above it.</summary>
        int Order { get; }

        /// <summary>
        /// Called once during <c>ScavLibPlugin.Awake</c>, after core services are
        /// ready. The host wraps this in try/catch, but a module should still guard
        /// its own internals. Use <paramref name="ctx"/> for Harmony/logging/status
        /// rather than reaching into <see cref="ScavLibPlugin"/> statically.
        /// </summary>
        void Init(ScavLibModuleContext ctx);
    }
}