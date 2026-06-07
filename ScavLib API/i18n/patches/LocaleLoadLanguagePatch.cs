
using HarmonyLib;

namespace ScavLib.i18n.patches
{

    [HarmonyPatch(typeof(Locale), nameof(Locale.LoadLanguage))]
    internal static class LocaleLoadLanguagePatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                LocaleRegistry.Flush();
            }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[LocaleLoadLanguagePatch] Failed to flush i18n: {ex}");
            }
        }
    }
}
