using HarmonyLib;
using UnityEngine;

namespace ScavLib.gui.ugui
{

    [HarmonyPatch(typeof(PlayerCamera), "HandleStartDragging")]
    internal static class UguiInputBlockerPatch
    {
        [HarmonyPrefix]
        private static bool Prefix()
        {
            if (!UguiWindowBase.AnyBlockingWindowVisible())
                return true;

            if (UIUtil.IsPointerOverUIElement())
                return false;

            return true;
        }
    }
}
