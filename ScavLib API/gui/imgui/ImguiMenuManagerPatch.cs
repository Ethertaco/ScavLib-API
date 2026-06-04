using UnityEngine;

namespace ScavLib.gui.imgui
{

    internal class ImguiMenuRenderer : MonoBehaviour
    {
        private static ImguiMenuRenderer _renderer;

        internal static void EnsureSpawned()
        {
            if (_renderer != null) return;

            var go = new GameObject("ScavLib_ImguiMenuRenderer");
            Object.DontDestroyOnLoad(go);
            _renderer = go.AddComponent<ImguiMenuRenderer>();

            ScavLibPlugin.Log.LogInfo(
                "[ImguiMenuManager] ImguiMenuRenderer spawned on standalone host.");
        }

        private void Awake()
        {
            ImguiMenuManager.EnsureInstance();
        }

        private void Update()
        {
            ImguiMenuManager.Instance?.DoUpdate();
        }

        private void OnGUI()
        {
            GUI.depth = 100;
            ImguiMenuManager.Instance?.DoOnGUI();
        }
    }
}
