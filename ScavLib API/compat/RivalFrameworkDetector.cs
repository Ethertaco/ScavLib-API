using BepInEx.Bootstrap;

namespace ScavLib.compat
{

    internal static class RivalFrameworkDetector
    {

        private static readonly string[] RivalGuids =
        {
            "com.rushellxyz.rshlib",

        };

        internal static void CheckAndWarn()
        {
            foreach (var guid in RivalGuids)
            {
                if (Chainloader.PluginInfos.ContainsKey(guid))
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[ScavLib] Detected a coexisting custom-item framework " +
                        $"'{guid}'. ScavLib runs its spawn patch at low priority and " +
                        $"only handles its own ids, but cross-framework conflicts " +
                        $"(duplicate ids, mutually exclusive Utils.Create overrides) " +
                        $"cannot be auto-resolved. If custom items misbehave, this " +
                        $"coexistence is the likely cause. Mention this log line in " +
                        $"any bug report.");
                }
            }
        }
    }
}
