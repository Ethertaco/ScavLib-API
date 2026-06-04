using UnityEngine;
using UnityEngine.UI;

namespace ScavLib.gui.ugui
{
    public sealed partial class UguiTheme
    {

        public sealed class PaletteData
        {
            public readonly ColorBlock ButtonColors;
            public readonly Color TextPrimary;
            public readonly Color PanelTint;
            public readonly Color PanelFallbackBg;
            public readonly Color HeaderBg;
            public readonly Color BorderColor;
            public readonly Color SliderTrackTint;
            public readonly Color SliderTrackOutline;
            public readonly Color SliderFillTint;

            public readonly Color InputFieldBg;
            public readonly Color InputPlaceholder;
            public readonly Color CloseButtonTint;

            public readonly Color DropdownArrowTint;

            public readonly Color TooltipBg;

            public readonly Color ToggleSelectedTint;

            public PaletteData(
                ColorBlock buttonColors, Color textPrimary, Color panelTint, Color panelFallbackBg,
                Color headerBg, Color borderColor, Color sliderTrackTint, Color sliderTrackOutline,
                Color sliderFillTint, Color inputFieldBg, Color inputPlaceholder, Color closeButtonTint,
                Color dropdownArrowTint, Color tooltipBg, Color toggleSelectedTint)
            {
                ButtonColors = buttonColors;
                TextPrimary = textPrimary;
                PanelTint = panelTint;
                PanelFallbackBg = panelFallbackBg;
                HeaderBg = headerBg;
                BorderColor = borderColor;
                SliderTrackTint = sliderTrackTint;
                SliderTrackOutline = sliderTrackOutline;
                SliderFillTint = sliderFillTint;
                InputFieldBg = inputFieldBg;
                InputPlaceholder = inputPlaceholder;
                CloseButtonTint = closeButtonTint;
                DropdownArrowTint = dropdownArrowTint;
                TooltipBg = tooltipBg;
                ToggleSelectedTint = toggleSelectedTint;
            }

            internal PaletteData CloneWithHeaderBg(Color headerBg)
            {
                return new PaletteData(
                    ButtonColors, TextPrimary, PanelTint, PanelFallbackBg,
                    headerBg, BorderColor, SliderTrackTint, SliderTrackOutline,
                    SliderFillTint, InputFieldBg, InputPlaceholder, CloseButtonTint,
                    DropdownArrowTint, TooltipBg, ToggleSelectedTint);
            }

            internal PaletteData CloneWithPanelColor(Color panelFallbackBg)
            {
                return new PaletteData(
                    ButtonColors, TextPrimary, PanelTint, panelFallbackBg,
                    panelFallbackBg, BorderColor, SliderTrackTint, SliderTrackOutline,
                    SliderFillTint, InputFieldBg, InputPlaceholder, CloseButtonTint,
                    DropdownArrowTint, TooltipBg, ToggleSelectedTint);
            }
        }
    }
}
