using ScavLib.bootstrap;
using ScavLib.gui.imgui;
using ScavLib.gui.ugui;
using UnityEngine.SceneManagement;

namespace ScavLib.gui
{

    public sealed class GuiModule : IScavLibModule
    {
        public string Name => "Gui";
        public int Order => 100;

        private static bool _initialized;
        private ScavLibModuleContext _ctx;

        public void Init(ScavLibModuleContext ctx)
        {
            if (_initialized) return;
            _initialized = true;
            _ctx = ctx;

            ImguiMenuManager.Init();

            try
            {
                ctx.Harmony.PatchAll(typeof(UguiInputBlockerPatch));
                ctx.ReportStatus("UguiInputBlockerPatch", true);
            }
            catch (System.Exception ex)
            {
                ctx.ReportStatus("UguiInputBlockerPatch", false, ex.Message);
                ctx.Log.LogError(
                    $"[ScavLib] Patch 'UguiInputBlockerPatch' failed to apply: {ex.Message}. " +
                    $"Dependent functionality will be disabled, but the rest of " +
                    $"ScavLib will continue to operate.");
            }

            SceneManager.sceneLoaded += OnFirstSceneLoaded;
        }

        private void OnFirstSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnFirstSceneLoaded;
            _ctx.Log.LogInfo($"[ScavLib] First scene '{scene.name}' loaded — spawning GUI hosts.");
            SpawnGuiHosts();
        }

        private void SpawnGuiHosts()
        {
            try
            {
                ImguiMenuRenderer.EnsureSpawned();
                _ctx.ReportStatus("ImguiHost", true);
            }
            catch (System.Exception ex)
            {
                _ctx.ReportStatus("ImguiHost", false, ex.Message);
                _ctx.Log.LogError($"[ScavLib] Failed to spawn IMGUI host: {ex.Message}.");
            }

            try
            {
                UguiHost.EnsureSpawned();
                _ctx.ReportStatus("UguiHost", true);
            }
            catch (System.Exception ex)
            {
                _ctx.ReportStatus("UguiHost", false, ex.Message);
                _ctx.Log.LogError($"[ScavLib] Failed to spawn uGUI host: {ex.Message}.");
            }
        }
    }
}