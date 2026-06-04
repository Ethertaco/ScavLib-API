using UnityEngine;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;

namespace ScavLib.gui.ugui
{

    internal static class UguiStyleApplier
    {

        public static bool ApplyNineSliceOrOutline(GameObject go, UImage img, string[] spriteHints,
            Color fallbackColor, UguiTheme theme, Color? outlineColor = null)
        {
            var sprite = ResourceLookupCache.FindSpriteAny(spriteHints);
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = UImage.Type.Sliced;
                img.color = Color.white;
                return true;
            }

            img.color = fallbackColor;
            var oc = outlineColor ?? Color.white;
            var ol1 = go.AddComponent<Outline>();
            ol1.effectColor = oc;
            ol1.effectDistance = theme.Fallback.OutlineDistanceOuter;
            var ol2 = go.AddComponent<Outline>();
            ol2.effectColor = oc;
            ol2.effectDistance = theme.Fallback.OutlineDistanceInner;
            return false;
        }
    }
}
