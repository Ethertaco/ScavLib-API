using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ScavLib.bootstrap
{
    /// <summary>
    /// Discovers <see cref="IScavLibModule"/> implementations and runs their
    /// <c>Init</c> in a fault-isolated loop, recording results into the same
    /// PatchStatus/PatchErrors tables used by the per-patch loop in
    /// <c>ScavLibPlugin.ApplyPatchesIndividually</c> — same philosophy, no new
    /// machinery.
    ///
    /// <para>Scans the core assembly itself plus every sibling
    /// <c>ScavLib*.dll</c> sitting next to the core DLL, so a feature module that
    /// has been split into its own assembly (e.g. the UI module in
    /// <c>ScavLib API.UI.dll</c>) is still found. The core never references those
    /// assemblies at compile time — discovery is purely reflective, which keeps the
    /// core/module references one-directional.</para>
    /// </summary>
    internal static class ScavLibModuleHost
    {
        internal static void DiscoverAndInit(ScavLibModuleContext ctx)
        {
            List<IScavLibModule> modules = Discover();

            // Deterministic order: never rely on reflection/file enumeration order.
            // Ties broken by name so the sequence is reproducible across machines.
            modules.Sort((a, b) =>
            {
                int byOrder = a.Order.CompareTo(b.Order);
                return byOrder != 0 ? byOrder : string.CompareOrdinal(a.Name, b.Name);
            });

            foreach (var module in modules)
            {
                string name = module.Name;
                try
                {
                    module.Init(ctx);
                    ScavLibPlugin.PatchStatus[name] = true;
                    ctx.Log.LogInfo($"[ScavLib] Module '{name}' initialized.");
                }
                catch (Exception ex)
                {
                    ScavLibPlugin.PatchStatus[name] = false;
                    ScavLibPlugin.PatchErrors[name] = ex.Message;
                    ctx.Log.LogError(
                        $"[ScavLib] Module '{name}' failed to initialize: {ex.Message}. " +
                        $"Its functionality will be disabled, but the rest of ScavLib continues.");
                }
            }
        }

        // --- discovery ------------------------------------------------------------

        private static List<IScavLibModule> Discover()
        {
            var result = new List<IScavLibModule>();
            var seen = new HashSet<string>();

            // The core assembly itself (in case any module still lives in-process here).
            CollectFromAssembly(Assembly.GetExecutingAssembly(), result, seen);

            // Sibling ScavLib*.dll module assemblies sitting next to the core DLL.
            try
            {
                string coreFile = Assembly.GetExecutingAssembly().Location;
                string dir = Path.GetDirectoryName(coreFile);
                if (!string.IsNullOrEmpty(dir))
                {
                    foreach (string path in Directory.GetFiles(dir, "ScavLib*.dll"))
                    {
                        // Skip the core assembly file itself; it is already scanned.
                        if (Path.GetFileName(path).Equals(Path.GetFileName(coreFile),
                                StringComparison.OrdinalIgnoreCase))
                            continue;

                        try
                        {
                            CollectFromAssembly(Assembly.LoadFrom(path), result, seen);
                        }
                        catch (Exception ex)
                        {
                            ScavLibPlugin.Log.LogError(
                                $"[ScavLib] Failed to load module assembly " +
                                $"'{Path.GetFileName(path)}': {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError($"[ScavLib] Module assembly scan failed: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Adds every concrete <see cref="IScavLibModule"/> found in
        /// <paramref name="asm"/> to <paramref name="sink"/>, de-duplicated by full
        /// type name via <paramref name="seen"/> so a type reachable through more
        /// than one assembly path is only instantiated once.
        /// </summary>
        private static void CollectFromAssembly(Assembly asm, List<IScavLibModule> sink,
                                                HashSet<string> seen)
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Surface WHY types failed to load — this branch used to silently
                // swallow a module whose dependencies could not be resolved.
                ScavLibPlugin.Log.LogWarning(
                    $"[ScavLib] GetTypes() on '{asm.GetName().Name}' threw " +
                    $"ReflectionTypeLoadException; loader errors follow:");
                if (ex.LoaderExceptions != null)
                {
                    foreach (var le in ex.LoaderExceptions)
                        if (le != null)
                            ScavLibPlugin.Log.LogWarning($"[ScavLib]   loader: {le.Message}");
                }
                types = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (var t in types)
            {
                if (t == null || t.IsInterface || t.IsAbstract) continue;
                if (!typeof(IScavLibModule).IsAssignableFrom(t)) continue;
                if (!seen.Add(t.FullName)) continue; // de-dupe across assemblies

                try
                {
                    sink.Add((IScavLibModule)Activator.CreateInstance(t));
                }
                catch (Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[ScavLib] Failed to instantiate module type '{t.FullName}': {ex.Message}");
                }
            }
        }
    }
}