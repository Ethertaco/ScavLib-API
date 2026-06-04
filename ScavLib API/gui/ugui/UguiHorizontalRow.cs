using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;

namespace ScavLib.gui.ugui
{

    public sealed class UguiHorizontalRow : IDisposable
    {
        private readonly RectTransform _rowRect;
        private readonly TMP_FontAsset _font;
        private readonly UguiTheme _theme;
        private readonly Action<float> _commitRowHeight;
        private readonly float _rowWidth;
        private readonly float _rowHeight;

        private float _cursorX;

        private readonly List<FlexItem> _flexItems = new List<FlexItem>();
        private bool _ended;

        private struct FlexItem
        {
            public string Label;
            public Action OnClick;
        }

        internal UguiHorizontalRow(
            RectTransform rowRect, float rowWidth, float rowHeight,
            TMP_FontAsset font, UguiTheme theme, Action<float> commitRowHeight)
        {
            _rowRect = rowRect;
            _rowWidth = rowWidth;
            _rowHeight = rowHeight;
            _font = font ?? UguiFontManager.PrimaryFont;
            _theme = theme ?? UguiTheme.Default;
            _commitRowHeight = commitRowHeight;
            _cursorX = 0f;
        }

        public Button AddSmallButton(string label, Action onClick, float fixedWidth)
        {
            float w = Mathf.Max(fixedWidth, _theme.Metrics.SmallButtonMinWidth);
            var btn = BuildButtonAt(label, onClick, _cursorX, w);
            AdvanceCursor(w);
            return btn;
        }

        public Button AddSmallButton(string label, Action onClick)
        {
            float w = MeasureButtonWidth(label);
            var btn = BuildButtonAt(label, onClick, _cursorX, w);
            AdvanceCursor(w);
            return btn;
        }

        public void AddFlexibleSmallButton(string label, Action onClick)
        {
            _flexItems.Add(new FlexItem { Label = label, OnClick = onClick });
        }

        public void End()
        {
            if (_ended) return;
            _ended = true;

            if (_flexItems.Count > 0)
                LayoutFlexItems();

            _commitRowHeight?.Invoke(_rowHeight);
        }

        public void Dispose() => End();

        private void LayoutFlexItems()
        {
            float spacing = _theme.Metrics.HorizontalSpacing;
            float remaining = _rowWidth - _cursorX;
            int n = _flexItems.Count;
            if (remaining <= 0f || n <= 0) return;

            float totalSpacing = spacing * (n - 1);
            float each = Mathf.Max((remaining - totalSpacing) / n, _theme.Metrics.SmallButtonMinWidth);

            float x = _cursorX;
            for (int i = 0; i < n; i++)
            {
                BuildButtonAt(_flexItems[i].Label, _flexItems[i].OnClick, x, each);
                x += each + spacing;
            }
        }

        private void AdvanceCursor(float width)
        {
            _cursorX += width + _theme.Metrics.HorizontalSpacing;
        }

        private float MeasureButtonWidth(string label)
        {
            float textW = MeasureTextWidth(label);
            float w = textW + _theme.Metrics.SmallButtonPadding * 2f;
            return Mathf.Max(w, _theme.Metrics.SmallButtonMinWidth);
        }

        private float MeasureTextWidth(string label)
        {
            var f = _font ?? UguiFontManager.PrimaryFont;
            if (f == null || string.IsNullOrEmpty(label))
                return _theme.Metrics.SmallButtonMinWidth;

            var measureGo = new GameObject("__measure");
            measureGo.hideFlags = HideFlags.HideAndDontSave;
            var tmp = measureGo.AddComponent<TextMeshProUGUI>();
            tmp.font = f;
            tmp.fontSize = _theme.Typography.BodyFontSize;
            tmp.text = label;
            tmp.enableWordWrapping = false;
            float w = tmp.preferredWidth;
            UnityEngine.Object.DestroyImmediate(measureGo);
            return w;
        }

        private Button BuildButtonAt(string label, Action onClick, float x, float width)
        {

            var go = new GameObject("SmallButton");
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(_rowRect, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(x, 0f);
            rect.sizeDelta = new Vector2(width, 0f);

            var img = go.AddComponent<UImage>();

            UguiStyleApplier.ApplyNineSliceOrOutline(go, img, _theme.Sprites.Button, Color.black, _theme);
            img.raycastTarget = true;

            var btn = go.AddComponent<Button>();
            btn.colors = _theme.Palette.ButtonColors;
            btn.targetGraphic = img;
            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var textGo = new GameObject("Text");
            textGo.layer = LayerMask.NameToLayer("UI");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(_theme.Metrics.SmallButtonPadding, 0f);
            textRect.offsetMax = new Vector2(-_theme.Metrics.SmallButtonPadding, 0f);

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = _theme.Typography.BodyFontSize;
            tmp.color = _theme.Palette.TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            tmp.raycastTarget = false;
            var f = _font ?? UguiFontManager.PrimaryFont;
            if (f != null) tmp.font = f;

            return btn;
        }
    }
}
