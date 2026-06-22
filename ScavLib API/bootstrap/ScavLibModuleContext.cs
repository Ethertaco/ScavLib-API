using BepInEx.Logging;
using HarmonyLib;

namespace ScavLib.bootstrap
{
    /// <summary>
    /// Dependency bundle handed to a ScavLib feature module when the core
    /// initializes it.
    ///
    /// <para>Passing these in explicitly — instead of letting a module reach for
    /// <see cref="ScavLibPlugin"/>.Instance / its internal statics — keeps the
    /// coupling one-directional and, crucially, lets a module keep compiling
    /// after it is moved out into its own DLL (it then talks to the core only
    /// through this public surface).</para>
    /// </summary>
    public sealed class ScavLibModuleContext
    {
        /// <summary>The single shared Harmony instance; modules register their own patches through it.</summary>
        public Harmony Harmony { get; internal set; }

        /// <summary>Shared ScavLib log source.</summary>
        public ManualLogSource Log { get; internal set; }

        /// <summary>Core/library version string, for modules that want to gate behavior.</summary>
        public string Version { get; internal set; }

        /// <summary>Directory the ScavLib plugin DLL lives in (for locating assets, locales, etc.).</summary>
        public string PluginDir { get; internal set; }

        /// <summary>
        /// Records a patch/host status into the shared diagnostics tables that the
        /// <c>scavlib</c> console command surfaces. Going through the context — rather
        /// than touching <see cref="ScavLibPlugin"/>'s internal dictionaries directly —
        /// is what allows a module to remain compilable across a DLL boundary.
        /// </summary>
        public void ReportStatus(string key, bool ok, string error = null)
        {
            ScavLibPlugin.PatchStatus[key] = ok;
            if (!ok && error != null)
                ScavLibPlugin.PatchErrors[key] = error;
        }
    }
}