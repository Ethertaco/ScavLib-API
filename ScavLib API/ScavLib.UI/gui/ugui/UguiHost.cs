using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ScavLib.gui.ugui
{

    internal class UguiHost : MonoBehaviour
    {
        internal static UguiHost Instance { get; private set; }
        internal RectTransform UiRoot { get; private set; }

        private Canvas _overlayCanvas;

        internal static void EnsureSpawned()
        {
            if (Instance != null) return;

            var go = new GameObject("ScavLibUgui_Host");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<UguiHost>();

            ScavLibPlugin.Log.LogInfo(
                "[ScavLib.Ugui.Host] Host spawned (eager, from ScavLibPlugin.Awake).");
        }

        private void Awake()
        {
            Instance = this;
            UguiFontManager.Initialize();

            EnsureOverlayCanvas();
            EnsureRootObject();

            UguiOverlayLayer.EnsureInitialized();
        }

        private void Update()
        {

            EnsureOverlayCanvas();
            EnsureRootObject();

            if (PlayerCamera.main != null && PlayerCamera.main.mainCanvas != null)
                UguiFontManager.EnsurePrimaryResolved();

            if (ConsoleScript.instance != null && ConsoleScript.instance.active)
                return;

            UguiWindowBase.UpdateAll();
        }

        private void EnsureOverlayCanvas()
        {

            if (_overlayCanvas != null) return;

            var canvasGo = new GameObject("ScavLibUgui_OverlayCanvas");
            canvasGo.layer = LayerMask.NameToLayer("UI");
            Object.DontDestroyOnLoad(canvasGo);

            _overlayCanvas = canvasGo.AddComponent<Canvas>();
            _overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            _overlayCanvas.sortingOrder = 32760;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            UiRoot = null;
        }

        private void EnsureRootObject()
        {
            if (UiRoot != null && UiRoot.parent != null) return;

            if (_overlayCanvas == null) EnsureOverlayCanvas();

            if (UiRoot == null)
            {
                var rootGo = new GameObject("ScavLibUgui_UiRoot");
                rootGo.layer = LayerMask.NameToLayer("UI");
                Object.DontDestroyOnLoad(rootGo);

                UiRoot = rootGo.AddComponent<RectTransform>();
                UiRoot.anchorMin = Vector2.zero;
                UiRoot.anchorMax = Vector2.one;
                UiRoot.offsetMin = Vector2.zero;
                UiRoot.offsetMax = Vector2.zero;
            }

            UiRoot.SetParent(_overlayCanvas.transform, false);
            UiRoot.localScale = Vector3.one;
            UiRoot.anchorMin = Vector2.zero;
            UiRoot.anchorMax = Vector2.one;
            UiRoot.offsetMin = Vector2.zero;
            UiRoot.offsetMax = Vector2.zero;
            UiRoot.SetAsLastSibling();
        }

        internal TMP_FontAsset PrimaryFont => UguiFontManager.PrimaryFont;
    }
}
