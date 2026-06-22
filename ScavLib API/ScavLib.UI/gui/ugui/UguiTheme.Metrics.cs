using UnityEngine;

namespace ScavLib.gui.ugui
{
    public sealed partial class UguiTheme
    {

        public sealed class MetricsData
        {
            public readonly float RowHeight;
            public readonly float SliderRowHeight;
            public readonly float RowSpacing;
            public readonly float SeparatorHeight;
            public readonly float DefaultSpace;
            public readonly float HeaderHeight;
            public readonly Vector2 ContentPadMin;
            public readonly Vector2 ContentPadMax;
            public readonly float TitleTextInset;
            public readonly Vector2 WindowDefaultSize;

            public readonly float SmallButtonHeight;
            public readonly float SmallButtonPadding;
            public readonly float SmallButtonMinWidth;
            public readonly float HorizontalSpacing;

            public readonly float TabHeaderHeight;
            public readonly float AutoSizeMinHeight;
            public readonly float AutoSizeMaxHeight;

            public readonly float InputFieldHeight;

            public readonly float InputFieldPadding;

            public readonly float StepperButtonWidth;

            public readonly bool ShowCloseButton;

            public readonly float CloseButtonSize;

            public readonly float CloseButtonRightPad;

            public readonly float ScrollbarWidth;

            public readonly float ImageDefaultMaxSize;

            public readonly float DropdownArrowWidth;

            public readonly float DropdownRowHeight;

            public readonly int DropdownMaxVisibleRows;

            public readonly float TooltipMaxWidth;

            public readonly float TooltipPadding;

            public readonly Vector2 TooltipCursorOffset;

            public readonly float ToggleGroupButtonHeight;

            public readonly bool HideTitle;
            public readonly TitleAlign TitleAlignment;
            public readonly float TitlePadLeft;
            public readonly float TitlePadRight;
            public readonly float TitlePadTop;
            public readonly float TitlePadBottom;

            public readonly float ButtonTextInset;
            public readonly float ToggleBoxSize;
            public readonly float ToggleLabelIndent;
            public readonly float CheckmarkInsetMin;
            public readonly float CheckmarkInsetMax;
            public readonly float SliderTrackTop;
            public readonly Vector2 SliderTrackPadMin;
            public readonly Vector2 SliderTrackPadMax;
            public readonly float SliderHandlePad;
            public readonly float SliderHandleWidth;
            public readonly float ScrollSensitivity;
            public readonly float ProgressBarHeightRatio;

            public MetricsData(
                float rowHeight, float sliderRowHeight, float rowSpacing, float separatorHeight,
                float defaultSpace, float headerHeight, Vector2 contentPadMin, Vector2 contentPadMax,
                float titleTextInset, Vector2 windowDefaultSize,
                float smallButtonHeight, float smallButtonPadding, float smallButtonMinWidth,
                float horizontalSpacing,
                float tabHeaderHeight, float autoSizeMinHeight, float autoSizeMaxHeight,
                float inputFieldHeight, float inputFieldPadding, float stepperButtonWidth,
                bool showCloseButton, float closeButtonSize, float closeButtonRightPad,
                float scrollbarWidth,
                float imageDefaultMaxSize, float dropdownArrowWidth, float dropdownRowHeight,
                int dropdownMaxVisibleRows, float tooltipMaxWidth, float tooltipPadding,
                Vector2 tooltipCursorOffset, float toggleGroupButtonHeight,
                bool hideTitle, TitleAlign titleAlignment,
                float titlePadLeft, float titlePadRight, float titlePadTop, float titlePadBottom,
                float buttonTextInset, float toggleBoxSize, float toggleLabelIndent,
                float checkmarkInsetMin, float checkmarkInsetMax,
                float sliderTrackTop, Vector2 sliderTrackPadMin, Vector2 sliderTrackPadMax,
                float sliderHandlePad, float sliderHandleWidth, float scrollSensitivity,
                float progressBarHeightRatio)
            {
                RowHeight = rowHeight;
                SliderRowHeight = sliderRowHeight;
                RowSpacing = rowSpacing;
                SeparatorHeight = separatorHeight;
                DefaultSpace = defaultSpace;
                HeaderHeight = headerHeight;
                ContentPadMin = contentPadMin;
                ContentPadMax = contentPadMax;
                TitleTextInset = titleTextInset;
                WindowDefaultSize = windowDefaultSize;
                SmallButtonHeight = smallButtonHeight;
                SmallButtonPadding = smallButtonPadding;
                SmallButtonMinWidth = smallButtonMinWidth;
                HorizontalSpacing = horizontalSpacing;
                TabHeaderHeight = tabHeaderHeight;
                AutoSizeMinHeight = autoSizeMinHeight;
                AutoSizeMaxHeight = autoSizeMaxHeight;
                InputFieldHeight = inputFieldHeight;
                InputFieldPadding = inputFieldPadding;
                StepperButtonWidth = stepperButtonWidth;
                ShowCloseButton = showCloseButton;
                CloseButtonSize = closeButtonSize;
                CloseButtonRightPad = closeButtonRightPad;
                ScrollbarWidth = scrollbarWidth;
                ImageDefaultMaxSize = imageDefaultMaxSize;
                DropdownArrowWidth = dropdownArrowWidth;
                DropdownRowHeight = dropdownRowHeight;
                DropdownMaxVisibleRows = dropdownMaxVisibleRows;
                TooltipMaxWidth = tooltipMaxWidth;
                TooltipPadding = tooltipPadding;
                TooltipCursorOffset = tooltipCursorOffset;
                ToggleGroupButtonHeight = toggleGroupButtonHeight;
                HideTitle = hideTitle;
                TitleAlignment = titleAlignment;
                TitlePadLeft = titlePadLeft;
                TitlePadRight = titlePadRight;
                TitlePadTop = titlePadTop;
                TitlePadBottom = titlePadBottom;
                ButtonTextInset = buttonTextInset;
                ToggleBoxSize = toggleBoxSize;
                ToggleLabelIndent = toggleLabelIndent;
                CheckmarkInsetMin = checkmarkInsetMin;
                CheckmarkInsetMax = checkmarkInsetMax;
                SliderTrackTop = sliderTrackTop;
                SliderTrackPadMin = sliderTrackPadMin;
                SliderTrackPadMax = sliderTrackPadMax;
                SliderHandlePad = sliderHandlePad;
                SliderHandleWidth = sliderHandleWidth;
                ScrollSensitivity = scrollSensitivity;
                ProgressBarHeightRatio = progressBarHeightRatio;
            }

            internal MetricsData CloneWith(
                float? rowHeight = null,
                float? rowSpacing = null,
                float? defaultSpace = null,
                float? headerHeight = null,
                Vector2? windowDefaultSize = null,
                Vector2? contentPadMin = null,
                Vector2? contentPadMax = null,
                float? smallButtonHeight = null,
                float? smallButtonPadding = null,
                float? smallButtonMinWidth = null,
                float? horizontalSpacing = null,
                float? tabHeaderHeight = null,
                float? autoSizeMinHeight = null,
                float? autoSizeMaxHeight = null,
                float? inputFieldHeight = null,
                float? inputFieldPadding = null,
                float? stepperButtonWidth = null,
                bool? showCloseButton = null,
                float? closeButtonSize = null,
                float? closeButtonRightPad = null,
                float? scrollbarWidth = null,
                float? imageDefaultMaxSize = null,
                float? dropdownArrowWidth = null,
                float? dropdownRowHeight = null,
                int? dropdownMaxVisibleRows = null,
                float? tooltipMaxWidth = null,
                float? tooltipPadding = null,
                Vector2? tooltipCursorOffset = null,
                float? toggleGroupButtonHeight = null,
                bool? hideTitle = null,
                TitleAlign? titleAlignment = null,
                float? titlePadLeft = null,
                float? titlePadRight = null,
                float? titlePadTop = null,
                float? titlePadBottom = null)
            {
                return new MetricsData(
                    rowHeight ?? RowHeight,
                    SliderRowHeight,
                    rowSpacing ?? RowSpacing,
                    SeparatorHeight,
                    defaultSpace ?? DefaultSpace,
                    headerHeight ?? HeaderHeight,
                    contentPadMin ?? ContentPadMin,
                    contentPadMax ?? ContentPadMax,
                    TitleTextInset,
                    windowDefaultSize ?? WindowDefaultSize,
                    smallButtonHeight ?? SmallButtonHeight,
                    smallButtonPadding ?? SmallButtonPadding,
                    smallButtonMinWidth ?? SmallButtonMinWidth,
                    horizontalSpacing ?? HorizontalSpacing,
                    tabHeaderHeight ?? TabHeaderHeight,
                    autoSizeMinHeight ?? AutoSizeMinHeight,
                    autoSizeMaxHeight ?? AutoSizeMaxHeight,
                    inputFieldHeight ?? InputFieldHeight,
                    inputFieldPadding ?? InputFieldPadding,
                    stepperButtonWidth ?? StepperButtonWidth,
                    showCloseButton ?? ShowCloseButton,
                    closeButtonSize ?? CloseButtonSize,
                    closeButtonRightPad ?? CloseButtonRightPad,
                    scrollbarWidth ?? ScrollbarWidth,
                    imageDefaultMaxSize ?? ImageDefaultMaxSize,
                    dropdownArrowWidth ?? DropdownArrowWidth,
                    dropdownRowHeight ?? DropdownRowHeight,
                    dropdownMaxVisibleRows ?? DropdownMaxVisibleRows,
                    tooltipMaxWidth ?? TooltipMaxWidth,
                    tooltipPadding ?? TooltipPadding,
                    tooltipCursorOffset ?? TooltipCursorOffset,
                    toggleGroupButtonHeight ?? ToggleGroupButtonHeight,
                    hideTitle ?? HideTitle,
                    titleAlignment ?? TitleAlignment,
                    titlePadLeft ?? TitlePadLeft,
                    titlePadRight ?? TitlePadRight,
                    titlePadTop ?? TitlePadTop,
                    titlePadBottom ?? TitlePadBottom,
                    ButtonTextInset,
                    ToggleBoxSize,
                    ToggleLabelIndent,
                    CheckmarkInsetMin,
                    CheckmarkInsetMax,
                    SliderTrackTop,
                    SliderTrackPadMin,
                    SliderTrackPadMax,
                    SliderHandlePad,
                    SliderHandleWidth,
                    ScrollSensitivity,
                    ProgressBarHeightRatio);
            }
        }
    }
}
