using System.Collections.Generic;
using System.Text;

namespace ScavLib.command
{
    /// <summary>
    /// Base class every ScavLib-managed console command inherits from.
    ///
    /// <para>This abstraction shields mod authors from the raw vanilla
    /// <see cref="Command"/> constructor (positional args, tuple-typed
    /// description array, dictionary autofill that throws on duplicate keys)
    /// and adds two features the vanilla type does not have natively:
    /// owner attribution and nested subcommand routing.</para>
    /// </summary>
    public abstract class BaseCommand
    {
        /// <summary>
        /// Command keyword as typed in the console. Must contain no spaces
        /// (the game tokenizes input on whitespace) and must not collide
        /// with a built-in command — see <see cref="ValidateName"/>.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>One-line description shown by the vanilla `help` command.</summary>
        public abstract string Description { get; }

        /// <summary>
        /// Per-argument descriptors, one entry per positional argument,
        /// in the form <c>(shortLabel, longLabel)</c>. The short label is
        /// also what the vanilla constructor scans to auto-inject autofill
        /// candidates:
        /// <list type="bullet">
        ///   <item><description><c>"bool ..."</c> → injects <c>true</c> / <c>false</c>.</description></item>
        ///   <item><description><c>"position ..."</c> → injects <c>cursor</c> / <c>player</c> / <c>random</c> / <c>#.#</c>.</description></item>
        /// </list>
        /// <para>Important: those two prefixes call <c>argAutofill.Add(i, ...)</c>,
        /// not the indexer. If you also list candidates for the same index in
        /// <see cref="ArgAutofill"/>, registration will throw
        /// <see cref="System.ArgumentException"/> at construction time.</para>
        /// </summary>
        public virtual (string, string)[] ArgDescription => System.Array.Empty<(string, string)>();

        /// <summary>
        /// Tab-completion candidates per argument index. Key 0 is the first
        /// user-supplied argument (i.e. <c>args[1]</c> inside <see cref="Execute"/>),
        /// matching the game's lookup formula <c>argAutofill[args.Length - 2]</c>
        /// inside <c>ConsoleScript.TryFinishCommandPart</c>.
        /// </summary>
        public virtual Dictionary<int, List<string>> ArgAutofill => null;

        /// <summary>
        /// Optional first-level subcommand table. When non-null,
        /// <see cref="ExecuteSubCommand"/> dispatches the call by looking up
        /// <c>args[1]</c> here.
        ///
        /// <para>Keys must be lowercase — the router lowercases the incoming
        /// arg before lookup so users can type any case. ScavLib also auto-merges
        /// these keys into <see cref="ArgAutofill"/>[0] so first-level subcommand
        /// names get Tab completion for free (see <c>CommandRegistry.MergeSubCommandAutofill</c>).</para>
        ///
        /// <para>Limitations:
        /// <list type="bullet">
        ///   <item><description>Only the <b>first</b> level is auto-completed.
        ///   The vanilla console only consults the top-level command's autofill;
        ///   second-level subcommand names must be discovered via your own help text.</description></item>
        ///   <item><description>Conflicts with <c>bool</c>/<c>position</c> as the
        ///   first arg type. The vanilla constructor will inject autofill candidates
        ///   for that index using <c>Add()</c>; merging subcommand keys at the same
        ///   index would duplicate-key throw. <c>CommandRegistry.TryRegister</c>
        ///   refuses such combinations explicitly.</description></item>
        /// </list></para>
        /// </summary>
        public virtual Dictionary<string, BaseCommand> SubCommands => null;

        /// <summary>
        /// Command body. <c>args[0]</c> is the command name itself; user-supplied
        /// arguments start at <c>args[1]</c>. Throwing here is safe — the vanilla
        /// console wraps the call in try/catch and displays the message inline.
        /// </summary>
        public abstract void Execute(string[] args);

        // --- subcommand routing ---------------------------------------------------

        /// <summary>
        /// Default subcommand dispatcher. Call this from <see cref="Execute"/>
        /// when <see cref="SubCommands"/> is non-null. Behavior:
        /// <list type="bullet">
        ///   <item><description>No subcommand part → print usage.</description></item>
        ///   <item><description><c>help</c> / <c>?</c> / <c>--help</c> → print usage.</description></item>
        ///   <item><description>Unknown key → error line + usage.</description></item>
        ///   <item><description>Known key → forward the full <paramref name="args"/>
        ///   array to the child's <see cref="Execute"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="args">The args array passed to <see cref="Execute"/>.</param>
        /// <param name="subArgIndex">
        /// Position of the subcommand keyword. Top-level commands pass 1
        /// (<c>args[0]</c> is the command name); a nested group should pass
        /// <c>subArgIndex + 1</c> when calling further down.
        /// </param>
        protected void ExecuteSubCommand(string[] args, int subArgIndex = 1)
        {
            var subs = SubCommands;
            if (subs == null || subs.Count == 0)
            {
                LogLine($"[{Name}] No subcommands available.");
                return;
            }

            if (args.Length <= subArgIndex)
            {
                PrintUsage(args, subArgIndex);
                return;
            }

            string key = args[subArgIndex].ToLower();

            if (key == "help" || key == "?" || key == "--help")
            {
                PrintUsage(args, subArgIndex);
                return;
            }

            if (!subs.TryGetValue(key, out var sub))
            {
                LogLine($"[{Name}] Unknown subcommand '{args[subArgIndex]}'.");
                PrintUsage(args, subArgIndex);
                return;
            }

            try
            {
                sub.Execute(args);
            }
            catch (System.Exception ex)
            {
                LogLine($"[{Name}] Subcommand '{key}' threw: {ex.Message}");
                ScavLibPlugin.Log.LogError(
                    $"[BaseCommand] '{Name} {key}' raised: {ex}");
            }
        }

        /// <summary>
        /// Print a help block for the current subcommand level. Override to
        /// customize formatting. The default rebuilds the typed path from
        /// <paramref name="args"/> so nested groups read naturally.
        /// </summary>
        protected virtual void PrintUsage(string[] args, int subArgIndex)
        {
            var subs = SubCommands;

            var path = new StringBuilder();
            for (int i = 0; i < subArgIndex && i < args.Length; i++)
            {
                if (i > 0) path.Append(' ');
                path.Append(args[i]);
            }

            LogLine($"Usage: {path} <subcommand> [args...]");

            if (subs == null || subs.Count == 0)
            {
                LogLine("  (no subcommands available)");
                return;
            }

            LogLine("Subcommands:");
            int maxKeyLen = 0;
            foreach (var kv in subs)
                if (kv.Key.Length > maxKeyLen) maxKeyLen = kv.Key.Length;

            foreach (var kv in subs)
            {
                string padding = new string(' ', maxKeyLen - kv.Key.Length + 2);
                LogLine($"  {kv.Key}{padding}- {kv.Value.Description}");
            }
        }

        /// <summary>
        /// Append one line to the in-game console. Routed through
        /// <c>GameUtil.Log</c> so multi-line strings split correctly.
        /// </summary>
        protected static void LogLine(string message)
        {
            util.GameUtil.Log(message);
        }

        // --- name validation ------------------------------------------------------

        /// <summary>
        /// Hand-maintained snapshot of names registered by
        /// <c>ConsoleScript.RegisterAllCommands</c>. Used only to produce a
        /// clearer error than the registry's generic duplicate check would —
        /// the duplicate check itself is the real safeguard, so a stale
        /// entry here cannot cause incorrect behavior, only suboptimal error text.
        /// May drift slightly from the live game build; sync as needed.
        /// </summary>
        internal static readonly HashSet<string> BuiltinGameCommands
            = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "help", "heal", "coagulate", "kill", "spawn", "spawncategory",
            "tp", "skiplayer", "skiptext", "log", "talk", "framerate", "alert",
            "volume", "saveandquit", "resetskills", "fucklore", "timescale",
            "setconsoleheight", "setconsolecolor", "copylog", "clear", "addxp",
            "loglocale", "nukeplayerprefs", "openfolder", "setbodyfield",
            "setlimbfield", "amputate", "unchipped", "pixelate",
            "addcustomcommand", "addliquid", "locate", "removecustomcommand",
            "music", "bind", "repeat", "explode", "echo", "ui", "freecam",
            "starterkit", "noclip", "playsound", "fullbright", "plushies",
            "errorlogging","floodfill",
        };

        /// <summary>
        /// Validate a command name before registration.
        /// <list type="bullet">
        ///   <item><description>Null/empty → reject.</description></item>
        ///   <item><description>Contains whitespace → reject (game tokenizer splits on spaces).</description></item>
        ///   <item><description>Collides with a built-in name → reject.</description></item>
        ///   <item><description>No <c>_</c> in the name and not literally <c>scavlib</c> →
        ///   emit a soft warning recommending the <c>modname_command</c> convention,
        ///   but allow registration to proceed.</description></item>
        /// </list>
        /// </summary>
        internal static bool ValidateName(string name, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(name))
            {
                error = "Command name is null or empty.";
                return false;
            }

            if (name.Contains(" "))
            {
                error = $"Command name '{name}' contains spaces, which the " +
                        $"game console does not support.";
                return false;
            }

            if (BuiltinGameCommands.Contains(name))
            {
                error = $"Command name '{name}' collides with a built-in game " +
                        $"command. Please rename your command (a mod-specific " +
                        $"prefix like 'mymod_{name}' is recommended).";
                return false;
            }

            if (!name.Contains("_") && !name.Equals("scavlib",
                    System.StringComparison.OrdinalIgnoreCase))
            {
                ScavLibPlugin.Log.LogWarning(
                    $"[BaseCommand] Command name '{name}' has no underscore " +
                    $"prefix. To minimize conflicts with other mods, the " +
                    $"convention is '<modname>_<commandname>'.");
            }

            return true;
        }
    }
}
