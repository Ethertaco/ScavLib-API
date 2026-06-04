namespace ScavLib.gui.ugui
{
    public sealed partial class UguiTheme
    {

        public sealed class TypographyData
        {
            public readonly float BodyFontSize;
            public readonly float TitleFontSize;
            public readonly float SliderLabelFontSize;
            public readonly string[] PrimaryFontHints;
            public readonly string[] FallbackFontHints;

            public TypographyData(
                float bodyFontSize, float titleFontSize, float sliderLabelFontSize,
                string[] primaryFontHints, string[] fallbackFontHints)
            {
                BodyFontSize = bodyFontSize;
                TitleFontSize = titleFontSize;
                SliderLabelFontSize = sliderLabelFontSize;
                PrimaryFontHints = primaryFontHints;
                FallbackFontHints = fallbackFontHints;
            }
        }

        public enum TitleAlign
        {
            Left,
            Center,
            Right,
        }
    }
}
