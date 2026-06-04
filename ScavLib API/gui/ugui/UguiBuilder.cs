using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;
using USlider = UnityEngine.UI.Slider;
using UToggle = UnityEngine.UI.Toggle;

namespace ScavLib.gui.ugui
{
    public class UguiBuilder
    {
        private readonly RectTransform _content;
        private readonly UguiTheme _theme;
        private TMP_FontAsset _font;
        private readonly List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
        private readonly VerticalLayoutGroup _vlayout;

        public UguiBuilder(RectTransform content, TMP_FontAsset font, UguiTheme theme = null)
        {
            _content = content;
            _theme = theme ?? UguiTheme.Default;
            _font = font ?? UguiFontManager.PrimaryFont;
            _vlayout = EnsureVerticalLayout(_content, _theme);
        }

        internal static VerticalLayoutGroup EnsureVerticalLayout(RectTransform content, UguiTheme theme)
        {
            var vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = theme.Metrics.RowSpacing;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(0, 0, 0, 0);
            return vlg;
        }

        internal void ApplyFont(TMP_FontAsset font)
        {
            _font = font;
            foreach (var t in _texts)
                if (t != null) t.font = font;
        }

        public TextMeshProUGUI AddLabel(string text)
        {
            var go = MakeRow("Label", _theme.Metrics.RowHeight);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = _theme.Typography.BodyFontSize;
            tmp.color = _theme.Palette.TextPrimary;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            SetFont(tmp);
            return tmp;
        }

        public Button AddButton(string label, Action onClick)
        {
            var go = MakeRow("Button", _theme.Metrics.RowHeight);
            var img = go.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(go, img, _theme.Sprites.Button, Color.black, _theme);

            var btn = go.AddComponent<Button>();
            btn.colors = _theme.Palette.ButtonColors;
            btn.targetGraphic = img;
            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var textGo = MakeChild(go, "Text");
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(_theme.Metrics.ButtonTextInset, 0f);
            textRect.offsetMax = new Vector2(-_theme.Metrics.ButtonTextInset, 0f);

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = _theme.Typography.BodyFontSize;
            tmp.color = _theme.Palette.TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            SetFont(tmp);

            return btn;
        }

        public UguiHorizontalRow BeginHorizontal(float rowHeight = -1f)
        {
            if (rowHeight < 0f) rowHeight = _theme.Metrics.SmallButtonHeight;

            var rowGo = MakeRow("HRow", rowHeight);
            var hlg = rowGo.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = _theme.Metrics.HorizontalSpacing;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            var rowRect = rowGo.GetComponent<RectTransform>();
            float rowWidth = _content.rect.width;
            if (rowWidth <= 1f) rowWidth = Mathf.Abs(_content.sizeDelta.x);

            return new UguiHorizontalRow(
                rowRect, rowWidth, rowHeight, _font, _theme,
                commitRowHeight: _ => { });
        }

        public UguiTabView BeginTabs()
        {
            var go = MakeRow("TabView", _theme.Metrics.TabHeaderHeight);
            var rect = go.GetComponent<RectTransform>();
            return new UguiTabView(go, rect, _font, _theme);
        }

        public TMP_InputField AddInputField(
            string initialText = "",
            string placeholder = "",
            Action<string> onValueChanged = null,
            Action<string> onEndEdit = null)
        {
            var go = MakeRow("InputField", _theme.Metrics.InputFieldHeight);
            var bg = go.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(go, bg, _theme.Sprites.Button,
                _theme.Palette.InputFieldBg, _theme);
            if (bg.sprite != null) bg.color = _theme.Palette.InputFieldBg;

            var input = go.AddComponent<TMP_InputField>();
            input.targetGraphic = bg;
            input.lineType = TMP_InputField.LineType.SingleLine;

            var areaGo = MakeChild(go, "TextArea");
            var areaRect = areaGo.GetComponent<RectTransform>();
            areaRect.anchorMin = Vector2.zero;
            areaRect.anchorMax = Vector2.one;
            areaRect.offsetMin = new Vector2(_theme.Metrics.InputFieldPadding, 0f);
            areaRect.offsetMax = new Vector2(-_theme.Metrics.InputFieldPadding, 0f);
            var areaMask = areaGo.AddComponent<RectMask2D>();

            var phGo = MakeChild(areaGo, "Placeholder");
            var phRect = phGo.GetComponent<RectTransform>();
            phRect.anchorMin = Vector2.zero;
            phRect.anchorMax = Vector2.one;
            phRect.offsetMin = Vector2.zero;
            phRect.offsetMax = Vector2.zero;
            var phTmp = phGo.AddComponent<TextMeshProUGUI>();
            phTmp.text = placeholder;
            phTmp.fontSize = _theme.Typography.BodyFontSize;
            phTmp.color = _theme.Palette.InputPlaceholder;
            phTmp.alignment = TextAlignmentOptions.MidlineLeft;
            phTmp.raycastTarget = false;
            SetFont(phTmp);

            var textGo = MakeChild(areaGo, "Text");
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var textTmp = textGo.AddComponent<TextMeshProUGUI>();
            textTmp.fontSize = _theme.Typography.BodyFontSize;
            textTmp.color = _theme.Palette.TextPrimary;
            textTmp.alignment = TextAlignmentOptions.MidlineLeft;
            SetFont(textTmp);

            input.textViewport = areaRect;
            input.textComponent = textTmp;
            input.placeholder = phTmp;
            input.text = initialText;

            if (onValueChanged != null) input.onValueChanged.AddListener(v => onValueChanged(v));
            if (onEndEdit != null) input.onEndEdit.AddListener(v => onEndEdit(v));

            return input;
        }

        public UguiNumberField AddNumberField(
            float initial = 0f, float step = 1f, float min = float.MinValue, float max = float.MaxValue,
            bool isInteger = false, Action<float> onChange = null)
        {
            var rowGo = MakeRow("NumberField", _theme.Metrics.InputFieldHeight);
            var hlg = rowGo.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = _theme.Metrics.HorizontalSpacing;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            var field = rowGo.AddComponent<UguiNumberField>();

            BuildStepperButton(rowGo, "-", () => field.Step(-1));

            var inputGo = MakeChild(rowGo, "Input");
            var inputLe = inputGo.AddComponent<LayoutElement>();
            inputLe.flexibleWidth = 1f;
            inputLe.preferredHeight = _theme.Metrics.InputFieldHeight;
            var inputBg = inputGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(inputGo, inputBg, _theme.Sprites.Button,
                _theme.Palette.InputFieldBg, _theme);
            if (inputBg.sprite != null) inputBg.color = _theme.Palette.InputFieldBg;

            var input = inputGo.AddComponent<TMP_InputField>();
            input.targetGraphic = inputBg;
            input.lineType = TMP_InputField.LineType.SingleLine;

            var areaGo = MakeChild(inputGo, "TextArea");
            var areaRect = areaGo.GetComponent<RectTransform>();
            areaRect.anchorMin = Vector2.zero;
            areaRect.anchorMax = Vector2.one;
            areaRect.offsetMin = new Vector2(_theme.Metrics.InputFieldPadding, 0f);
            areaRect.offsetMax = new Vector2(-_theme.Metrics.InputFieldPadding, 0f);
            areaGo.AddComponent<RectMask2D>();

            var textGo = MakeChild(areaGo, "Text");
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var textTmp = textGo.AddComponent<TextMeshProUGUI>();
            textTmp.fontSize = _theme.Typography.BodyFontSize;
            textTmp.color = _theme.Palette.TextPrimary;
            textTmp.alignment = TextAlignmentOptions.Center;
            SetFont(textTmp);

            input.textViewport = areaRect;
            input.textComponent = textTmp;

            BuildStepperButton(rowGo, "+", () => field.Step(1));

            field.Init(input, initial, step, min, max, isInteger, onChange);
            return field;
        }

        private void BuildStepperButton(GameObject parent, string label, Action onClick)
        {
            var go = new GameObject($"Step_{label}");
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(parent.transform, false);
            go.AddComponent<RectTransform>();
            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = _theme.Metrics.StepperButtonWidth;
            le.flexibleWidth = 0f;

            var img = go.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(go, img, _theme.Sprites.Button, Color.black, _theme);
            img.raycastTarget = true;

            var btn = go.AddComponent<Button>();
            btn.colors = _theme.Palette.ButtonColors;
            btn.targetGraphic = img;
            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var textGo = MakeChild(go, "Text");
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = _theme.Typography.BodyFontSize;
            tmp.color = _theme.Palette.TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            SetFont(tmp);
        }

        public UToggle AddToggle(string label, bool initialValue, Action<bool> onChange = null)
        {
            var go = MakeRow("Toggle", _theme.Metrics.RowHeight);
            var img = go.AddComponent<UImage>();
            img.color = Color.clear;

            var toggle = go.AddComponent<UToggle>();
            toggle.isOn = initialValue;
            if (onChange != null) toggle.onValueChanged.AddListener(v => onChange(v));

            var boxGo = MakeChild(go, "Box");
            var boxRect = boxGo.GetComponent<RectTransform>();
            boxRect.anchorMin = new Vector2(0f, 0.5f);
            boxRect.anchorMax = new Vector2(0f, 0.5f);
            boxRect.pivot = new Vector2(0f, 0.5f);
            boxRect.anchoredPosition = Vector2.zero;
            boxRect.sizeDelta = new Vector2(_theme.Metrics.ToggleBoxSize, _theme.Metrics.ToggleBoxSize);

            var boxImg = boxGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(boxGo, boxImg, _theme.Sprites.Box, Color.black, _theme);

            var checkGo = MakeChild(boxGo, "Checkmark");
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

            var textGo = MakeChild(go, "Label");
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(_theme.Metrics.ToggleLabelIndent, 0f);
            textRect.offsetMax = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = _theme.Typography.BodyFontSize;
            tmp.color = _theme.Palette.TextPrimary;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.raycastTarget = false;
            SetFont(tmp);

            return toggle;
        }

        public USlider AddSlider(string label, float min, float max, float initialValue, Action<float> onChange = null)
        {
            var go = MakeRow("Slider", _theme.Metrics.SliderRowHeight);

            var headerGo = MakeChild(go, "Header");
            var headerRect = headerGo.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 0.5f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;

            var labelTmp = headerGo.AddComponent<TextMeshProUGUI>();
            labelTmp.fontSize = _theme.Typography.SliderLabelFontSize;
            labelTmp.color = _theme.Palette.TextPrimary;
            labelTmp.alignment = TextAlignmentOptions.MidlineLeft;
            labelTmp.raycastTarget = false;
            SetFont(labelTmp);

            var trackGo = MakeChild(go, "Track");
            var trackRect = trackGo.GetComponent<RectTransform>();
            trackRect.anchorMin = new Vector2(0f, 0f);
            trackRect.anchorMax = new Vector2(1f, _theme.Metrics.SliderTrackTop);
            trackRect.offsetMin = _theme.Metrics.SliderTrackPadMin;
            trackRect.offsetMax = _theme.Metrics.SliderTrackPadMax;

            var trackImg = trackGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(trackGo, trackImg, _theme.Sprites.Button,
                Color.black, _theme, _theme.Palette.SliderTrackOutline);
            if (trackImg.sprite != null)
                trackImg.color = _theme.Palette.SliderTrackTint;

            var fillAreaGo = MakeChild(trackGo, "FillArea");
            var fillAreaRect = fillAreaGo.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0f);
            fillAreaRect.anchorMax = new Vector2(1f, 1f);
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            var fillGo = MakeChild(fillAreaGo, "Fill");
            var fillRect = fillGo.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImg = fillGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(fillGo, fillImg, _theme.Sprites.Fill,
                Color.black, _theme, _theme.Palette.SliderFillTint);
            if (fillImg.sprite != null)
                fillImg.color = _theme.Palette.SliderFillTint;

            var handleAreaGo = MakeChild(trackGo, "HandleArea");
            var handleAreaRect = handleAreaGo.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(_theme.Metrics.SliderHandlePad, 0f);
            handleAreaRect.offsetMax = new Vector2(-_theme.Metrics.SliderHandlePad, 0f);

            var handleGo = MakeChild(handleAreaGo, "Handle");
            var handleRect = handleGo.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(_theme.Metrics.SliderHandleWidth, 0f);
            var handleImg = handleGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(handleGo, handleImg, _theme.Sprites.Button,
                Color.black, _theme, Color.white);

            var slider = trackGo.AddComponent<USlider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImg;
            slider.direction = USlider.Direction.LeftToRight;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = Mathf.Clamp(initialValue, min, max);

            labelTmp.text = $"{label}: {slider.value:F1}";
            slider.onValueChanged.AddListener(v =>
            {
                labelTmp.text = $"{label}: {v:F1}";
                onChange?.Invoke(v);
            });

            return slider;
        }

        public UguiPixelBar AddProgressBar(float initial01 = 0f)
        {
            var go = MakeRow("ProgressBar", _theme.Metrics.RowHeight * _theme.Metrics.ProgressBarHeightRatio);
            var img = go.AddComponent<UImage>();
            var sprite = ResourceLookupCache.FindSpriteAny(_theme.Sprites.Fill);
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = UImage.Type.Filled;
                img.fillMethod = UImage.FillMethod.Horizontal;
                img.fillOrigin = 0;
                img.color = Color.white;
            }
            else
            {
                img.type = UImage.Type.Filled;
                img.fillMethod = UImage.FillMethod.Horizontal;
                img.color = Color.white;
            }

            var bar = go.AddComponent<UguiPixelBar>();
            bar.image = img;
            bar.Fill = Mathf.Clamp01(initial01);
            return bar;
        }

        public UImage AddImage(Sprite sprite, float pixelSize = 3f, float? maxSize = null)
        {
            if (sprite == null || sprite.texture == null) return null;

            float maxS = maxSize ?? _theme.Metrics.ImageDefaultMaxSize;
            var size = PlayerCamera.ImageSizeDelta(sprite.texture, pixelSize, maxS);

            var go = MakeRow("Image", size.y);
            var img = go.AddComponent<UImage>();
            img.sprite = sprite;
            img.color = Color.white;
            img.raycastTarget = false;
            img.preserveAspect = true;

            var inner = MakeChild(go, "Img");
            var innerRect = inner.GetComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.5f, 0.5f);
            innerRect.anchorMax = new Vector2(0.5f, 0.5f);
            innerRect.pivot = new Vector2(0.5f, 0.5f);
            innerRect.sizeDelta = size;
            var innerImg = inner.AddComponent<UImage>();
            innerImg.sprite = sprite;
            innerImg.color = Color.white;
            innerImg.raycastTarget = false;
            innerImg.preserveAspect = true;

            img.enabled = false;

            return innerImg;
        }

        public UImage AddImage(string spriteName, float pixelSize = 3f, float? maxSize = null)
        {
            var sprite = ResourceLookupCache.FindSpriteAny(spriteName);
            return AddImage(sprite, pixelSize, maxSize);
        }

        public UguiDropdown AddDropdown(
            IList<string> options, int initialIndex = 0, Action<int> onChange = null)
        {
            var opts = new List<string>();
            if (options != null) opts.AddRange(options);

            var go = MakeRow("Dropdown", _theme.Metrics.RowHeight);
            var bodyRect = go.GetComponent<RectTransform>();

            var img = go.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(go, img, _theme.Sprites.Button, Color.black, _theme);

            img.raycastTarget = true;

            var colors = _theme.Palette.ButtonColors;
            var normalColor = colors.normalColor;
            var highlightColor = colors.highlightedColor;
            var pressedColor = colors.pressedColor;

            img.color = normalColor;

            UguiDropdown dropdown = null;
            var trigger = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            void AddEntry(UnityEngine.EventSystems.EventTriggerType type,
                          UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData> cb)
            {
                var e = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = type };
                e.callback.AddListener(cb);
                trigger.triggers.Add(e);
            }

            AddEntry(UnityEngine.EventSystems.EventTriggerType.PointerEnter,
                _ => img.color = highlightColor);

            AddEntry(UnityEngine.EventSystems.EventTriggerType.PointerExit,
                _ => img.color = normalColor);

            AddEntry(UnityEngine.EventSystems.EventTriggerType.PointerDown,
                _ => img.color = pressedColor);

            AddEntry(UnityEngine.EventSystems.EventTriggerType.PointerUp,
                _ => img.color = highlightColor);

            AddEntry(UnityEngine.EventSystems.EventTriggerType.PointerClick,
                _ =>
                {
                    if (dropdown != null)
                    {
                        try
                        {
                            dropdown.ToggleExpand();
                        }
                        catch (System.Exception ex)
                        {

                            ScavLibPlugin.Log.LogError(
                                "[ScavLib.Ugui.Dropdown] ToggleExpand threw: " + ex);
                        }
                    }
                });

            var labelGo = MakeChild(go, "Label");
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(_theme.Metrics.ButtonTextInset, 0f);
            labelRect.offsetMax = new Vector2(
                -(_theme.Metrics.DropdownArrowWidth + _theme.Metrics.ButtonTextInset), 0f);

            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.fontSize = _theme.Typography.BodyFontSize;
            labelTmp.color = _theme.Palette.TextPrimary;
            labelTmp.alignment = TextAlignmentOptions.MidlineLeft;
            labelTmp.enableWordWrapping = false;
            labelTmp.overflowMode = TextOverflowModes.Ellipsis;
            labelTmp.raycastTarget = false;
            SetFont(labelTmp);

            var arrowGo = MakeChild(go, "Arrow");
            var arrowRect = arrowGo.GetComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1f, 0.5f);
            arrowRect.anchorMax = new Vector2(1f, 0.5f);
            arrowRect.pivot = new Vector2(1f, 0.5f);
            arrowRect.sizeDelta = new Vector2(
                _theme.Metrics.DropdownArrowWidth, _theme.Metrics.RowHeight);
            arrowRect.anchoredPosition =
                new Vector2(-_theme.Metrics.ButtonTextInset, 0f);

            var arrowTmp = arrowGo.AddComponent<TextMeshProUGUI>();
            arrowTmp.text = "\u25BC";
            arrowTmp.fontSize = _theme.Typography.BodyFontSize;
            arrowTmp.color = _theme.Palette.DropdownArrowTint;
            arrowTmp.alignment = TextAlignmentOptions.Center;
            arrowTmp.raycastTarget = false;
            SetFont(arrowTmp);

            dropdown = new UguiDropdown(
                bodyRect, labelTmp, opts, initialIndex, _font, _theme, onChange);

            return dropdown;
        }

        public UguiToggleGroup AddToggleGroup(
            IList<string> options,
            int initialIndex = 0,
            UguiToggleGroupStyle style = UguiToggleGroupStyle.Checkbox,
            Action<int> onChange = null)
        {
            var opts = new List<string>();
            if (options != null) opts.AddRange(options);

            var group = new UguiToggleGroup(style, _font, _theme, onChange);

            Func<string, float, GameObject> makeRow = (name, h) => MakeRow(name, h);
            Func<GameObject, string, GameObject> makeChild = (parent, name) => MakeChild(parent, name);

            if (style == UguiToggleGroupStyle.Checkbox)
                group.BuildCheckbox(this, _content, opts, initialIndex, makeRow, makeChild);
            else
                group.BuildButtons(this, _content, opts, initialIndex, makeRow, makeChild);

            return group;
        }

        public void AddSeparator()
        {
            MakeRow("Separator", _theme.Metrics.SeparatorHeight);
        }

        public void AddSpace(float pixels = -1f)
        {
            if (pixels < 0f) pixels = _theme.Metrics.DefaultSpace;
            MakeRow("Space", pixels);
        }

        public (ScrollRect scrollRect, RectTransform scrollContent) AddScrollView(float height)
        {
            var go = MakeRow("ScrollView", height);
            var viewportImg = go.AddComponent<UImage>();
            viewportImg.color = Color.clear;
            var mask = go.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            var contentGo = MakeChild(go, "Content");
            var contentRect = contentGo.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;

            var scrollRect = go.AddComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = go.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.scrollSensitivity = _theme.Metrics.ScrollSensitivity;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            var sbWidth = _theme.Metrics.ScrollbarWidth;

            var sbGo = MakeChild(go, "Scrollbar");
            var sbRect = sbGo.GetComponent<RectTransform>();
            sbRect.anchorMin = new Vector2(1f, 0f);
            sbRect.anchorMax = new Vector2(1f, 1f);
            sbRect.pivot = new Vector2(1f, 0.5f);
            sbRect.sizeDelta = new Vector2(sbWidth, 0f);
            sbRect.anchoredPosition = Vector2.zero;

            var sbImg = sbGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(sbGo, sbImg, _theme.Sprites.Button,
                _theme.Palette.SliderTrackTint, _theme, _theme.Palette.SliderTrackOutline);
            if (sbImg.sprite != null) sbImg.color = _theme.Palette.SliderTrackTint;

            var slidingGo = MakeChild(sbGo, "SlidingArea");
            var slidingRect = slidingGo.GetComponent<RectTransform>();
            slidingRect.anchorMin = Vector2.zero;
            slidingRect.anchorMax = Vector2.one;
            slidingRect.offsetMin = Vector2.zero;
            slidingRect.offsetMax = Vector2.zero;

            var handleGo = MakeChild(slidingGo, "Handle");
            var handleRect = handleGo.GetComponent<RectTransform>();
            handleRect.anchorMin = Vector2.zero;
            handleRect.anchorMax = Vector2.one;
            handleRect.offsetMin = Vector2.zero;
            handleRect.offsetMax = Vector2.zero;
            var handleImg = handleGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(handleGo, handleImg, _theme.Sprites.Button,
                Color.black, _theme, _theme.Palette.SliderFillTint);
            if (handleImg.sprite != null) handleImg.color = _theme.Palette.SliderFillTint;

            var scrollbar = sbGo.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImg;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarSpacing = 0f;

            return (scrollRect, contentRect);
        }

        private GameObject MakeRow(string name, float rowHeight)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(_content, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = rowHeight;
            le.flexibleHeight = 0f;

            return go;
        }

        private static GameObject MakeChild(GameObject parent, string name)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(parent.transform, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        private void SetFont(TextMeshProUGUI tmp)
        {
            var f = _font ?? UguiFontManager.PrimaryFont;
            if (f != null) tmp.font = f;
            _texts.Add(tmp);
        }
    }
}
