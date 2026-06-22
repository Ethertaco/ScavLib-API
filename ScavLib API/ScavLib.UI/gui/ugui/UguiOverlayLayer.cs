using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UImage = UnityEngine.UI.Image;

namespace ScavLib.gui.ugui
{

    public static class UguiOverlayLayer
    {
        private static bool _initialized;
        private static RectTransform _overlayRoot;
        private static UguiOverlayDriver _driver;
        private static UguiTooltipOverlay _tooltip;

        internal static UguiDropdown OpenDropdown;

        public static RectTransform OverlayRoot
        {
            get
            {
                EnsureInitialized();
                return _overlayRoot;
            }
        }

        internal static UguiTooltipOverlay Tooltip
        {
            get
            {
                EnsureInitialized();
                return _tooltip;
            }
        }

        public static void EnsureInitialized()
        {

            if (_initialized && _overlayRoot != null && _driver != null) return;

            var host = UguiHost.Instance;
            if (host == null || host.UiRoot == null)
            {

                return;
            }

            if (_overlayRoot == null)
            {
                var go = new GameObject("ScavLibUgui_OverlayLayer");
                go.layer = LayerMask.NameToLayer("UI");
                _overlayRoot = go.AddComponent<RectTransform>();
                _overlayRoot.SetParent(host.UiRoot, false);
                _overlayRoot.anchorMin = Vector2.zero;
                _overlayRoot.anchorMax = Vector2.one;
                _overlayRoot.offsetMin = Vector2.zero;
                _overlayRoot.offsetMax = Vector2.zero;
                _overlayRoot.localScale = Vector3.one;
                _overlayRoot.SetAsLastSibling();
            }

            if (_driver == null)
            {
                _driver = _overlayRoot.gameObject.AddComponent<UguiOverlayDriver>();
            }

            if (_tooltip == null)
            {
                _tooltip = new UguiTooltipOverlay(_overlayRoot, UguiTheme.Default);
            }

            _initialized = true;

            ScavLibPlugin.Log.LogInfo("[ScavLib.Ugui.OverlayLayer] Initialized.");
        }

        internal static void BringToFront()
        {
            if (_overlayRoot != null) _overlayRoot.SetAsLastSibling();
        }

        internal static void CloseOpenDropdown()
        {
            if (OpenDropdown != null)
            {
                OpenDropdown.Collapse();
                OpenDropdown = null;
            }
        }

        internal static void RegisterOpenDropdown(UguiDropdown dd)
        {
            if (OpenDropdown != null && OpenDropdown != dd)
                OpenDropdown.Collapse();
            OpenDropdown = dd;
            BringToFront();
        }
    }

    internal sealed class UguiOverlayDriver : MonoBehaviour
    {
        private static readonly List<RaycastResult> _empty = new List<RaycastResult>();

        private void LateUpdate()
        {

            UguiOverlayLayer.BringToFront();

            if (ConsoleScript.instance != null && ConsoleScript.instance.active)
            {
                UguiOverlayLayer.CloseOpenDropdown();
                UguiOverlayLayer.Tooltip?.HideImmediate();
                return;
            }

            List<RaycastResult> casts;
            try
            {
                casts = UIUtil.GetEventSystemRaycastResults();
            }
            catch
            {
                casts = _empty;
            }

            var open = UguiOverlayLayer.OpenDropdown;
            if (open != null)
                open.HandleOutsideClick(casts);

            UguiOverlayLayer.Tooltip?.Tick(casts);
        }
    }

    public sealed class UguiTooltipMarker : MonoBehaviour
    {
        public string TipName;
        public string TipDesc;
    }

    internal sealed class UguiTooltipOverlay
    {
        private readonly UguiTheme _theme;
        private readonly RectTransform _overlayRoot;

        private GameObject _panelGo;
        private RectTransform _panelRect;
        private TextMeshProUGUI _nameTmp;
        private TextMeshProUGUI _descTmp;
        private bool _built;

        public UguiTooltipOverlay(RectTransform overlayRoot, UguiTheme theme)
        {
            _overlayRoot = overlayRoot;
            _theme = theme ?? UguiTheme.Default;
        }

        private void EnsureBuilt()
        {
            if (_built && _panelGo != null) return;

            var m = _theme.Metrics;

            _panelGo = new GameObject("Tooltip");
            _panelGo.layer = LayerMask.NameToLayer("UI");
            _panelGo.transform.SetParent(_overlayRoot, false);

            _panelRect = _panelGo.AddComponent<RectTransform>();

            _panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            _panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            _panelRect.pivot = new Vector2(0f, 1f);

            var bg = _panelGo.AddComponent<UImage>();
            UguiStyleApplier.ApplyNineSliceOrOutline(_panelGo, bg, _theme.Sprites.Panel,
                _theme.Palette.TooltipBg, _theme);
            if (bg.sprite != null) bg.color = _theme.Palette.TooltipBg;
            bg.raycastTarget = false;

            var vlg = _panelGo.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(
                (int)m.TooltipPadding, (int)m.TooltipPadding,
                (int)m.TooltipPadding, (int)m.TooltipPadding);
            vlg.spacing = 2f;
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;

            var fitter = _panelGo.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _nameTmp = MakeText("Name", _theme.Typography.BodyFontSize, FontStyles.Bold);
            _descTmp = MakeText("Desc", _theme.Typography.SliderLabelFontSize, FontStyles.Normal);

            _panelGo.SetActive(false);
            _built = true;
        }

        private TextMeshProUGUI MakeText(string name, float fontSize, FontStyles style)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(_panelGo.transform, false);
            go.AddComponent<RectTransform>();

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = fontSize;
            tmp.color = _theme.Palette.TextPrimary;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.fontStyle = style;
            tmp.enableWordWrapping = true;
            tmp.raycastTarget = false;

            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = -1f;
            le.flexibleWidth = 0f;

            tmp.rectTransform.sizeDelta = new Vector2(
                _theme.Metrics.TooltipMaxWidth - _theme.Metrics.TooltipPadding * 2f, 0f);

            var f = UguiFontManager.PrimaryFont;
            if (f != null) tmp.font = f;
            return tmp;
        }

        public void Tick(List<RaycastResult> casts)
        {
            EnsureBuilt();

            string tipName = null;
            string tipDesc = null;

            if (casts != null)
            {
                for (int i = 0; i < casts.Count; i++)
                {
                    var go = casts[i].gameObject;
                    if (go == null) continue;
                    var marker = go.GetComponent<UguiTooltipMarker>();
                    if (marker == null) continue;
                    if (!string.IsNullOrEmpty(marker.TipName) || !string.IsNullOrEmpty(marker.TipDesc))
                    {
                        tipName = marker.TipName;
                        tipDesc = marker.TipDesc;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(tipName) && string.IsNullOrEmpty(tipDesc))
            {
                if (_panelGo.activeSelf) _panelGo.SetActive(false);
                return;
            }

            bool hasName = !string.IsNullOrEmpty(tipName);
            bool hasDesc = !string.IsNullOrEmpty(tipDesc);
            _nameTmp.gameObject.SetActive(hasName);
            _descTmp.gameObject.SetActive(hasDesc);
            if (hasName) _nameTmp.text = tipName;
            if (hasDesc) _descTmp.text = tipDesc;

            if (!_panelGo.activeSelf) _panelGo.SetActive(true);
            _panelRect.SetAsLastSibling();

            FollowCursor();
        }

        private void FollowCursor()
        {
            var m = _theme.Metrics;
            Vector2 local;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _overlayRoot, Input.mousePosition, null, out local))
                return;

            local += m.TooltipCursorOffset;

            var size = _panelRect.rect.size;
            float halfW = _overlayRoot.rect.width * 0.5f;
            float halfH = _overlayRoot.rect.height * 0.5f;

            if (local.x + size.x > halfW)
                local.x = local.x - size.x - m.TooltipCursorOffset.x * 2f;

            if (local.y - size.y < -halfH)
                local.y = local.y + size.y - m.TooltipCursorOffset.y * 2f;

            _panelRect.anchoredPosition = local;
        }

        public void HideImmediate()
        {
            if (_built && _panelGo != null && _panelGo.activeSelf)
                _panelGo.SetActive(false);
        }
    }

    public sealed class UguiDropdown
    {
        private readonly UguiTheme _theme;
        private readonly TMP_FontAsset _font;
        private readonly RectTransform _bodyRect;
        private readonly TextMeshProUGUI _labelTmp;
        private readonly List<string> _options;
        private readonly Action<int> _onChange;

        private GameObject _panelGo;
        private RectTransform _panelRect;
        private readonly List<Button> _optionButtons = new List<Button>();
        private readonly List<UImage> _optionImages = new List<UImage>();
        private readonly List<Transform> _optionBorderRoots = new List<Transform>();

        private int _index;
        public int Index => _index;
        public string SelectedText =>
            (_index >= 0 && _index < _options.Count) ? _options[_index] : "";

        private static readonly Color PanelBg = new Color(0f, 0f, 0f, 0.92f);
        private static readonly Color OptionBg = new Color(0f, 0f, 0f, 0.92f);
        private static readonly Color BorderNormal = Color.white;
        private static readonly Color BorderHover = new Color(0.45f, 0.45f, 0.45f, 1f);
        private static readonly Color BorderSelected = new Color(0.75f, 0.75f, 0.75f, 1f);

        internal UguiDropdown(
            RectTransform bodyRect, TextMeshProUGUI labelTmp,
            List<string> options, int initialIndex,
            TMP_FontAsset font, UguiTheme theme, Action<int> onChange)
        {
            _bodyRect = bodyRect;
            _labelTmp = labelTmp;
            _options = options ?? new List<string>();
            _font = font ?? UguiFontManager.PrimaryFont;
            _theme = theme ?? UguiTheme.Default;
            _onChange = onChange;

            _index = Mathf.Clamp(initialIndex, 0, Mathf.Max(0, _options.Count - 1));
            RefreshLabel();
        }

        private void RefreshLabel()
        {
            if (_labelTmp != null)
                _labelTmp.text = SelectedText;
        }

        public void SetIndex(int index, bool notify = true)
        {
            int clamped = Mathf.Clamp(index, 0, Mathf.Max(0, _options.Count - 1));
            if (clamped == _index) { RefreshLabel(); return; }
            _index = clamped;
            RefreshLabel();
            RefreshOptionVisuals(-1);
            if (notify) _onChange?.Invoke(_index);
        }

        internal void ToggleExpand()
        {
            if (_panelGo != null && _panelGo.activeSelf)
                UguiOverlayLayer.CloseOpenDropdown();
            else
                Expand();
        }

        private void Expand()
        {
            UguiOverlayLayer.EnsureInitialized();
            if (UguiOverlayLayer.OverlayRoot == null)
            {
                ScavLibPlugin.Log.LogWarning(
                    "[ScavLib.Ugui.Dropdown] Expand aborted — OverlayRoot not ready.");
                return;
            }

            UguiOverlayLayer.RegisterOpenDropdown(this);

            EnsurePanelBuilt();
            if (_panelGo == null) return;

            _panelGo.SetActive(true);
            _panelRect.SetAsLastSibling();
            PositionPanelUnderBody();
        }

        internal void Collapse()
        {
            if (_panelGo != null && _panelGo.activeSelf)
                _panelGo.SetActive(false);
        }

        private void EnsurePanelBuilt()
        {
            if (_panelGo != null) return;

            var overlay = UguiOverlayLayer.OverlayRoot;
            if (overlay == null) return;

            _panelGo = new GameObject("DropdownPanel");
            _panelGo.layer = LayerMask.NameToLayer("UI");
            _panelGo.transform.SetParent(overlay, false);

            _panelRect = _panelGo.AddComponent<RectTransform>();

            _panelRect.anchorMin = Vector2.zero;
            _panelRect.anchorMax = Vector2.zero;
            _panelRect.pivot = new Vector2(0f, 1f);
            _panelRect.localScale = Vector3.one;

            var bg = _panelGo.AddComponent<UImage>();
            bg.sprite = null;
            bg.color = PanelBg;
            bg.raycastTarget = true;

            BuildBorderFrame(_panelGo, BorderNormal, 1f);

            var vlg = _panelGo.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(2, 2, 2, 2);
            vlg.spacing = 0f;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var fitter = _panelGo.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            _optionButtons.Clear();
            _optionImages.Clear();
            _optionBorderRoots.Clear();

            for (int i = 0; i < _options.Count; i++)
            {
                int idx = i;
                BuildOptionRow(idx, _options[i], () =>
                {
                    SetIndex(idx, notify: true);
                    UguiOverlayLayer.CloseOpenDropdown();
                });
            }

            RefreshOptionVisuals(-1);
        }

        private void BuildOptionRow(int index, string label, Action onClick)
        {
            var m = _theme.Metrics;

            var go = new GameObject("Option");
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(_panelGo.transform, false);
            go.AddComponent<RectTransform>();

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = m.DropdownRowHeight;
            le.flexibleHeight = 0f;

            var img = go.AddComponent<UImage>();
            img.sprite = null;
            img.color = OptionBg;
            img.raycastTarget = true;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = OptionBg;
            colors.highlightedColor = OptionBg;
            colors.pressedColor = OptionBg;
            colors.selectedColor = OptionBg;
            colors.colorMultiplier = 1f;
            btn.colors = colors;
            if (onClick != null) btn.onClick.AddListener(() => onClick());

            var borderRoot = BuildBorderFrame(go, BorderNormal, 1f);

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
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            tmp.raycastTarget = false;
            if (_font != null) tmp.font = _font;

            int idx = index;
            var trigger = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            void Add(UnityEngine.EventSystems.EventTriggerType t,
                     UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData> cb)
            {
                var e = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = t };
                e.callback.AddListener(cb);
                trigger.triggers.Add(e);
            }
            Add(UnityEngine.EventSystems.EventTriggerType.PointerEnter,
                _ => RefreshOptionVisuals(idx));
            Add(UnityEngine.EventSystems.EventTriggerType.PointerExit,
                _ => RefreshOptionVisuals(-1));

            _optionButtons.Add(btn);
            _optionImages.Add(img);
            _optionBorderRoots.Add(borderRoot);
        }

        private Transform BuildBorderFrame(GameObject parent, Color color, float thickness)
        {
            var frameGo = new GameObject("Border");
            frameGo.layer = LayerMask.NameToLayer("UI");
            frameGo.transform.SetParent(parent.transform, false);

            var fr = frameGo.AddComponent<RectTransform>();
            fr.anchorMin = Vector2.zero;
            fr.anchorMax = Vector2.one;
            fr.offsetMin = Vector2.zero;
            fr.offsetMax = Vector2.zero;

            MakeEdge(frameGo, "Top", new Vector2(0f, 1f), new Vector2(1f, 1f),
                     new Vector2(0f, -thickness), Vector2.zero, color);
            MakeEdge(frameGo, "Bottom", new Vector2(0f, 0f), new Vector2(1f, 0f),
                     Vector2.zero, new Vector2(0f, thickness), color);
            MakeEdge(frameGo, "Left", new Vector2(0f, 0f), new Vector2(0f, 1f),
                     Vector2.zero, new Vector2(thickness, 0f), color);
            MakeEdge(frameGo, "Right", new Vector2(1f, 0f), new Vector2(1f, 1f),
                     new Vector2(-thickness, 0f), Vector2.zero, color);

            return frameGo.transform;
        }

        private static void MakeEdge(GameObject parent, string name,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(parent.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            var img = go.AddComponent<UImage>();
            img.sprite = null;
            img.color = color;
            img.raycastTarget = false;
        }

        private void RefreshOptionVisuals(int hoverIndex)
        {
            for (int i = 0; i < _optionBorderRoots.Count; i++)
            {
                var frame = _optionBorderRoots[i];
                if (frame == null) continue;
                Color c;
                if (i == _index) c = BorderSelected;
                else if (i == hoverIndex) c = BorderHover;
                else c = BorderNormal;

                for (int k = 0; k < frame.childCount; k++)
                {
                    var edgeImg = frame.GetChild(k).GetComponent<UImage>();
                    if (edgeImg != null) edgeImg.color = c;
                }
            }
        }

        private void PositionPanelUnderBody()
        {
            var overlay = UguiOverlayLayer.OverlayRoot;
            if (overlay == null || _bodyRect == null || _panelRect == null) return;

            RebuildLayoutUpwards(_bodyRect);

            Vector3[] corners = new Vector3[4];
            _bodyRect.GetWorldCorners(corners);

            Vector2 screenBL = new Vector2(corners[0].x, corners[0].y);
            Vector2 screenTL = new Vector2(corners[1].x, corners[1].y);
            Vector2 screenTR = new Vector2(corners[2].x, corners[2].y);

            if (screenBL == Vector2.zero && screenTR == Vector2.zero)
            {
                ScavLibPlugin.Log.LogWarning(
                    "[ScavLib.Ugui.Dropdown] Body screen corners are zero — " +
                    "deferring panel position to next frame.");
                var driver = overlay.GetComponent<UguiOverlayDriver>();
                if (driver != null) driver.StartCoroutine(DeferredPosition());
                return;
            }

            Vector2 localBL, localTL, localTR;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                overlay, screenBL, null, out localBL);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                overlay, screenTL, null, out localTL);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                overlay, screenTR, null, out localTR);

            float panelWidth = Mathf.Abs(localTR.x - localBL.x);
            if (panelWidth < 1f) panelWidth = 120f;

            float halfW = overlay.rect.width * 0.5f;
            float halfH = overlay.rect.height * 0.5f;

            Vector2 anchoredBL = new Vector2(localBL.x + halfW, localBL.y + halfH);
            Vector2 anchoredTL = new Vector2(localTL.x + halfW, localTL.y + halfH);

            _panelRect.sizeDelta = new Vector2(panelWidth, _panelRect.sizeDelta.y);

            _panelRect.anchoredPosition = anchoredBL;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelRect);
            float panelHeight = _panelRect.rect.height;

            if (anchoredBL.y - panelHeight < 0f)
            {
                _panelRect.anchoredPosition = new Vector2(
                    anchoredBL.x, anchoredTL.y + panelHeight);
            }

            ScavLibPlugin.Log.LogInfo(
                $"[ScavLib.Ugui.Dropdown] Positioned at anchored=({anchoredBL.x:F0}, {anchoredBL.y:F0}) " +
                $"width={panelWidth:F0} height={panelHeight:F0} " +
                $"overlay=({overlay.rect.width:F0}x{overlay.rect.height:F0})");
        }

        private static void RebuildLayoutUpwards(RectTransform leaf)
        {
            var node = leaf;
            int guard = 0;
            while (node != null && guard++ < 16)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(node);
                var parent = node.parent as RectTransform;
                if (parent == null) break;
                if (parent.GetComponent<Canvas>() != null) break;
                node = parent;
            }
        }

        private System.Collections.IEnumerator DeferredPosition()
        {
            yield return null;
            if (_panelGo != null && _panelGo.activeSelf)
                PositionPanelUnderBody();
        }

        internal void HandleOutsideClick(List<RaycastResult> casts)
        {
            if (_panelGo == null || !_panelGo.activeSelf) return;
            if (!Input.GetMouseButtonDown(0)) return;

            bool insidePanel = false;
            bool insideBody = false;
            if (casts != null)
            {
                for (int i = 0; i < casts.Count; i++)
                {
                    var t = casts[i].gameObject == null ? null : casts[i].gameObject.transform;
                    if (t == null) continue;
                    if (t.IsChildOf(_panelGo.transform)) { insidePanel = true; break; }
                    if (t == _bodyRect || t.IsChildOf(_bodyRect)) { insideBody = true; }
                }
            }
            if (!insidePanel && !insideBody)
                UguiOverlayLayer.CloseOpenDropdown();
        }
    }

    public static class UguiTooltipExtensions
    {

        public static T WithTooltip<T>(this T component, string name, string desc = "")
            where T : Component
        {
            if (component != null)
                AttachTooltip(component.gameObject, name, desc);
            return component;
        }

        public static GameObject WithTooltip(this GameObject go, string name, string desc = "")
        {
            AttachTooltip(go, name, desc);
            return go;
        }

        private static void AttachTooltip(GameObject go, string name, string desc)
        {
            if (go == null) return;

            var marker = go.GetComponent<UguiTooltipMarker>();
            if (marker == null) marker = go.AddComponent<UguiTooltipMarker>();
            marker.TipName = name ?? "";
            marker.TipDesc = desc ?? "";

            var graphic = go.GetComponent<Graphic>();
            if (graphic != null) graphic.raycastTarget = true;
        }
    }
}
