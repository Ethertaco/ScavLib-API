using System.Reflection;
using ScavLib.event_bus.events;
using ScavLib.mods;
using UnityEngine;

namespace TrapLib;

/// <summary>
/// Drives world-gen trap distribution off ScavLib's <see cref="LayerLoadedEvent"/>
/// instead of a Harmony Postfix on <c>WorldGeneration.Update</c>.
///
/// <para>Rationale: the original <c>TrapSpawner</c>/<c>TrapSpawner_Reset</c> patches
/// hooked the same <c>WorldGeneration.Update</c> point that ScavLib's
/// <c>LayerLoadedPatch</c> already owns. Two patches racing on the same method to
/// answer "did a layer just finish generating?" duplicate the private-field
/// (<c>instantiatingWorld</c>) probing and risk ordering bugs. ScavLib has already
/// solved that detection once and re-publishes the result as a single, ordered
/// event — this distributor subscribes to that event and deletes the duplicate
/// machinery.</para>
///
/// <para>The event fires exactly once per playable layer (including the first), so
/// the old <c>DidSpawn</c>/<c>WasGenerating</c> latches are unnecessary here.</para>
/// </summary>
internal sealed class TrapDistributor : ModLifecycleBase
{
    // Mirrors the BindingFlags the original TrapSpawner used to poke each trap
    // type's static ResetSpawnCount()/SpawnCount members generically.
    private const BindingFlags MemberFlags =
        BindingFlags.Static | BindingFlags.NonPublic |
        BindingFlags.Public | BindingFlags.FlattenHierarchy;

    /// <summary>
    /// Distribute every registered trap into the layer that just became playable.
    ///
    /// <para>Reacquires the <see cref="WorldGeneration"/> MonoBehaviour via
    /// <c>FindObjectOfType</c> because <see cref="LayerLoadedEvent"/> intentionally
    /// carries only <c>BiomeDepth</c>/<c>IsFirstLoad</c>, while
    /// <c>biomeDepth</c>/<c>totalTrapRarity</c>/<c>DistributeEntities</c> are all
    /// instance members. <c>WorldGeneration.world</c> is the grid object, not this
    /// MonoBehaviour, so it cannot stand in.</para>
    /// </summary>
    public override void OnLayerLoaded(LayerLoadedEvent e)
    {
        var wg = Object.FindObjectOfType<WorldGeneration>();
        if (wg == null || !wg.worldExists) return;

        TrapLibPlugin.Log?.LogInfo(
            $"[TrapDistributor] Layer ready (depth={wg.biomeDepth}, " +
            $"trapRarity={wg.totalTrapRarity}, firstLoad={e.IsFirstLoad}). Distributing traps.");

        foreach (var kv in TrapRegistry.Entries)
        {
            var config = kv.Value.config;
            var type   = kv.Value.type;

            if (wg.biomeDepth < config.MinBiomeDepth) continue;
            if (config.MaxBiomeDepth > 0 && wg.biomeDepth > config.MaxBiomeDepth) continue;

            float min = config.SpawnRateMin * wg.totalTrapRarity;
            float max = config.SpawnRateMax * wg.totalTrapRarity;

            var prefab = TrapRegistry.GetOrCreatePrefab(type, config);
            if (prefab == null) continue;

            // Per-trap spawn counter reset (generic, matches the original patch).
            type.GetMethod("ResetSpawnCount", MemberFlags)?.Invoke(null, null);

            // Preserved verbatim from the original TrapSpawner. Reproduced
            // positionally rather than via named args so it binds to the exact
            // same 12-param DistributeEntities overload this assembly compiles
            // against — the trailing `true` lands on the `isTrap` slot.
            wg.DistributeEntities(prefab, min, max, config.SpawnYOffset, 0f,
                config.SpawnYOffsetDeviation, config.SpawnInGround, false, null, true);

            var countField = type.GetField("SpawnCount", MemberFlags);
            int count = countField != null ? (int)countField.GetValue(null) : -1;
            TrapLibPlugin.Log?.LogInfo($"[TrapDistributor] {config.Id}: spawned={count}");
        }
    }

    /// <summary>
    /// Clears the live-trap bookkeeping when the world tears down, replacing the
    /// old <c>TrapSpawner_Reset</c> Postfix on <c>WorldGeneration.Start</c>.
    ///
    /// <para>Hooked to <c>OnWorldDestroyed</c> (scene unload) rather than a
    /// per-layer event because <c>TrapBuildings</c> is a session-scoped cache of
    /// BuildingEntity instances; the original reset fired once per WorldGeneration
    /// lifetime (Start), and OnWorldDestroyed is the event-bus equivalent.</para>
    /// </summary>
    public override void OnWorldDestroyed(WorldDestroyedEvent e)
    {
        TrapBase.TrapBuildings.Clear();
        TrapBase.ResetSpawnCount();
    }
}
