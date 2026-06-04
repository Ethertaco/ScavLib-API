using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScavLib.gui.ugui
{

    public sealed class UguiTabView : IDisposable
    {
        private readonly GameObject _rowGo;
        private readonly RectTransform _rowRect;
        private readonly TMP_FontAsset _font;
        private readonly UguiTheme _theme;
        private readonly LayoutElement _selfLayout;

        private RectTransform _headerRect;
        private RectTransform _bodyRect;

        private readonly List<TabPage> _pages = new List<TabPage>();
        private readonly List<Button> _tabButtons = new List<Button>();
        private int _activeIndex = -1;
        private bool _ended;

        private sealed class TabPage
        {
            public string Name;
            public Action<UguiBuilder> Build;
            public RectTransform PageRect;
            public ContentSizeFitter Fitter;
        }

        internal UguiTabView(GameObject rowGo, RectTransform rowRect, TMP_FontAsset font, UguiTheme theme)
        {
            _rowGo = rowGo;
            _rowRect = rowRect;
            _font = font ?? UguiFontManager.PrimaryFont;
            _theme = theme ?? UguiTheme.Default;

            _selfLayout = rowGo.GetComponent<LayoutElement>();

            BuildSkeleton();
        }

        public void AddTab(string name, Action<UguiBuilder> buildPage)
        {
            _pages.Add(new TabPage { Name = name, Build = buildPage });
        }

        public void End()
        {
            if (_ended) return;
            _ended = true;
            if (_pages.Count == 0) return;

            BuildHeaderButtons();
            BuildPages();
            Select(0);
        }

        public void Dispose() => End();

        private void BuildSkeleton()
        {

            var vlg = _rowGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = _theme.Metrics.RowSpacing;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childAlignment = TextAnchor.UpperCenter;

            _headerRect = MakeChildRow("TabHeader", _theme.Metrics.TabHeaderHeight, expandHeight: false);

            var hlg = _headerRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = _theme.Metrics.HorizontalSpacing;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childAlignment = TextAnchor.MiddleCenter;

            _bodyRect = MakeChildRow("TabBody", 0f, expandHeight: false);

            var bodyVlg = _bodyRect.gameObject.AddComponent<VerticalLayoutGroup>();
            bodyVlg.childControlWidth = true;
            bodyVlg.childControlHeight = true;
            bodyVlg.childForceExpandWidth = true;
            bodyVlg.childForceExpandHeight = false;
            bodyVlg.childAlignment = TextAnchor.UpperCenter;

            var bodyFitter = _bodyRect.gameObject.AddComponent<ContentSizeFitter>();
            bodyFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            bodyFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        private RectTransform MakeChildRow(string name, float preferredHeight, bool expandHeight)
        {
            var go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(_rowRect, false);
            var rect = go.AddComponent<RectTransform>();
            var le = go.AddComponent<LayoutElement>();
            if (preferredHeight > 0f) le.preferredHeight = preferredHeight;
            le.flexibleHeight = expandHeight ? 1f : 0f;
            return rect;
        }

        private void BuildHeaderButtons()
        {
            float headerWidth = _rowRect.rect.width;
            if (headerWidth <= 1f) headerWidth = Mathf.Abs(_rowRect.sizeDelta.x);

            var row = new UguiHorizontalRow(
                _headerRect, headerWidth, _theme.Metrics.TabHeaderHeight,
                _font, _theme, commitRowHeight: _ => { });

            for (int i = 0; i < _pages.Count; i++)
            {
                int idx = i;
                row.AddFlexibleSmallButton(_pages[i].Name, () => Select(idx));
            }
            row.End();

            _tabButtons.Clear();
            foreach (var btn in _headerRect.GetComponentsInChildren<Button>(true))
                _tabButtons.Add(btn);
        }

        private void BuildPages()
        {
            foreach (var page in _pages)
            {
                var pageGo = new GameObject($"Page_{page.Name}");
                pageGo.layer = LayerMask.NameToLayer("UI");
                pageGo.transform.SetParent(_bodyRect, false);

                var pageRect = pageGo.AddComponent<RectTransform>();
                pageRect.anchorMin = Vector2.zero;
                pageRect.anchorMax = Vector2.one;

                var pageFitter = pageGo.AddComponent<ContentSizeFitter>();
                pageFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                pageFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

                page.PageRect = pageRect;
                page.Fitter = pageFitter;

                var pageBuilder = new UguiBuilder(pageRect, _font, _theme);
                page.Build?.Invoke(pageBuilder);

                pageGo.SetActive(false);
            }
        }

        public void Select(int index)
        {
            if (index < 0 || index >= _pages.Count) return;
            _activeIndex = index;

            for (int i = 0; i < _pages.Count; i++)
            {
                var pr = _pages[i].PageRect;
                if (pr != null) pr.gameObject.SetActive(i == index);
            }

            UpdateSelfHeight();
        }

        private void UpdateSelfHeight()
        {
            if (_selfLayout == null) return;

            if (_activeIndex >= 0 && _pages[_activeIndex].PageRect != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(_pages[_activeIndex].PageRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_bodyRect);

            float bodyH = LayoutUtility.GetPreferredHeight(_bodyRect);
            float total = _theme.Metrics.TabHeaderHeight + _theme.Metrics.RowSpacing + bodyH;

            _selfLayout.preferredHeight = total;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rowRect);
        }

        public int ActiveIndex => _activeIndex;
    }
}
