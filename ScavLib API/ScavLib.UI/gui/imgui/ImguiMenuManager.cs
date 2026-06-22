using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Input;

namespace ScavLib.gui.imgui
{

    public class ImguiMenuManager
    {

        public static ImguiMenuManager Instance { get; private set; }

        internal static void EnsureInstance()
        {
            if (Instance != null) return;
            Instance = new ImguiMenuManager();
            ScavLibPlugin.Log.LogInfo("[ImguiMenuManager] Instance created.");
        }

        private readonly List<ImguiWindow> _windows = new List<ImguiWindow>();
        private int _nextWindowId = 9000;

        private ImguiMenuManager() { }

        public static void Register(ImguiWindow window)
        {
            EnsureInstance();
            window.Initialize(Instance._nextWindowId++);

            int insertAt = Instance._windows.Count;
            for (int i = 0; i < Instance._windows.Count; i++)
            {
                if (window.Layer < Instance._windows[i].Layer)
                {
                    insertAt = i;
                    break;
                }
            }
            Instance._windows.Insert(insertAt, window);

            ScavLibPlugin.Log.LogInfo(
                $"[ImguiMenuManager] Registered window: '{window.Title}' (Layer={window.Layer})");
        }

        internal void DoUpdate()
        {
            if (ConsoleScript.instance != null && ConsoleScript.instance.active) return;

            bool inGame = PlayerCamera.main != null;

            foreach (var window in _windows)
            {
                if (!inGame && !window.ShowInMenu) continue;
                if (window.ToggleKey != KeyCode.None && Input.GetKeyDown(window.ToggleKey))
                    window.Toggle();
            }
        }

        internal void DoOnGUI()
        {
            bool inGame = PlayerCamera.main != null;

            foreach (var window in _windows)
            {
                if (!inGame && !window.ShowInMenu) continue;
                window.OnGUI();
            }
        }

        internal static void Init()
        {
            ScavLibPlugin.Log.LogInfo(
                "[ImguiMenuManager] Init() called (renderer attaches on ConsoleScript.Awake).");
        }
    }
}
