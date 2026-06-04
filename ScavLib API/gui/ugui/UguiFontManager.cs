using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScavLib.gui.ugui
{
    public static class UguiFontManager
    {
        public static TMP_FontAsset PrimaryFont { get; private set; }

        private static string[] PrimaryHints => UguiTheme.Default.Typography.PrimaryFontHints;
        private static string[] FallbackHints => UguiTheme.Default.Typography.FallbackFontHints;

        private static bool _initialized;
        private static readonly List<TMP_FontAsset> _fallbackChain
            = new List<TMP_FontAsset>();

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            ResolvePrimary();
            BuildFallbackChain();
            ApplyFallbacks();

            SceneManager.sceneLoaded += OnSceneLoaded;

            ScavLibPlugin.Log.LogInfo(
                $"[ScavLib.Ugui.FontManager] Primary font: '{PrimaryFont?.name ?? "<none>"}', " +
                $"fallback chain size: {_fallbackChain.Count}.");
        }

        public static void EnsurePrimaryResolved()
        {
            if (PrimaryFont != null) return;

            if (PlayerCamera.main != null && PlayerCamera.main.mainCanvas != null)
            {
                var tmp = PlayerCamera.main.mainCanvas
                                      .GetComponentInChildren<TextMeshProUGUI>(true);
                if (tmp != null && tmp.font != null)
                {
                    PrimaryFont = tmp.font;
                    ScavLibPlugin.Log.LogInfo(
                        $"[ScavLib.Ugui.FontManager] Primary resolved late from canvas: '{PrimaryFont.name}'.");
                    return;
                }
            }

            ResourceLookupCache.Invalidate();
            ResolvePrimary();
            BuildFallbackChain();
            ApplyFallbacks();
        }

        private static void ResolvePrimary()
        {
            PrimaryFont = ResourceLookupCache.FindFontAny(PrimaryHints);

            if (PrimaryFont == null)
            {
                foreach (var f in ResourceLookupCache.AllFonts())
                {
                    PrimaryFont = f;
                    break;
                }
            }
        }

        private static void BuildFallbackChain()
        {
            _fallbackChain.Clear();
            foreach (var hint in FallbackHints)
            {
                var f = ResourceLookupCache.FindFontContains(hint);
                if (f != null && !_fallbackChain.Contains(f))
                    _fallbackChain.Add(f);
            }
        }

        private static void ApplyFallbacks()
        {
            if (_fallbackChain.Count == 0) return;

            if (TMP_Settings.fallbackFontAssets != null)
            {
                foreach (var f in _fallbackChain)
                    if (!TMP_Settings.fallbackFontAssets.Contains(f))
                        TMP_Settings.fallbackFontAssets.Add(f);
            }

            foreach (var font in ResourceLookupCache.AllFonts())
            {
                if (font == null) continue;
                if (font.fallbackFontAssetTable == null)
                    font.fallbackFontAssetTable = new List<TMP_FontAsset>();

                foreach (var fb in _fallbackChain)
                {
                    if (fb == font) continue;
                    if (!font.fallbackFontAssetTable.Contains(fb))
                        font.fallbackFontAssetTable.Add(fb);
                }
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ResourceLookupCache.Invalidate();
            if (PrimaryFont == null) ResolvePrimary();
            BuildFallbackChain();
            ApplyFallbacks();
        }
    }
}
