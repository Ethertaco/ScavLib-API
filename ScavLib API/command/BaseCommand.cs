using System.Collections.Generic;
using System.Text;

namespace ScavLib.command
{
    public abstract class BaseCommand
    {

        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual (string, string)[] ArgDescription => System.Array.Empty<(string, string)>();

        public virtual Dictionary<int, List<string>> ArgAutofill => null;

        public virtual Dictionary<string, BaseCommand> SubCommands => null;

        public abstract void Execute(string[] args);

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

        protected static void LogLine(string message)
        {
            util.GameUtil.Log(message);
        }

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
