using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ScavLib.gui.ugui
{
    internal static class ResourceLookupCache
    {
        private static readonly Dictionary<string, Sprite> SpritesByName
            = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Sprite> SpriteContainsCache
            = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, TMP_FontAsset> FontsByName
            = new Dictionary<string, TMP_FontAsset>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, TMP_FontAsset> FontContainsCache
            = new Dictionary<string, TMP_FontAsset>(StringComparer.OrdinalIgnoreCase);

        private static bool _spritesLoaded;
        private static bool _fontsLoaded;

        public static Sprite FindSprite(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            EnsureSpritesLoaded();
            SpritesByName.TryGetValue(name, out var sprite);
            return sprite;
        }

        public static Sprite FindSpriteContains(string fragment)
        {
            if (string.IsNullOrEmpty(fragment)) return null;
            EnsureSpritesLoaded();

            if (SpriteContainsCache.TryGetValue(fragment, out var cached))
                return cached;

            foreach (var kv in SpritesByName)
            {
                if (kv.Value != null &&
                    kv.Key.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    SpriteContainsCache[fragment] = kv.Value;
                    return kv.Value;
                }
            }

            SpriteContainsCache[fragment] = null;
            return null;
        }

        public static Sprite FindSpriteAny(params string[] fragments)
        {
            if (fragments == null) return null;

            foreach (var frag in fragments)
            {
                var exact = FindSprite(frag);
                if (exact != null) return exact;
            }

            foreach (var frag in fragments)
            {
                var s = FindSpriteContains(frag);
                if (s != null) return s;
            }

            return null;
        }

        public static TMP_FontAsset FindFont(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            EnsureFontsLoaded();
            FontsByName.TryGetValue(name, out var font);
            return font;
        }

        public static TMP_FontAsset FindFontContains(string fragment)
        {
            if (string.IsNullOrEmpty(fragment)) return null;
            EnsureFontsLoaded();

            if (FontContainsCache.TryGetValue(fragment, out var cached))
                return cached;

            foreach (var kv in FontsByName)
            {
                if (kv.Value != null &&
                    kv.Key.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    FontContainsCache[fragment] = kv.Value;
                    return kv.Value;
                }
            }

            FontContainsCache[fragment] = null;
            return null;
        }

        public static TMP_FontAsset FindFontAny(params string[] fragments)
        {
            if (fragments == null) return null;
            foreach (var frag in fragments)
            {
                var exact = FindFont(frag);
                if (exact != null) return exact;
            }
            foreach (var frag in fragments)
            {
                var f = FindFontContains(frag);
                if (f != null) return f;
            }
            return null;
        }

        public static IEnumerable<TMP_FontAsset> AllFonts()
        {
            EnsureFontsLoaded();
            return new List<TMP_FontAsset>(FontsByName.Values);
        }

        public static void Invalidate()
        {
            _spritesLoaded = false;
            _fontsLoaded = false;
            SpritesByName.Clear();
            SpriteContainsCache.Clear();
            FontsByName.Clear();
            FontContainsCache.Clear();
        }

        private static void EnsureSpritesLoaded()
        {
            if (_spritesLoaded) return;
            _spritesLoaded = true;

            foreach (var sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (sprite == null || string.IsNullOrEmpty(sprite.name)) continue;
                if (!SpritesByName.ContainsKey(sprite.name))
                    SpritesByName.Add(sprite.name, sprite);
            }
        }

        private static void EnsureFontsLoaded()
        {
            if (_fontsLoaded) return;
            _fontsLoaded = true;

            foreach (var font in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if (font == null || string.IsNullOrEmpty(font.name)) continue;
                if (!FontsByName.ContainsKey(font.name))
                    FontsByName.Add(font.name, font);
            }
        }
    }
}
