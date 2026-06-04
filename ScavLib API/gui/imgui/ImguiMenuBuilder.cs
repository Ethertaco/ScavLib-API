using UnityEngine;

namespace ScavLib.gui.imgui
{

    public static class ImguiMenuBuilder
    {

        public static bool Button(string label)
        {
            return GUILayout.Button(label);
        }

        public static bool Toggle(string label, bool value)
        {
            return GUILayout.Toggle(value, label);
        }

        public static float Slider(string label, float value, float min, float max)
        {
            GUILayout.Label($"{label}: {value:F2}");
            return GUILayout.HorizontalSlider(value, min, max);
        }

        public static void SliderConfig(string label,
            BepInEx.Configuration.ConfigEntry<float> entry, float min, float max)
        {
            entry.Value = Slider(label, entry.Value, min, max);
        }

        public static void ToggleConfig(string label,
            BepInEx.Configuration.ConfigEntry<bool> entry)
        {
            entry.Value = Toggle(label, entry.Value);
        }

        public static void Label(string text)
        {
            GUILayout.Label(text);
        }

        public static void Separator()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1f));
        }

        public static void Space(float pixels = 8f)
        {
            GUILayout.Space(pixels);
        }

        public static void BeginHorizontal() => GUILayout.BeginHorizontal();

        public static void EndHorizontal() => GUILayout.EndHorizontal();

        public static void BeginScroll(ref Vector2 scrollPos)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
        }

        public static void EndScroll() => GUILayout.EndScrollView();
    }
}
