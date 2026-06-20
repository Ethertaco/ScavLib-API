using HarmonyLib;
using HarmonyLib.Tools;

namespace ScavLib.command
{
    /// <summary>
    /// Postfix on <c>ConsoleScript.Start</c> that drains
    /// <see cref="CommandRegistry.FlushPending"/>.
    ///
    /// <para>Why <c>Start</c> and not <c>Awake</c>: vanilla calls
    /// <c>RegisterAllCommands</c> from <c>Start</c>, gated on
    /// <c>Commands.Count == 0</c>. Running our flush as a Postfix on the same
    /// method guarantees the vanilla list is already populated, so our own
    /// <see cref="CommandRegistry.SearchExact"/>-based duplicate check sees
    /// every native command.</para>
    ///
    /// <para>Why <see cref="Priority.High"/>: <c>WorldLoadedPatch</c> runs at
    /// <see cref="Priority.Normal"/> in the same Postfix slot. Higher priority
    /// here ensures commands are registered before any
    /// <c>WorldLoadedEvent</c> handler can dispatch.</para>
    /// </summary>
    [HarmonyPatch(typeof(ConsoleScript), "Start")]
    internal static class CommandRegistryPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.High)]
        private static void Postfix()
        {
            CommandRegistry.FlushPending();
        }
    }
}
