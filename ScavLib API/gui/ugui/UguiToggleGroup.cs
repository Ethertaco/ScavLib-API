using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;
using UToggle = UnityEngine.UI.Toggle;

namespace ScavLib.gui.ugui
{

    public enum UguiToggleGroupStyle
    {

        Checkbox,

        Buttons,
    }

    public sealed class UguiToggleGroup
    {
        private readonly UguiTheme _theme;
        private readonly TMP_FontAsset _font;
        private readonly UguiToggleGroupStyle _style;
        private readonly Action<int> _onChange;

        private ToggleGroup _toggleGroup;
        private readonly List<UToggle> _toggles = new List<UToggle>();

        private readonly List<Button> _buttons = new List<Button>();

        private readonly List<UImage> _buttonImages = new List<UImage>();

        private int _selectedIndex;
        private bool _suppress;

        public int SelectedIndex => _selectedIndex;

        internal UguiToggleGroup(
            UguiToggleGroupStyle style, TMP_FontAsset font, UguiTheme theme, Action<int> onChange)
        {
            _style = style;
            _font = font ?? UguiFontManager.PrimaryFont;
            _theme = theme ?? UguiTheme.Default;
            _onChange = onChange;
            _selectedIndex = 0;
        }

        internal void BuildCheckbox(
            UguiBuilder builder, RectTransform contentRect,
            IList<string> options, int initialIndex,
            Func<string, float, GameObject> makeRow,
            Func<GameObject, string, GameObject> makeChild)
        {
            _selectedIndex = Mathf.Clamp(initialIndex, 0, Mathf.Max(0, options.Count - 1));

            _toggleGroup = contentRect.gameObject.AddComponent<ToggleGroup>();
            _toggleGroup.allowSwitchOff = false;

            for (int i = 0; i < options.Count; i++)
            {
                int idx = i;
                var go = makeRow("ToggleGroupItem", _theme.Metrics.RowHeight);

                var img = go.AddComponent<UImage>();
                img.color = Color.clear;

                var toggle = go.AddComponent<UToggle>();
                toggle.group = _toggleGroup;
                toggle.isOn = (i == _selectedIndex);

                var boxGo = makeChild(go, "Box");
                var boxRect = boxGo.GetComponent<RectTransform>();
                boxRect.anchorMin = new Vector2(0f, 0.5f);
                boxRect.anchorMax = new Vector2(0f, 0.5f);
                boxRect.pivot = new Vector2(0f, 0.5f);
                boxRect.anchoredPosition = Vector2.zero;
                boxRect.sizeDelta = new Vector2(_theme.Metrics.ToggleBoxSize, _theme.Metrics.ToggleBoxSize);

                var boxImg = boxGo.AddComponent<UImage>();
                UguiStyleApplier.ApplyNineSliceOrOutline(boxGo, boxImg, _theme.Sprites.Box, Color.black, _theme);

                var checkGo = makeChild(boxGo, "Checkmark");
                var checkRect = checkGo.GetComponent<RectTransform>();
                checkRect.anchorMin = new Vector2(_theme.Metrics.CheckmarkInsetMin, _theme.Metrics.CheckmarkInsetMin);
                checkRect.anchorMax = new Vector2(_theme.Metrics.CheckmarkInsetMax, _theme.Metrics.CheckmarkInsetMax);
                checkRect.offsetMin = Vector2.zero;
                checkRect.offsetMax = Vector2.zero;
                var checkImg = checkGo.AddComponent<UImage>();
                var checkSprite = ResourceLookupCache.FindSpriteAny(_theme.Sprites.Check);
                if (checkSprite != null) checkImg.sprite = checkSprite;
                checkImg.color = Color.white;

                toggle.graphic = checkImg;
                toggle.targetGraphic = boxImg;

                var textGo = makeChild(go, "Label");
                var textRect = textGo.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(_theme.Metrics.ToggleLabelIndent, 0f);
                textRect.offsetMax = Vector2.zero;

                var tmp = textGo.AddComponent<TextMeshProUGUI>();
                tmp.text = options[i];
                tmp.fontSize = _theme.Typography.BodyFontSize;
                tmp.color = _theme.Palette.TextPrimary;
                tmp.alignment = TextAlignmentOptions.MidlineLeft;
                tmp.raycastTarget = false;
                if (_font != null) tmp.font = _font;

                toggle.onValueChanged.AddListener(on =>
                {
                    if (_suppress) return;
                    if (on)
                    {
                        _selectedIndex = idx;
                        _onChange?.Invoke(_selectedIndex);
                    }
                });

                _toggles.Add(toggle);
            }
        }

        internal void BuildButtons(
            UguiBuilder builder, RectTransform contentRect,
            IList<string> options, int initialIndex,
            Func<string, float, GameObject> makeRow,
            Func<GameObject, string, GameObject> makeChild)
        {
            _selectedIndex = Mathf.Clamp(initialIndex, 0, Mathf.Max(0, options.Count - 1));

            var rowGo = makeRow("ToggleGroupButtons", _theme.Metrics.ToggleGroupButtonHeight);
            var hlg = rowGo.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = _theme.Metrics.HorizontalSpacing;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;

            for (int i = 0; i < options.Count; i++)
            {
                int idx = i;
                var go = makeChild(rowGo, "Btn");
                var le = go.AddComponent<LayoutElement>();
                le.flexibleWidth = 1f;
                le.preferredHeight = _theme.Metrics.ToggleGroupButtonHeight;

                var img = go.AddComponent<UImage>();
                UguiStyleApplier.ApplyNineSliceOrOutline(go, img, _theme.Sprites.Button, Color.black, _theme);
                img.raycastTarget = true;

                var btn = go.AddComponent<Button>();
                btn.targetGraphic = img;

                btn.colors = _theme.Palette.ButtonColors;
                btn.onClick.AddListener(() =>
                {
                    SetSelected(idx, notify: true);
                });

                var textGo = makeChild(go, "Text");
                var textRect = textGo.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(_theme.Metrics.SmallButtonPadding, 0f);
                textRect.offsetMax = new Vector2(-_theme.Metrics.SmallButtonPadding, 0f);

                var tmp = textGo.AddComponent<TextMeshProUGUI>();
                tmp.text = options[i];
                tmp.fontSize = _theme.Typography.BodyFontSize;
                tmp.color = _theme.Palette.TextPrimary;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.enableWordWrapping = false;
                tmp.overflowMode = TextOverflowModes.Ellipsis;
                tmp.raycastTarget = false;
                if (_font != null) tmp.font = _font;

                _buttons.Add(btn);
                _buttonImages.Add(img);
            }

            RefreshButtonHighlight();
        }

        public void SetSelected(int index, bool notify = true)
        {
            int clamped = Mathf.Clamp(index, 0, Mathf.Max(0, Count - 1));
            if (Count == 0) return;
            bool changed = clamped != _selectedIndex;
            _selectedIndex = clamped;

            if (_style == UguiToggleGroupStyle.Checkbox)
            {
                _suppress = true;
                for (int i = 0; i < _toggles.Count; i++)
                    if (_toggles[i] != null) _toggles[i].isOn = (i == _selectedIndex);
                _suppress = false;
            }
            else
            {
                RefreshButtonHighlight();
            }

            if (notify && changed) _onChange?.Invoke(_selectedIndex);
        }

        private int Count =>
            _style == UguiToggleGroupStyle.Checkbox ? _toggles.Count : _buttons.Count;

        private void RefreshButtonHighlight()
        {

            var normalColor = _theme.Palette.ButtonColors.normalColor;
            var selectedColor = _theme.Palette.ToggleSelectedTint;

            for (int i = 0; i < _buttonImages.Count; i++)
            {
                var img = _buttonImages[i];
                if (img == null) continue;
                img.color = (i == _selectedIndex) ? selectedColor : normalColor;
            }
        }
    }
}
