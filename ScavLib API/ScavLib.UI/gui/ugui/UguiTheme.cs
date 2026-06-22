using UnityEngine;
using UnityEngine.UI;

namespace ScavLib.gui.ugui
{

    public sealed partial class UguiTheme
    {
        public readonly SpriteHints Sprites;
        public readonly PaletteData Palette;
        public readonly TypographyData Typography;
        public readonly MetricsData Metrics;
        public readonly FallbackData Fallback;

        public UguiTheme(
            SpriteHints sprites,
            PaletteData palette,
            TypographyData typography,
            MetricsData metrics,
            FallbackData fallback)
        {
            Sprites = sprites;
            Palette = palette;
            Typography = typography;
            Metrics = metrics;
            Fallback = fallback;
        }

        private static UguiTheme _default;
        public static UguiTheme Default => _default ?? (_default = BuildDefault());

        private static UguiTheme BuildDefault()
        {
            var sprites = new SpriteHints(
                panel: new[] { "uiBlock", "uiBlockSmall" },
                button: new[] { "uiBlockNano" },
                box: new[] { "uiBlockNano" },
                check: new[] { "whitesquare", "Square" },
                fill: new[] { "uiBlockNano" });

            var btnColors = ColorBlock.defaultColorBlock;
            btnColors.normalColor = Color.white;
            btnColors.highlightedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            btnColors.pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            btnColors.selectedColor = Color.white;
            btnColors.colorMultiplier = 1f;
            btnColors.fadeDuration = 0.08f;

            var panelFallbackBg = new Color(0f, 0f, 0f, 0.50f);

            var palette = new PaletteData(
                buttonColors: btnColors,
                textPrimary: Color.white,
                panelTint: Color.white,
                panelFallbackBg: panelFallbackBg,
                headerBg: panelFallbackBg,
                borderColor: Color.white,
                sliderTrackTint: new Color(0.4f, 0.4f, 0.4f, 1f),
                sliderTrackOutline: new Color(0.35f, 0.35f, 0.35f, 1f),
                sliderFillTint: Color.white,
                inputFieldBg: new Color(0f, 0f, 0f, 0.35f),
                inputPlaceholder: new Color(1f, 1f, 1f, 0.4f),
                closeButtonTint: Color.white,

                dropdownArrowTint: Color.white,
                tooltipBg: new Color(0f, 0f, 0f, 0.85f),
                toggleSelectedTint: new Color(0.55f, 0.55f, 0.55f, 1f));

            var typography = new TypographyData(
                bodyFontSize: 13f,
                titleFontSize: 14f,
                sliderLabelFontSize: 12f,
                primaryFontHints: new[]
                {
                    "Retro Gaming SDF", "Retro GamingPix", "Retro Gaming",
                    "ConsoleFont", "Romulus", "LiberationSans SDF",
                },
                fallbackFontHints: new[]
                {
                    "unifont", "NotoSans", "NotoAscii", "ConsoleFont", "LiberationSans SDF",
                });

            var metrics = new MetricsData(
                rowHeight: 28f,
                sliderRowHeight: 44f,
                rowSpacing: 5f,
                separatorHeight: 6f,
                defaultSpace: 8f,
                headerHeight: 28f,
                contentPadMin: new Vector2(10f, 10f),
                contentPadMax: new Vector2(-10f, -32f),
                titleTextInset: 8f,
                windowDefaultSize: new Vector2(400f, 300f),

                smallButtonHeight: 22f,
                smallButtonPadding: 8f,
                smallButtonMinWidth: 24f,
                horizontalSpacing: 5f,

                tabHeaderHeight: 24f,
                autoSizeMinHeight: 80f,
                autoSizeMaxHeight: 720f,

                inputFieldHeight: 26f,
                inputFieldPadding: 6f,
                stepperButtonWidth: 26f,
                showCloseButton: true,
                closeButtonSize: 20f,
                closeButtonRightPad: 6f,
                scrollbarWidth: 10f,

                imageDefaultMaxSize: 64f,
                dropdownArrowWidth: 18f,
                dropdownRowHeight: 24f,
                dropdownMaxVisibleRows: 8,
                tooltipMaxWidth: 320f,
                tooltipPadding: 8f,
                tooltipCursorOffset: new Vector2(16f, -16f),
                toggleGroupButtonHeight: 26f,

                hideTitle: false,
                titleAlignment: TitleAlign.Left,
                titlePadLeft: 8f,
                titlePadRight: 0f,
                titlePadTop: 0f,
                titlePadBottom: 0f,
                buttonTextInset: 6f,
                toggleBoxSize: 20f,
                toggleLabelIndent: 28f,
                checkmarkInsetMin: 0.28f,
                checkmarkInsetMax: 0.72f,
                sliderTrackTop: 0.46f,
                sliderTrackPadMin: new Vector2(0f, 2f),
                sliderTrackPadMax: new Vector2(0f, -2f),
                sliderHandlePad: 6f,
                sliderHandleWidth: 14f,
                scrollSensitivity: 20f,
                progressBarHeightRatio: 0.6f);

            var fallback = new FallbackData(
                outlineDistanceOuter: new Vector2(2f, -2f),
                outlineDistanceInner: new Vector2(1f, -1f),
                borderThickness: 7f,
                borderInset: 3f);

            return new UguiTheme(sprites, palette, typography, metrics, fallback);
        }
    }
}
