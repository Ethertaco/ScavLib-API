using System;
using System.Collections.Generic;

namespace ScavLib.command
{
    /// <summary>
    /// Central injection point for ScavLib-managed console commands.
    ///
    /// <para>Maintains three internal tables:
    /// <list type="bullet">
    ///   <item><description><b>_pending</b> — commands queued before
    ///   <c>ConsoleScript.instance</c> existed; flushed by
    ///   <see cref="CommandRegistryPatch"/>.</description></item>
    ///   <item><description><b>_ownerMap</b> — command name → owning mod display name,
    ///   used by <see cref="GetOwner"/> and the <c>scavlib check</c> diagnostic.</description></item>
    ///   <item><description><b>_injected</b> — command name → the actual game
    ///   <see cref="Command"/> instance we Added to <c>ConsoleScript.Commands</c>.
    ///   Needed because <see cref="Unregister"/> removes by reference.</description></item>
    /// </list></para>
    ///
    /// <para>Vanilla protection is implicit: a command is removable only if it
    /// appears in <c>_injected</c>. Game-native commands are never put there,
    /// so they cannot be deleted through <see cref="Unregister"/>. No
    /// hardcoded protection list is required.</para>
    /// </summary>
    public static class CommandRegistry
    {
        private static readonly List<(BaseCommand cmd, string owner)> _pending
            = new List<(BaseCommand, string)>();

        private static readonly Dictionary<string, string> _ownerMap
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, Command> _injected
            = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Reserved bootstrap hook. Currently a no-op.</summary>
        internal static void Init()
        {
        }

        // --- register -------------------------------------------------------------

        /// <summary>
        /// Convenience overload that registers without an owner string and
        /// discards the failure reason. Equivalent to
        /// <c>TryRegister(command, null, out _)</c>.
        /// </summary>
        public static void Register(BaseCommand command)
        {
            TryRegister(command, null, out _);
        }

        /// <summary>
        /// Register a command with explicit ownership and detailed failure info.
        /// </summary>
        /// <param name="command">The command to register. Null is rejected.</param>
        /// <param name="ownerModName">
        /// Display name of the owning mod (recommended: same string used in
        /// <c>ModRegistry.Register</c>). Pass <c>null</c> to opt out of the
        /// owner ledger — registration still succeeds.
        /// </param>
        /// <param name="error">Reason on failure, or <c>null</c> on success.</param>
        /// <returns>
        /// <c>true</c> when the command is either injected immediately or
        /// queued for later flush; <c>false</c> when validation rejected it.
        /// </returns>
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

            // Refuse the SubCommands + bool/position-first-arg combination:
            // vanilla Command ctor will Add() autofill for that index, then
            // MergeSubCommandAutofill would Add() the same index again and throw.
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

        // --- unregister -----------------------------------------------------------

        /// <summary>
        /// Remove a previously-ScavLib-registered command from
        /// <c>ConsoleScript.Commands</c>. Pending (un-injected) commands are
        /// also handled.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a command was actually removed. <c>false</c> when
        /// the name is empty, unknown, or refers to a game-native command
        /// (those are not in the ledger and are therefore protected).
        /// </returns>
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

        // --- queries --------------------------------------------------------------

        /// <summary>
        /// Owner mod name for a ScavLib-registered command, or <c>null</c>
        /// when the command is unknown to ScavLib (i.e. game-native, or
        /// registered without an owner string).
        /// </summary>
        public static string GetOwner(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return _ownerMap.TryGetValue(name, out var owner) ? owner : null;
        }

        /// <summary>
        /// Snapshot of all commands ScavLib has injected, paired with their
        /// owner (which may be <c>null</c>). Order is unspecified.
        /// </summary>
        public static IReadOnlyList<(string name, string owner)> GetAllRegistered()
        {
            var result = new List<(string, string)>();
            foreach (var kv in _ownerMap)
                result.Add((kv.Key, kv.Value));
            return result;
        }

        // --- injection ------------------------------------------------------------

        /// <summary>
        /// Drain the pending queue. Invoked by <see cref="CommandRegistryPatch"/>
        /// in the Postfix of <c>ConsoleScript.Start</c>, which itself runs only
        /// after vanilla <c>RegisterAllCommands</c> (the latter is gated on
        /// <c>Commands.Count == 0</c>), so duplicate-name checks here see the
        /// full vanilla list.
        /// </summary>
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

            // Merge BEFORE handing the dictionary to the game ctor — the ctor
            // does argAutofill.Add(i, ...) for bool/position prefixes and
            // throws on duplicate keys.
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

        /// <summary>
        /// Build the autofill dictionary handed to the vanilla
        /// <see cref="Command"/> constructor. Starts from a defensive copy of
        /// the user's <see cref="BaseCommand.ArgAutofill"/> (so the mod's own
        /// data is never mutated) and merges <see cref="BaseCommand.SubCommands"/>
        /// keys into index 0, deduped.
        /// </summary>
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

        /// <summary>
        /// Wrap user code so we can attribute exceptions to the owning mod
        /// in our own log channel. The exception is re-thrown so the vanilla
        /// console's own try/catch still produces the inline error message
        /// the player sees.
        /// </summary>
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
