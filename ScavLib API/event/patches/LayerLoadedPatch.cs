using System.Reflection;
using HarmonyLib;
using ScavLib.event_bus.events;

namespace ScavLib.event_bus.patches
{
    [HarmonyPatch(typeof(WorldGeneration), "Update")]
    internal static class LayerLoadedPatch
    {
        private static bool _wasGenerating = false;
        private static bool _isFirstLoad = true;

        private static readonly FieldInfo _instantiatingWorldField =
            typeof(WorldGeneration).GetField(
                "instantiatingWorld",
                BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPostfix]
        private static void Postfix(WorldGeneration __instance)
        {
            if (__instance == null) return;

            bool instantiating = false;
            if (_instantiatingWorldField != null)
            {
                object raw = _instantiatingWorldField.GetValue(__instance);
                if (raw is bool b) instantiating = b;
            }

            bool isGeneratingNow = __instance.generatingWorld || instantiating;

            if (_wasGenerating && !isGeneratingNow && __instance.worldExists)
            {
                bool isFirst = _isFirstLoad;
                _isFirstLoad = false;

                try
                {
                    EventBus.Post(new LayerLoadedEvent(
                        __instance.biomeDepth,
                        isFirst));
                }
                catch (System.Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[LayerLoadedPatch] Failed to post LayerLoadedEvent: {ex}");
                }
            }

            _wasGenerating = isGeneratingNow;
        }

        internal static void ResetState()
        {
            _wasGenerating = false;
            _isFirstLoad = true;
        }
    }
}
