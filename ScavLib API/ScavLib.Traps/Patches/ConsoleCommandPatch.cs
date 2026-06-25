using System;
using System.Collections.Generic;
using HarmonyLib;
using TrapLib.MP;
using UnityEngine;

namespace TrapLib.Patches;

/// <summary>
/// Postfix on <c>ConsoleScript.RegisterSpawnEntities</c> that (a) appends trap IDs
/// to the <c>spawn</c> command's autofill and (b) wraps the command's action once
/// so trap IDs route to <see cref="TrapRegistry.Spawn"/> while every other ID is
/// passed through to the original action untouched.
///
/// <para>Coexistence with <c>ScavLib.item.patches.ConsoleSpawnAutofillPatch</c>:
/// both sides Postfix the same method and append to <c>argAutofill[0]</c> with
/// <c>List.Add</c> only — neither rebuilds the dictionary nor uses the index-add
/// path the <c>Command</c> ctor reserves for <c>bool</c>/<c>position</c> args, so
/// the two compose without the duplicate-key <see cref="ArgumentException"/> that
/// path would raise.</para>
///
/// <para>Hook point: <c>RegisterSpawnEntities</c> runs lazily on the first
/// <c>ToggleActiveState</c> (i.e. when the console is first opened), after
/// vanilla has populated <c>argAutofill[0]</c>. A <c>Start</c> patch would be too
/// early. The action-wrap is guarded by a static latch so repeated invocations
/// can never double-wrap and recurse on themselves.</para>
///
/// <para>Pass-through is deliberate: non-trap IDs must reach the original action
/// so ScavLib's <c>UtilsCreatePatch</c> continues to handle custom items.</para>
/// </summary>
[HarmonyPatch(typeof(ConsoleScript), "RegisterSpawnEntities")]
internal static class ConsoleCommandPatch
{
    // Latch: action is wrapped at most once per process lifetime.
    // Without this a second RegisterSpawnEntities call would nest the wrapper
    // around itself and recurse on every /spawn invocation.
    private static bool _wrapped;

    private static void Postfix()
    {
        var cmd = ConsoleScript.SearchExact("spawn");
        if (cmd == null) return;

        // (a) Append trap IDs. Lazy-new the dict / list so we are robust even
        //     if a future build drops the 'position' arg that currently makes
        //     argAutofill non-null. Add-only; never replace the existing list.
        if (cmd.argAutofill == null)
            cmd.argAutofill = new Dictionary<int, List<string>>();
        if (!cmd.argAutofill.TryGetValue(0, out var fills) || fills == null)
        {
            fills = new List<string>();
            cmd.argAutofill[0] = fills;
        }
        foreach (var id in TrapRegistry.Entries.Keys)
            if (!fills.Contains(id)) fills.Add(id);

        // (b) Wrap the action exactly once.
        if (_wrapped) return;
        _wrapped = true;

        var originalAction = cmd.action;
        cmd.action = args =>
        {
            if (args.Length >= 2 && TrapRegistry.Entries.ContainsKey(args[1]))
            {
                // Trap spawning is server-authoritative; KrokMP syncs the object
                // out from the host. Spawning on a pure client would desync.
                if (MPSync.IsClient)
                {
                    TrapLibPlugin.Log?.LogWarning(
                        "[ConsoleCommandPatch] /spawn trap blocked on MP client — " +
                        "only the server/host may spawn traps.");
                    return;
                }

                TrapRegistry.Spawn(args[1], GetSpawnPosition(args));
                return;
            }

            // Non-trap ID: hand back so ScavLib's UtilsCreatePatch can run.
            originalAction(args);
        };
    }

    private static Vector3 GetSpawnPosition(string[] args)
    {
        if (args.Length > 2)
        {
            var input = args[2];

            if (input == "cursor")
                return CursorWorldPos();

            if (input == "player" && PlayerCamera.main != null)
                return PlayerCamera.main.body.transform.position;

            if (input == "random" && WorldGeneration.world != null)
                return new Vector2(
                    UnityEngine.Random.Range(-1f, 1f) * WorldGeneration.world.halfWidth,
                    UnityEngine.Random.Range(-1f, 1f) * WorldGeneration.world.halfHeight);

            float x, y;
            if (float.TryParse(input, out x) && args.Length >= 4 && float.TryParse(args[3], out y))
                return new Vector3(x, y, 0f);

            var parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2
                && float.TryParse(parts[0], out x)
                && float.TryParse(parts[1], out y))
                return new Vector3(x, y, 0f);
        }

        return CursorWorldPos();
    }

    private static Vector3 CursorWorldPos()
    {
        if (Camera.main == null)
        {
            TrapLibPlugin.Log?.LogWarning(
                "[ConsoleCommandPatch] Camera.main is null, cannot get cursor position.");
            return Vector3.zero;
        }
        var wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(wp.x, wp.y, 0f);
    }
}
