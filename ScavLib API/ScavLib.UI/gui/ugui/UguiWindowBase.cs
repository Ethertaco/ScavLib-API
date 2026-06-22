using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Image = UnityEngine.UI.Image;
using Input = UnityEngine.Input;

namespace ScavLib.gui.ugui
{
    public abstract class UguiWindowBase
    {
        private static readonly List<UguiWindowBase> _all = new List<UguiWindowBase>();

        public static void Register(UguiWindowBase window)
        {
            _all.Add(window);
            ScavLibPlugin.Log.LogInfo($"[ScavLib.Ugui.WindowBase] Registered window: '{window.Title}'");
        }

        internal static void UpdateAll()
        {
            foreach (var w in _all)
                if (w.ToggleKey != KeyCode.None && Input.GetKeyDown(w.ToggleKey))
                    w.Toggle();
        }

        internal static bool AnyBlockingWindowVisible()
        {
            foreach (var w in _all)
                if (w.IsVisible && w.BlockGameInput)
                    return true;
            return false;
        }

        public abstract string Title { get; }
        public virtual KeyCode ToggleKey => KeyCode.None;

        public virtual float Width => Theme.Metrics.WindowDefaultSize.x;
        public virtual float Height => Theme.Metrics.WindowDefaultSize.y;

        public virtual bool ShowInMenu => false;
        public virtual UguiTheme Theme => UguiTheme.Default;

        public virtual bool DefaultDraggable => true;
        public virtual bool DefaultBlockGameInput => true;

        public virtual bool AutoSizeHeight => true;
        public virtual bool AutoSizeWidth => false;

        public virtual bool DefaultShowCloseButton => true;

        public bool BlockGameInput { get; set; }
        public bool Draggable { get; private set; }

        protected abstract void Build(UguiBuilder builder);
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        protected virtual void OnCloseClicked() { Hide(); }

        public RectTransform Root { get; private set; }
        public bool IsVisible { get; private set; }
        private bool _built;
        private UguiDragHandler _dragHandler;
        private RectTransform _contentRect;

        public void Show()
        {
            EnsureBuilt();
            if (Root != null)
            {
                Root.gameObject.SetActive(true);
                ForceRebuildIfAutoSize();
            }
            IsVisible = true;
            OnShow();

            UguiOverlayLayer.BringToFront();
        }

        public void Hide()
        {
            if (Root != null) Root.gameObject.SetActive(false);
            IsVisible = false;
            OnHide();
        }

        public void Toggle()
        {
            if (IsVisible) Hide();
            else Show();
        }

        private void ForceRebuildIfAutoSize()
        {
            if (!AutoSizeHeight && !AutoSizeWidth) return;
            if (_contentRect != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
            if (Root != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(Root);
        }

        public void SetDraggable(bool value)
        {
            Draggable = value;
            if (Root == null) return;

            if (value)
            {
                if (_dragHandler == null)
                {
                    _dragHandler = Root.gameObject.AddComponent<UguiDragHandler>();
                    _dragHandler.Target = Root;
                }
            }
            else
            {
                if (_dragHandler != null)
                {
                    Object.Destroy(_dragHandler);
                    _dragHandler = null;
                }
            }
        }

        private void EnsureBuilt()
        {
            if (_built) return;

            var host = UguiHost.Instance;
            if (host == null)
            {
                ScavLibPlugin.Log.LogError($"[ScavLib.Ugui.WindowBase] Cannot build '{Title}' — UguiHost not ready.");
                return;
            }

            UguiFontManager.EnsurePrimaryResolved();

            var parent = host.UiRoot;
            if (parent == null)
            {
                ScavLibPlugin.Log.LogError($"[ScavLib.Ugui.WindowBase] Cannot build '{Title}' — UiRoot not ready.");
                return;
            }

            _built = true;
            var theme = Theme ?? UguiTheme.Default;
            var font = UguiFontManager.PrimaryFont;
            var m = theme.Metrics;

            bool showTitle = !m.HideTitle && !string.IsNullOrEmpty(Title);

            BlockGameInput = DefaultBlockGameInput;
            Draggable = DefaultDraggable;

            var panelGo = new GameObject($"Window_{Title}");
            panelGo.layer = LayerMask.NameToLayer("UI");
            panelGo.transform.SetParent(parent, false);

            Root = panelGo.AddComponent<RectTransform>();
            Root.sizeDelta = new Vector2(Width, Height);
            Root.anchoredPosition = Vector2.zero;
            Root.anchorMin = new Vector2(0.5f, 0.5f);
            Root.anchorMax = new Vector2(0.5f, 0.5f);
            Root.pivot = new Vector2(0.5f, 0.5f);

            var bg = panelGo.AddComponent<Image>();
            var panelSprite = ResourceLookupCache.FindSpriteAny(theme.Sprites.Panel);
            if (panelSprite != null)
            {
                bg.sprite = panelSprite;
                bg.type = Image.Type.Sliced;
                bg.color = theme.Palette.PanelTint;
            }
            else
            {
                bg.color = theme.Palette.PanelFallbackBg;
                BuildBorder(panelGo, theme.Fallback.BorderThickness, theme.Fallback.BorderInset,
                    theme.Palette.BorderColor);
            }

            if (showTitle)
            {
                var headerGo = new GameObject("Header");
                headerGo.layer = LayerMask.NameToLayer("UI");
                headerGo.transform.SetParent(panelGo.transform, false);
                var headerRect = headerGo.AddComponent<RectTransform>();
                headerRect.anchorMin = new Vector2(0f, 1f);
                headerRect.anchorMax = new Vector2(1f, 1f);
                headerRect.pivot = new Vector2(0.5f, 1f);
                headerRect.offsetMin = new Vector2(0f, -m.HeaderHeight);
                headerRect.offsetMax = Vector2.zero;
                var headerBg = headerGo.AddComponent<Image>();
                headerBg.color = Color.clear;

                bool showClose = DefaultShowCloseButton && m.ShowCloseButton;

                var titleGo = new GameObject("TitleText");
                titleGo.layer = LayerMask.NameToLayer("UI");
                titleGo.transform.SetParent(headerGo.transform, false);
                var titleRect = titleGo.AddComponent<RectTransform>();
                titleRect.anchorMin = Vector2.zero;
                titleRect.anchorMax = Vector2.one;

                bool leftAligned = m.TitleAlignment == UguiTheme.TitleAlign.Left;
                float padLeft = leftAligned ? m.TitlePadLeft : 0f;

                float closeReserve = showClose ? (m.CloseButtonSize + m.CloseButtonRightPad * 2f) : 0f;
                float padRight = (leftAligned ? m.TitlePadRight : 0f) + closeReserve;

                titleRect.offsetMin = new Vector2(padLeft, m.TitlePadBottom);
                titleRect.offsetMax = new Vector2(-padRight, -m.TitlePadTop);

                var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
                titleTmp.text = Title;
                titleTmp.fontSize = theme.Typography.TitleFontSize;
                titleTmp.color = theme.Palette.TextPrimary;
                titleTmp.alignment = MapTitleAlignment(m.TitleAlignment);
                titleTmp.raycastTarget = false;
                if (font != null) titleTmp.font = font;

                if (showClose)
                    BuildCloseButton(headerGo, theme, font);
            }

            var contentGo = new GameObject("Content");
            contentGo.layer = LayerMask.NameToLayer("UI");
            contentGo.transform.SetParent(panelGo.transform, false);
            var contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = m.ContentPadMin;
            contentRect.offsetMax = showTitle
                ? m.ContentPadMax
                : new Vector2(m.ContentPadMax.x, -m.ContentPadMin.y);
            _contentRect = contentRect;

            if (Draggable)
            {
                _dragHandler = panelGo.AddComponent<UguiDragHandler>();
                _dragHandler.Target = Root;
            }

            var builder = new UguiBuilder(contentRect, font, theme);
            Build(builder);

            if (AutoSizeHeight || AutoSizeWidth)
                ApplyAutoSize(contentRect, theme, showTitle);

            panelGo.SetActive(false);
            ScavLibPlugin.Log.LogInfo($"[ScavLib.Ugui.WindowBase] Built window: '{Title}'");
        }

        private void BuildCloseButton(GameObject headerGo, UguiTheme theme, TMP_FontAsset font)
        {
            var m = theme.Metrics;

            var go = new GameObject("CloseButton");
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(headerGo.transform, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.sizeDelta = new Vector2(m.CloseButtonSize, m.CloseButtonSize);
            rect.anchoredPosition = new Vector2(-m.CloseButtonRightPad, 0f);

            var img = go.AddComponent<Image>();
            UguiStyleApplier.ApplyNineSliceOrOutline(go, img, theme.Sprites.Button, Color.black, theme);
            img.raycastTarget = true;

            var btn = go.AddComponent<Button>();
            btn.colors = theme.Palette.ButtonColors;
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => OnCloseClicked());

            var textGo = new GameObject("X");
            textGo.layer = LayerMask.NameToLayer("UI");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "X";
            tmp.fontSize = theme.Typography.TitleFontSize;
            tmp.color = theme.Palette.CloseButtonTint;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            if (font != null) tmp.font = font;
        }

        private void ApplyAutoSize(RectTransform contentRect, UguiTheme theme, bool showTitle)
        {
            var fitter = contentRect.gameObject.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = contentRect.gameObject.AddComponent<ContentSizeFitter>();

            fitter.verticalFit = AutoSizeHeight
                ? ContentSizeFitter.FitMode.PreferredSize
                : ContentSizeFitter.FitMode.Unconstrained;
            fitter.horizontalFit = AutoSizeWidth
                ? ContentSizeFitter.FitMode.PreferredSize
                : ContentSizeFitter.FitMode.Unconstrained;

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

            var m = theme.Metrics;
            float padX = m.ContentPadMin.x + Mathf.Abs(m.ContentPadMax.x);
            float padTop = showTitle ? Mathf.Abs(m.ContentPadMax.y) : m.ContentPadMin.y;
            float padBottom = m.ContentPadMin.y;
            float padY = padTop + padBottom;

            float targetW = Width;
            float targetH = Height;

            if (AutoSizeHeight)
            {
                float contentH = LayoutUtility.GetPreferredHeight(contentRect);
                targetH = Mathf.Clamp(contentH + padY, m.AutoSizeMinHeight, m.AutoSizeMaxHeight);
            }
            if (AutoSizeWidth)
            {
                float contentW = LayoutUtility.GetPreferredWidth(contentRect);
                targetW = contentW + padX;
            }

            Root.sizeDelta = new Vector2(targetW, targetH);
        }

        private static TextAlignmentOptions MapTitleAlignment(UguiTheme.TitleAlign align)
        {
            switch (align)
            {
                case UguiTheme.TitleAlign.Center: return TextAlignmentOptions.Center;
                case UguiTheme.TitleAlign.Right: return TextAlignmentOptions.MidlineRight;
                default: return TextAlignmentOptions.MidlineLeft;
            }
        }

        private static void BuildBorder(GameObject parent, float t, float inset, Color color)
        {
            MakeBorderBar(parent, "Border_Top", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(inset, -t), new Vector2(-inset, 0f), color);
            MakeBorderBar(parent, "Border_Bottom", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(inset, 0f), new Vector2(-inset, t), color);
            MakeBorderBar(parent, "Border_Left", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, inset), new Vector2(t, -inset), color);
            MakeBorderBar(parent, "Border_Right", new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-t, inset), new Vector2(0f, -inset), color);

            MakeBorderBar(parent, "Corner_TL_H", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, -t), new Vector2(inset, 0f), color);
            MakeBorderBar(parent, "Corner_TL_V", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, -(inset + t)), new Vector2(t, -t), color);
            MakeBorderBar(parent, "Corner_TR_H", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-inset, -t), new Vector2(0f, 0f), color);
            MakeBorderBar(parent, "Corner_TR_V", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-t, -(inset + t)), new Vector2(0f, -t), color);
            MakeBorderBar(parent, "Corner_BL_H", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(inset, t), color);
            MakeBorderBar(parent, "Corner_BL_V", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, t), new Vector2(t, inset + t), color);
            MakeBorderBar(parent, "Corner_BR_H", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-inset, 0f), new Vector2(0f, t), color);
            MakeBorderBar(parent, "Corner_BR_V", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-t, t), new Vector2(0f, inset + t), color);
        }

        private static void MakeBorderBar(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(parent.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            var img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
        }
    }

    internal class UguiDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        internal RectTransform Target;
        private Vector2 _dragOffset;

        public void OnBeginDrag(PointerEventData e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Target.parent as RectTransform, e.position, e.pressEventCamera, out _dragOffset);
            _dragOffset = Target.anchoredPosition - _dragOffset;
        }

        public void OnDrag(PointerEventData e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Target.parent as RectTransform, e.position, e.pressEventCamera, out Vector2 local);
            Target.anchoredPosition = local + _dragOffset;
        }
    }
}
