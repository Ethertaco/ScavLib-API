using HarmonyLib;
using HarmonyLib.Tools;

namespace ScavLib.command
{

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
