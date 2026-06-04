using UnityEngine;

namespace ScavLib.gui.ugui
{
    public sealed partial class UguiTheme
    {

        public sealed class FallbackData
        {
            public readonly Vector2 OutlineDistanceOuter;
            public readonly Vector2 OutlineDistanceInner;
            public readonly float BorderThickness;
            public readonly float BorderInset;

            public FallbackData(Vector2 outlineDistanceOuter, Vector2 outlineDistanceInner,
                float borderThickness, float borderInset)
            {
                OutlineDistanceOuter = outlineDistanceOuter;
                OutlineDistanceInner = outlineDistanceInner;
                BorderThickness = borderThickness;
                BorderInset = borderInset;
            }
        }

        public UguiTheme WithRowSpacing(float rowSpacing)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(rowSpacing: rowSpacing), Fallback);

        public UguiTheme WithDefaultSpace(float defaultSpace)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(defaultSpace: defaultSpace), Fallback);

        public UguiTheme WithRowHeight(float rowHeight)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(rowHeight: rowHeight), Fallback);

        public UguiTheme WithHeaderHeight(float headerHeight)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(headerHeight: headerHeight), Fallback);

        public UguiTheme WithSmallButtonHeight(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(smallButtonHeight: v), Fallback);

        public UguiTheme WithSmallButtonPadding(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(smallButtonPadding: v), Fallback);

        public UguiTheme WithSmallButtonMinWidth(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(smallButtonMinWidth: v), Fallback);

        public UguiTheme WithHorizontalSpacing(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(horizontalSpacing: v), Fallback);

        public UguiTheme WithTabHeaderHeight(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(tabHeaderHeight: v), Fallback);

        public UguiTheme WithAutoSizeHeightLimits(float minHeight, float maxHeight)
            => new UguiTheme(Sprites, Palette, Typography,
                Metrics.CloneWith(autoSizeMinHeight: minHeight, autoSizeMaxHeight: maxHeight), Fallback);

        public UguiTheme WithInputFieldHeight(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(inputFieldHeight: v), Fallback);

        public UguiTheme WithInputFieldPadding(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(inputFieldPadding: v), Fallback);

        public UguiTheme WithStepperButtonWidth(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(stepperButtonWidth: v), Fallback);

        public UguiTheme WithShowCloseButton(bool show = true)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(showCloseButton: show), Fallback);

        public UguiTheme WithCloseButtonSize(float size)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(closeButtonSize: size), Fallback);

        public UguiTheme WithCloseButtonRightPad(float pad)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(closeButtonRightPad: pad), Fallback);

        public UguiTheme WithScrollbarWidth(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(scrollbarWidth: v), Fallback);

        public UguiTheme WithImageDefaultMaxSize(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(imageDefaultMaxSize: v), Fallback);

        public UguiTheme WithDropdownArrowWidth(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(dropdownArrowWidth: v), Fallback);

        public UguiTheme WithDropdownRowHeight(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(dropdownRowHeight: v), Fallback);

        public UguiTheme WithDropdownMaxVisibleRows(int v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(dropdownMaxVisibleRows: v), Fallback);

        public UguiTheme WithTooltipMaxWidth(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(tooltipMaxWidth: v), Fallback);

        public UguiTheme WithTooltipPadding(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(tooltipPadding: v), Fallback);

        public UguiTheme WithTooltipCursorOffset(float x, float y)
            => new UguiTheme(Sprites, Palette, Typography,
                Metrics.CloneWith(tooltipCursorOffset: new Vector2(x, y)), Fallback);

        public UguiTheme WithToggleGroupButtonHeight(float v)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(toggleGroupButtonHeight: v), Fallback);

        public UguiTheme WithWindowSize(float width, float height)
            => new UguiTheme(Sprites, Palette, Typography,
                Metrics.CloneWith(windowDefaultSize: new Vector2(width, height)), Fallback);

        public UguiTheme WithPanelColor(Color panelColor)
            => new UguiTheme(Sprites, Palette.CloneWithPanelColor(panelColor), Typography, Metrics, Fallback);

        public UguiTheme WithHeaderBg(Color headerBg)
            => new UguiTheme(Sprites, Palette.CloneWithHeaderBg(headerBg), Typography, Metrics, Fallback);

        public UguiTheme WithContentPadding(float padding)
            => WithContentPadding(padding, padding, padding, padding);

        public UguiTheme WithContentPadding(float left, float right, float top, float bottom)
        {
            var padMin = new Vector2(left, bottom);
            var padMax = new Vector2(-right, -top);
            return new UguiTheme(Sprites, Palette, Typography,
                Metrics.CloneWith(contentPadMin: padMin, contentPadMax: padMax), Fallback);
        }

        public UguiTheme WithHiddenTitle(bool hidden = true)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(hideTitle: hidden), Fallback);

        public UguiTheme WithTitleAlignment(TitleAlign alignment)
            => new UguiTheme(Sprites, Palette, Typography, Metrics.CloneWith(titleAlignment: alignment), Fallback);

        public UguiTheme WithTitlePadding(float padding)
            => WithTitlePadding(padding, padding, padding, padding);

        public UguiTheme WithTitlePadding(float left, float right, float top, float bottom)
            => new UguiTheme(Sprites, Palette, Typography,
                Metrics.CloneWith(titlePadLeft: left, titlePadRight: right, titlePadTop: top, titlePadBottom: bottom),
                Fallback);
    }
}
