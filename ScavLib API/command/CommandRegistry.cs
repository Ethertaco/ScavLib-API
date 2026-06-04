using System;
using System.Collections.Generic;

namespace ScavLib.command
{

    public static class CommandRegistry
    {

        private static readonly List<(BaseCommand cmd, string owner)> _pending
            = new List<(BaseCommand, string)>();

        private static readonly Dictionary<string, string> _ownerMap
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, Command> _injected
            = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);

        internal static void Init()
        {

        }

        public static void Register(BaseCommand command)
        {
            TryRegister(command, null, out _);
        }

        public static bool TryRegister(BaseCommand command, string ownerModName,
                                       out string error)
        {
            error = null;

            if (command == null)
            {
                error = "Command instance is null.";
                ScavLibPlugin.Log.LogError($"[CommandRegistry] {error}");
                return false;
            }

            if (!BaseCommand.ValidateName(command.Name, out error))
            {
                ScavLibPlugin.Log.LogError($"[CommandRegistry] {error}");
                return false;
            }

            if (command.SubCommands != null && command.SubCommands.Count > 0)
            {
                var argDesc = command.ArgDescription;
                if (argDesc != null && argDesc.Length > 0)
                {
                    var firstArgType = argDesc[0].Item1 ?? "";
                    if (firstArgType.StartsWith("bool") ||
                        firstArgType.StartsWith("position"))
                    {
                        error = $"Command '{command.Name}' has SubCommands but " +
                                $"its first ArgDescription is '{firstArgType}'. " +
                                $"The game's Command constructor will auto-inject " +
                                $"autofill candidates for that type, colliding with " +
                                $"ScavLib's subcommand-name injection. Either remove " +
                                $"SubCommands or change the first arg type.";
                        ScavLibPlugin.Log.LogError($"[CommandRegistry] {error}");
                        return false;
                    }
                }
            }

            if (ConsoleScript.instance != null)
            {
                return InjectCommand(command, ownerModName, out error);
            }

            _pending.Add((command, ownerModName));
            ScavLibPlugin.Log.LogInfo(
                $"[CommandRegistry] Queued command: {command.Name}" +
                (ownerModName != null ? $" (owner: {ownerModName})" : ""));
            return true;
        }

        public static bool Unregister(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ScavLibPlugin.Log.LogWarning(
                    "[CommandRegistry] Unregister called with null/empty name.");
                return false;
            }

            int pendingIdx = _pending.FindIndex(p =>
                p.cmd != null &&
                string.Equals(p.cmd.Name, name, StringComparison.OrdinalIgnoreCase));
            if (pendingIdx >= 0)
            {
                _pending.RemoveAt(pendingIdx);
                ScavLibPlugin.Log.LogInfo(
                    $"[CommandRegistry] Removed pending command: {name}");
                return true;
            }

            if (!_injected.TryGetValue(name, out var gameCommand) ||
                gameCommand == null)
            {
                ScavLibPlugin.Log.LogWarning(
                    $"[CommandRegistry] Cannot unregister '{name}' — not " +
                    $"registered through ScavLib (game-native commands are " +
                    $"protected from removal).");
                return false;
            }

            ConsoleScript.Commands.Remove(gameCommand);
            _injected.Remove(name);
            _ownerMap.Remove(name);

            ScavLibPlugin.Log.LogInfo(
                $"[CommandRegistry] Unregistered command: {name}");
            return true;
        }

        public static string GetOwner(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return _ownerMap.TryGetValue(name, out var owner) ? owner : null;
        }

        public static IReadOnlyList<(string name, string owner)> GetAllRegistered()
        {
            var result = new List<(string, string)>();
            foreach (var kv in _ownerMap)
                result.Add((kv.Key, kv.Value));
            return result;
        }

        public static void FlushPending()
        {
            foreach (var (cmd, owner) in _pending)
            {
                InjectCommand(cmd, owner, out _);
            }
            _pending.Clear();
        }

        private static bool InjectCommand(BaseCommand command, string ownerModName,
                                          out string error)
        {
            error = null;

            if (ConsoleScript.SearchExact(command.Name) != null)
            {
                string existingOwner = GetOwner(command.Name);
                error = $"Command '{command.Name}' is already registered" +
                        (existingOwner != null
                            ? $" by mod '{existingOwner}'."
                            : " (likely a built-in or non-ScavLib command).");
                ScavLibPlugin.Log.LogWarning($"[CommandRegistry] {error}");
                return false;
            }

            var mergedAutofill = MergeSubCommandAutofill(command);

            try
            {
                var gameCommand = new Command(
                    command.Name,
                    command.Description,
                    args => SafeExecute(command, args),
                    mergedAutofill,
                    command.ArgDescription
                );

                ConsoleScript.Commands.Add(gameCommand);
                _injected[command.Name] = gameCommand;
                _ownerMap[command.Name] = ownerModName;

                ScavLibPlugin.Log.LogInfo(
                    $"[CommandRegistry] Registered command: {command.Name}" +
                    (ownerModName != null ? $" (owner: {ownerModName})" : ""));
                return true;
            }
            catch (Exception ex)
            {
                error = $"Failed to construct game Command for '{command.Name}': {ex.Message}";
                ScavLibPlugin.Log.LogError($"[CommandRegistry] {error}");
                return false;
            }
        }

        private static Dictionary<int, List<string>> MergeSubCommandAutofill(
            BaseCommand command)
        {
            var userAutofill = command.ArgAutofill;
            var subs = command.SubCommands;

            if ((subs == null || subs.Count == 0) && userAutofill == null)
                return null;

            var result = userAutofill != null
                ? new Dictionary<int, List<string>>(userAutofill)
                : new Dictionary<int, List<string>>();

            if (subs != null && subs.Count > 0)
            {

                List<string> existing;
                if (!result.TryGetValue(0, out existing))
                    existing = null;

                var merged = existing != null
                    ? new List<string>(existing)
                    : new List<string>();

                foreach (var key in subs.Keys)
                {
                    if (!merged.Contains(key))
                        merged.Add(key);
                }

                result[0] = merged;
            }

            return result;
        }

        private static void SafeExecute(BaseCommand command, string[] args)
        {
            try
            {
                command.Execute(args);
            }
            catch (Exception ex)
            {
                string owner = GetOwner(command.Name) ?? "<unknown>";
                ScavLibPlugin.Log.LogError(
                    $"[CommandRegistry] Command '{command.Name}' " +
                    $"(owner: {owner}) threw: {ex}");
                throw;
            }
        }
    }
}
