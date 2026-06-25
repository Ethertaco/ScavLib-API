using System;
using System.Reflection;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using ScavLib.bootstrap;
using ScavLib.mods;
using TrapLib.MP;
using TrapLib.Patches;

namespace TrapLib;

/// <summary>
/// ScavLib feature-module entry point for the trap framework. Discovered
/// reflectively by <c>ScavLibModuleHost</c> via the <c>ScavLib*.dll</c> glob,
/// which is why the assembly is named <c>ScavLib.Traps.dll</c> and this type
/// is <c>public</c> with a parameterless ctor.
///
/// <para>Replaces the original <c>TrapLibPlugin.Awake</c>: the trap library is
/// no longer its own BepInEx plugin, so all bootstrap work (log/flag seeding,
/// patch application, lifecycle registration) moves here and runs during
/// <c>ScavLibPlugin.Awake</c> — well before any <c>TrapBase.Start</c> can run.</para>
/// </summary>
public sealed class TrapsModule : IScavLibModule
{
    public string Name => "Traps";

    /// <summary>Feature layer: core infrastructure is 0; traps sit above it.</summary>
    public int Order => 100;

    public void Init(ScavLibModuleContext ctx)
    {
        // ── 1. Seed the shared runtime holder so every moved TrapLib file
        //       (which still references TrapLibPlugin.*) keeps working. This is
        //       also the single MP-façade convergence point: KrokMP / RshLib
        //       detection happens once, here, then MPSync caches off it. ──
        TrapLibPlugin.Log = ctx.Log;
        TrapLibPlugin.KrokMpEnabled  =
            Chainloader.PluginInfos.ContainsKey("KrokoshaCasualtiesMP");
        TrapLibPlugin.RshLibInstalled =
            Chainloader.PluginInfos.ContainsKey("com.rushellxyz.rshlib");
        MPSync.Refresh();

        // ── 2. Apply attribute-based patches in THIS assembly only.
        //       PatchAll(Assembly) — not PatchAll() — so we never sweep up
        //       ScavLib core patch types that the host already applied. ──
        try
        {
            ctx.Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            ctx.ReportStatus("Traps_Patches", false, ex.Message);
            ctx.Log.LogError($"[Traps] PatchAll failed: {ex}");
        }

        // BuildingEntityPatch uses manual harmony.Patch (not [HarmonyPatch]),
        // so PatchAll won't catch it. Apply only when RshLib is absent —
        // RshLib's own drop patch is authoritative when present. (Parity with
        // the old TrapLibPlugin.Awake.)
        if (!TrapLibPlugin.RshLibInstalled)
        {
            try
            {
                BuildingEntityPatch.Apply(ctx.Harmony);
            }
            catch (Exception ex)
            {
                ctx.ReportStatus("Traps_BuildingEntityPatch", false, ex.Message);
                ctx.Log.LogError($"[Traps] BuildingEntityPatch.Apply failed: {ex}");
            }
        }

        // ── 3. Register with ScavLib's lifecycle. TrapDistributor's
        //       OnLayerLoaded / OnWorldDestroyed get auto-subscribed to the
        //       EventBus by ModRegistry — no manual EventBus.Register needed. ──
        ModRegistry.Register(
            new ModInfo(
                "Traps",
                "1.1.2",
                "Custom trap framework: world-gen integration, zone effects, MP-aware.",
                "MLSLi (vertigo)"),
            new TrapDistributor());

        // i18n: TrapLib itself ships zero CN entries — every translation comes
        // from downstream trap mods either through TrapConfig.FullNameCn
        // (legacy fallback handled by TrapLocale.Resolve) or via
        // LocaleManager.RegisterItem / RegisterString. No AutoRegister call
        // is needed at the framework level.

        ctx.ReportStatus("Traps", true);
        ctx.Log.LogInfo("Traps module loaded.");
    }
}

/// <summary>
/// Lightweight static holder that stands in for the removed
/// <c>TrapLibPlugin : BaseUnityPlugin</c>. The trap library is now a ScavLib
/// module, not a standalone plugin, but the moved source files still read
/// <c>TrapLibPlugin.Log</c> / <c>.KrokMpEnabled</c> / <c>.RshLibInstalled</c>.
/// Seeded once from <see cref="TrapsModule.Init"/>.
///
/// <para>Note: <c>Plugin.cs</c> is intentionally NOT part of the move set, so
/// there is no duplicate <c>TrapLib.TrapLibPlugin</c> type in this assembly.</para>
/// </summary>
internal static class TrapLibPlugin
{
    internal static ManualLogSource Log;
    internal static bool KrokMpEnabled;
    internal static bool RshLibInstalled;
}
