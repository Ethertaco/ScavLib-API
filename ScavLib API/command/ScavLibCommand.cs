using System.Collections.Generic;
using System.Text;
using ScavLib.mods;

namespace ScavLib.command
{
    /// <summary>
    /// Built-in <c>scavlib</c> management command. Doubles as the canonical
    /// example of <see cref="BaseCommand.SubCommands"/> in a real codebase.
    ///
    /// <para>Subcommands:
    /// <list type="bullet">
    ///   <item><description><c>status</c> — version + registered mods.</description></item>
    ///   <item><description><c>check</c> — diagnostic dump (patches / commands / keybinds).</description></item>
    ///   <item><description><c>help</c> / <c>?</c> / <c>--help</c> — auto-routed by
    ///   <see cref="BaseCommand.ExecuteSubCommand"/> to the usage printer.</description></item>
    /// </list></para>
    /// </summary>
    public class ScavLibCommand : BaseCommand
    {
        public override string Name => "scavlib";
        public override string Description => "ScavLib management commands.";

        public override (string, string)[] ArgDescription => new (string, string)[]
        {
            ("string action", "Action to perform. Try 'scavlib help'.")
        };

        // Keys are lowercase; ScavLib auto-merges them into ArgAutofill[0]
        // for Tab completion.
        private readonly Dictionary<string, BaseCommand> _subs;

        public ScavLibCommand()
        {
            _subs = new Dictionary<string, BaseCommand>
            {
                { "status", new StatusSubCommand() },
                { "check",  new CheckSubCommand()  },
            };
        }

        public override Dictionary<string, BaseCommand> SubCommands => _subs;

        public override void Execute(string[] args)
        {
            ExecuteSubCommand(args, subArgIndex: 1);
        }

        // --- status ---------------------------------------------------------------

        /// <summary>
        /// Prints version, authors, and the list of mods registered with
        /// <c>ModRegistry</c>. <c>[F]</c> tags entries that have a lifecycle
        /// implementation; <c>Deps:</c> lists declared dependencies.
        /// </summary>
        private class StatusSubCommand : BaseCommand
        {
            public override string Name => "status";
            public override string Description =>
                "Show ScavLib version and registered mods.";

            public override void Execute(string[] args)
            {
                var mods = ModRegistry.GetAll();

                LogLine("ScavLib Activated.");
                LogLine($"Version: {ScavLibPlugin.Version}");
                LogLine("Authors: Kanisuko / QinShenYu");

                if (mods.Count == 0)
                {
                    LogLine("Registered Mods: (none)");
                    return;
                }

                LogLine($"Registered Mods ({mods.Count}):");
                foreach (var mod in mods)
                {
                    var sb = new StringBuilder();
                    sb.Append("  ");
                    sb.Append(mod.ToString());

                    if (ModRegistry.HasLifecycle(mod))
                        sb.Append(" [F]");

                    if (mod.Dependencies != null && mod.Dependencies.Length > 0)
                    {
                        sb.Append(" Deps: [");
                        sb.Append(string.Join(", ", mod.Dependencies));
                        sb.Append("]");
                    }

                    LogLine(sb.ToString());
                }
            }
        }

        // --- check ----------------------------------------------------------------

        /// <summary>
        /// Diagnostic dump used to attach to bug reports. Three sections:
        /// <list type="number">
        ///   <item><description>Harmony patch status from
        ///   <c>ScavLibPlugin.PatchStatus</c> / <c>PatchErrors</c>. Reflects
        ///   patch <i>registration</i>, not whether the patched code path has
        ///   actually run.</description></item>
        ///   <item><description>Commands ScavLib injected via
        ///   <see cref="CommandRegistry.GetAllRegistered"/> with their owners.</description></item>
        ///   <item><description>KeyBinds registered via ScavLib, with the
        ///   currently-bound <see cref="UnityEngine.KeyCode"/> resolved through
        ///   <c>KeyBinds.GetBind</c> (returns <c>KeyCode.None</c> when unbound,
        ///   in which case the row is excluded from the same-key collision
        ///   tally). Rows sharing a non-<c>None</c> key with another binding
        ///   get a <c>[!]</c> marker.</description></item>
        /// </list>
        /// </summary>
        private class CheckSubCommand : BaseCommand
        {
            public override string Name => "check";
            public override string Description =>
                "Diagnostic report: patch status, command owners, conflicts.";

            public override void Execute(string[] args)
            {
                LogLine($"ScavLib Check (v{ScavLibPlugin.Version})");

                LogLine("  Harmony Patches:");
                if (ScavLibPlugin.PatchStatus.Count == 0)
                {
                    LogLine("    (no patches recorded — startup may have failed)");
                }
                else
                {
                    foreach (var kv in ScavLibPlugin.PatchStatus)
                    {
                        string tag = kv.Value ? "[OK]" : "[FAIL]";
                        string suffix = "";
                        if (!kv.Value &&
                            ScavLibPlugin.PatchErrors.TryGetValue(kv.Key, out var err))
                        {
                            suffix = $" — {err}";
                        }
                        LogLine($"    {tag} {kv.Key}{suffix}");
                    }
                }
                LogLine("    (status reflects patch registration, not runtime activity.)");

                var commands = CommandRegistry.GetAllRegistered();
                LogLine($"  Commands (via ScavLib): {commands.Count}");
                foreach (var (name, owner) in commands)
                {
                    string ownerStr = owner ?? "<no owner>";
                    LogLine($"    {name} (owner: {ownerStr})");
                }

                var keybinds = ScavLib.input.KeyBindRegistry.GetAllRegistered();
                LogLine($"  KeyBinds (via ScavLib): {keybinds.Count}");

                // Tally how many bindings share each non-None KeyCode so we
                // can flag collisions in the per-row dump below.
                var byKey = new System.Collections.Generic.Dictionary<UnityEngine.KeyCode, int>();
                foreach (var def in keybinds)
                {
                    var kc = KeyBinds.GetBind(def.FullId);
                    if (kc == UnityEngine.KeyCode.None) continue;
                    byKey[kc] = byKey.TryGetValue(kc, out var n) ? n + 1 : 1;
                }

                foreach (var def in keybinds)
                {
                    var kc = KeyBinds.GetBind(def.FullId);
                    string clash = "";
                    if (kc != UnityEngine.KeyCode.None &&
                        byKey.TryGetValue(kc, out var n) && n > 1)
                        clash = " [!]";
                    string cat = string.IsNullOrEmpty(def.Category) ? "<none>" : def.Category;
                    LogLine($"    {def.FullId} → {kc} (owner: {def.OwnerModName}, cat: {cat}){clash}");
                }
                if (byKey.Count != keybinds.Count)
                    LogLine("    [!] = key shared with another binding.");

                LogLine("  No conflicts detected.");
            }
        }
    }
}
