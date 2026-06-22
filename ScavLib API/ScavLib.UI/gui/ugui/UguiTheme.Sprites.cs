namespace ScavLib.gui.ugui
{
    public sealed partial class UguiTheme
    {

        public sealed class SpriteHints
        {
            public readonly string[] Panel;
            public readonly string[] Button;
            public readonly string[] Box;
            public readonly string[] Check;
            public readonly string[] Fill;

            public SpriteHints(string[] panel, string[] button, string[] box, string[] check, string[] fill)
            {
                Panel = panel;
                Button = button;
                Box = box;
                Check = check;
                Fill = fill;
            }
        }
    }
}
