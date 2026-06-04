using UnityEngine;

namespace ScavLib.gui.imgui
{

    public abstract class ImguiWindow
    {

        public abstract string Title { get; }

        public virtual KeyCode ToggleKey => KeyCode.None;

        public virtual float Width => 300f;

        public virtual float Height => 0f;

        public virtual Vector2 InitialPosition => new Vector2(
            (Screen.width - Width) / 2f,
            (Screen.height - 400f) / 2f);

        public virtual int Layer => 0;

        public virtual bool ShowInMenu => false;

        public bool IsVisible { get; private set; } = false;

        internal Rect WindowRect;
        private bool _initialized = false;
        private int _windowId;

        private bool AutoHeight => Height <= 0f;

        internal void Initialize(int windowId)
        {
            _windowId = windowId;
            float initialHeight = AutoHeight ? 0f : Height;
            WindowRect = new Rect(InitialPosition.x, InitialPosition.y, Width, initialHeight);
            _initialized = true;
        }

        public void Show()
        {
            if (IsVisible) return;
            IsVisible = true;
            OnShow();
        }

        public void Hide()
        {
            if (!IsVisible) return;
            IsVisible = false;
            OnHide();
        }

        public void Toggle()
        {
            if (IsVisible) Hide();
            else Show();
        }

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }

        internal void OnGUI()
        {
            if (!IsVisible || !_initialized) return;

            if (AutoHeight)
            {
                WindowRect = GUILayout.Window(
                    _windowId,
                    WindowRect,
                    DrawWindow,
                    Title,
                    GUILayout.Width(Width),
                    GUILayout.ExpandHeight(true));
            }
            else
            {
                WindowRect = GUILayout.Window(
                    _windowId,
                    WindowRect,
                    DrawWindow,
                    Title,
                    GUILayout.Width(Width),
                    GUILayout.Height(Height));
            }
        }

        private void DrawWindow(int _)
        {
            DrawContent();
            GUI.DragWindow(new Rect(0, 0, Width, 20f));
        }

        protected abstract void DrawContent();
    }
}
