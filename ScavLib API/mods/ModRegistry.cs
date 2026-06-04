using System;
using System.Collections.Generic;
using ScavLib.event_bus;
using ScavLib.event_bus.events;

namespace ScavLib.mods
{

    public static class ModRegistry
    {

        private static readonly List<ModSession> _orderedSessions = new List<ModSession>();
        private static readonly Dictionary<string, ModSession> _sessionsByName
            = new Dictionary<string, ModSession>(StringComparer.OrdinalIgnoreCase);

        public static void Register(ModInfo mod)
        {
            RegisterInternal(mod, null);
        }

        public static void Register(ModInfo mod, IModLifecycle lifecycle)
        {
            RegisterInternal(mod, lifecycle);
        }

        private static void RegisterInternal(ModInfo mod, IModLifecycle lifecycle)
        {
            if (mod == null) return;

            if (IsRegistered(mod.Name))
            {
                ScavLibPlugin.Log.LogWarning(
                    $"[ModRegistry] A mod named '{mod.Name}' is already registered. " +
                    $"Registering again — dependency lookups may be ambiguous.");
            }

            var session = new ModSession(mod, lifecycle);
            _orderedSessions.Add(session);

            if (!string.IsNullOrEmpty(mod.Name) && !_sessionsByName.ContainsKey(mod.Name))
                _sessionsByName[mod.Name] = session;

            if (lifecycle != null)
            {
                WireLifecycle(session);
            }

            CheckDependencies(mod);

            ScavLibPlugin.Log.LogInfo(
                $"[ModRegistry] Registered mod: {mod.Name} ({mod.Version})" +
                (lifecycle != null ? " [F]" : ""));

            if (lifecycle != null)
            {
                SafeInvoke(mod, "OnEnabled", () => lifecycle.OnEnabled());
            }
        }

        private static void WireLifecycle(ModSession session)
        {
            var adapter = new LifecycleEventAdapter(session.Info, session.Lifecycle);
            session.LifecycleAdapter = adapter;
            EventBus.Register(adapter);
        }

        private class LifecycleEventAdapter
        {
            private readonly ModInfo _mod;
            private readonly IModLifecycle _lifecycle;

            public LifecycleEventAdapter(ModInfo mod, IModLifecycle lifecycle)
            {
                _mod = mod;
                _lifecycle = lifecycle;
            }

            [Subscribe]
            private void OnWorldLoaded(WorldLoadedEvent e)
            {
                SafeInvoke(_mod, "OnWorldLoaded", () => _lifecycle.OnWorldLoaded(e));
            }

            [Subscribe]
            private void OnLayerLoaded(LayerLoadedEvent e)
            {
                SafeInvoke(_mod, "OnLayerLoaded", () => _lifecycle.OnLayerLoaded(e));
            }

            [Subscribe]
            private void OnWorldUnloading(WorldUnloadingEvent e)
            {
                SafeInvoke(_mod, "OnWorldUnloading", () => _lifecycle.OnWorldUnloading(e));
            }

            [Subscribe]
            private void OnWorldDestroyed(WorldDestroyedEvent e)
            {
                SafeInvoke(_mod, "OnWorldDestroyed", () => _lifecycle.OnWorldDestroyed(e));
            }
        }

        private static void SafeInvoke(ModInfo mod, string callbackName, Action body)
        {
            try
            {
                body();
            }
            catch (Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ModRegistry] '{mod.Name}'.{callbackName}() threw: {ex}");
            }
        }

        private static void CheckDependencies(ModInfo mod)
        {
            var deps = mod.VersionedDependencies;
            if (deps == null || deps.Length == 0) return;

            foreach (var dep in deps)
            {
                if (string.IsNullOrEmpty(dep.Name)) continue;

                ModSession depSession;
                if (!_sessionsByName.TryGetValue(dep.Name, out depSession))
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[ModRegistry] '{mod.Name}' declares dependency on " +
                        $"'{dep}', but no such mod is registered yet. If load " +
                        $"order matters, use BepInEx [BepInDependency] to enforce it.");
                    continue;
                }

                if (!dep.IsSatisfiedBy(depSession.Info.Version))
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[ModRegistry] '{mod.Name}' requires {dep}, but the " +
                        $"installed version of '{dep.Name}' is " +
                        $"'{depSession.Info.Version}'. The mod will continue " +
                        $"to load, but compatibility is not guaranteed.");
                }
            }
        }

        public static IReadOnlyList<ModInfo> GetAll()
        {
            var result = new List<ModInfo>(_orderedSessions.Count);
            foreach (var session in _orderedSessions)
                result.Add(session.Info);
            return result.AsReadOnly();
        }

        public static bool TryFind(string name, out ModInfo info)
        {
            if (string.IsNullOrEmpty(name))
            {
                info = null;
                return false;
            }
            ModSession session;
            if (_sessionsByName.TryGetValue(name, out session))
            {
                info = session.Info;
                return true;
            }
            info = null;
            return false;
        }

        public static bool IsRegistered(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return _sessionsByName.ContainsKey(name);
        }

        public static IModLifecycle GetLifecycle(ModInfo mod)
        {
            if (mod == null) return null;
            var session = FindSession(mod);
            return session != null ? session.Lifecycle : null;
        }

        public static bool HasLifecycle(ModInfo mod)
        {
            if (mod == null) return false;
            var session = FindSession(mod);
            return session != null && session.Lifecycle != null;
        }

        public static ModSession GetSession(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            ModSession session;
            return _sessionsByName.TryGetValue(name, out session) ? session : null;
        }

        private static ModSession FindSession(ModInfo mod)
        {

            if (mod == null || string.IsNullOrEmpty(mod.Name)) return null;

            ModSession session;
            if (_sessionsByName.TryGetValue(mod.Name, out session))
            {

                if (ReferenceEquals(session.Info, mod)) return session;
            }

            foreach (var s in _orderedSessions)
            {
                if (ReferenceEquals(s.Info, mod)) return s;
            }
            return null;
        }
    }
}
