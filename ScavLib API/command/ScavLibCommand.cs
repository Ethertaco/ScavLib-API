using System.Collections.Generic;
using System.Text;
using ScavLib.mods;

namespace ScavLib.command
{

    public class ScavLibCommand : BaseCommand
    {
        public override string Name => "scavlib";
        public override string Description => "ScavLib management commands.";

        public override (string, string)[] ArgDescription => new (string, string)[]
        {
            ("string action", "Action to perform. Try 'scavlib help'.")
        };

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

                LogLine("  No conflicts detected.");
            }
        }
    }
}
