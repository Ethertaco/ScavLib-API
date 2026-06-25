using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ScavLib.bootstrap;
using ScavLib.command;
using ScavLib.i18n;
using ScavLib.mods;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ScavLib
{
    [BepInPlugin("com.kanisuko.scavlib", "ScavLib", Version)]
    [BepInDependency("KrokoshaCasualtiesMP", BepInDependency.DependencyFlags.SoftDependency)]
    public class ScavLibPlugin : BaseUnityPlugin
    {
        public const string Version = "0.8.0";

        public static ScavLibPlugin Instance { get; private set; }

        /// <summary>
        /// Shared log source. Public so feature modules living in their own DLLs can
        /// write through it without taking a back-reference on plugin internals.
        /// </summary>
        public static ManualLogSource Log { get; private set; }

        private Harmony _harmony;

        internal static readonly Dictionary<string, bool> PatchStatus
            = new Dictionary<string, bool>();
        internal static readonly Dictionary<string, string> PatchErrors
            = new Dictionary<string, string>();

        internal const string SelfModName = "ScavLib";

        private void Awake()
        {
            Instance = this;
            Log = base.Logger;

            _harmony = new Harmony("com.kanisuko.scavlib");

            string pluginDir = Path.GetDirectoryName(Info.Location);

            // Apply all patches, fault-isolated one at a time.
            ApplyPatchesIndividually();

            CommandRegistry.Init();

            // Discover and initialize feature modules (GUI, etc.). The core does not
            // reference any module directly; the host finds them by reflection, which
            // is what keeps the core/module assembly references one-directional.
            var moduleCtx = new ScavLibModuleContext
            {
                Harmony = _harmony,
                Log = Log,
                Version = Version,
                PluginDir = pluginDir
            };
            ScavLibModuleHost.DiscoverAndInit(moduleCtx);

            compat.RivalFrameworkDetector.CheckAndWarn();
            compat.krokmp.KrokMpBridge.Init(_harmony);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnFirstSceneLoaded;

            // Auto-register ScavLib's own i18n.
            try
            {
                i18n.LocaleManager.AutoRegister("ScavLib", pluginDir);
            }
            catch (System.Exception ex)
            {
                Log.LogError($"[ScavLib] Failed to initialize self-i18n: {ex.Message}");
            }

            ModRegistry.Register(new ModInfo(
                SelfModName,
                Version,
                "Base API library for Scav Prototype mods.",
                "Kanisuko / QinShenYu"
            ));

            // Register the library's own console command.
            CommandRegistry.TryRegister(new ScavLibCommand(), SelfModName, out _);

            Log.LogInfo($"ScavLib {Version} loaded successfully.");
        }

        private void OnFirstSceneLoaded(
            UnityEngine.SceneManagement.Scene scene,
            UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnFirstSceneLoaded;
            Log.LogInfo($"[ScavLib] First scene '{scene.name}' loaded — spawning input host.");
            SpawnInputHosts();
        }

        private void ApplyPatchesIndividually()
        {
            var patchTypes = new System.Type[]
            {
                typeof(command.CommandRegistryPatch),
                typeof(event_bus.patches.WorldLoadedPatch),
                typeof(event_bus.patches.LayerLoadedPatch),
                typeof(event_bus.patches.WorldDestroyedPatch),
                typeof(event_bus.patches.WorldUnloadingContinueRunPatch),
                typeof(event_bus.patches.WorldUnloadingSaveAndExitPatch),
                typeof(event_bus.patches.ItemDropPatch),
                typeof(event_bus.patches.ItemPickupPatch),

                // --- custom content ---
                typeof(item.patches.ItemSetupItemsPatch),
                typeof(item.patches.UtilsCreatePatch.PosRot),
                typeof(item.patches.UtilsCreatePatch.Parented),
                typeof(item.patches.ConsoleSpawnAutofillPatch),
                typeof(recipe.patches.RecipesSetUpRecipesPatch),
                typeof(recipe.patches.RecipeResultSpritePatch),
                typeof(i18n.LocalePatches),

                // --- save compatibility ---
                typeof(save.patches.SaveGamePatch),
                typeof(save.patches.TryLoadGamePatch),

                // --- keybind API ---
                typeof(input.patches.SettingsDefaultsPatch),
                typeof(input.patches.SettingsLoadPostPatch),
                typeof(input.patches.SettingsMenuTabPatch),
            };

            foreach (var t in patchTypes)
            {
                string name = t.Name;
                try
                {
                    _harmony.PatchAll(t);
                    PatchStatus[name] = true;
                }
                catch (System.Exception ex)
                {
                    PatchStatus[name] = false;
                    PatchErrors[name] = ex.Message;
                    Log.LogError(
                        $"[ScavLib] Patch '{name}' failed to apply: {ex.Message}. " +
                        $"Dependent functionality will be disabled, but the rest of " +
                        $"ScavLib will continue to operate.");
                }
            }
        }

        // Spawns the core-owned scene hosts. Currently only KeyBindHost (input);
        // the GUI hosts moved to GuiModule. This shrinks further when input is split.
        private void SpawnInputHosts()
        {
            try
            {
                input.KeyBindHost.EnsureSpawned();
                PatchStatus["KeyBindHost"] = true;
            }
            catch (System.Exception ex)
            {
                PatchStatus["KeyBindHost"] = false;
                PatchErrors["KeyBindHost"] = ex.Message;
                Log.LogError($"[ScavLib] Failed to spawn KeyBindHost: {ex.Message}.");
            }
        }
    }
}