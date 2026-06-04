using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScavLib.event_bus
{

    public static class EventBus
    {

        private static readonly Dictionary<Type, List<(object target, MethodInfo method)>> _handlers
            = new Dictionary<Type, List<(object, MethodInfo)>>();

        public static void Register(object listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            UnregisterInternal(listener, silent: true);

            var methods = listener.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            int count = 0;
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<SubscribeAttribute>() == null) continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1 || !typeof(BusEvent).IsAssignableFrom(parameters[0].ParameterType))
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[EventBus] Method '{method.Name}' in '{listener.GetType().Name}' " +
                        $"has [Subscribe] but invalid signature. Expected exactly one BusEvent parameter.");
                    continue;
                }

                var eventType = parameters[0].ParameterType;
                if (!_handlers.ContainsKey(eventType))
                    _handlers[eventType] = new List<(object, MethodInfo)>();

                _handlers[eventType].Add((listener, method));
                count++;
            }

            ScavLibPlugin.Log.LogInfo(
                $"[EventBus] Registered {count} handler(s) from '{listener.GetType().Name}'.");
        }

        public static void Unregister(object listener)
        {
            UnregisterInternal(listener, silent: false);
        }

        private static void UnregisterInternal(object listener, bool silent)
        {
            if (listener == null) return;

            int removed = 0;
            foreach (var list in _handlers.Values)
                removed += list.RemoveAll(entry => entry.target == listener);

            if (!silent)
            {
                ScavLibPlugin.Log.LogInfo(
                    $"[EventBus] Unregistered {removed} handler(s) from '{listener.GetType().Name}'.");
            }
        }

        public static void Post<T>(T busEvent) where T : BusEvent
        {
            if (busEvent == null) throw new ArgumentNullException(nameof(busEvent));

            var type = busEvent.GetType();
            while (type != null && typeof(BusEvent).IsAssignableFrom(type))
            {
                if (_handlers.TryGetValue(type, out var handlers))
                {

                    var snapshot = new List<(object target, MethodInfo method)>(handlers);
                    foreach (var (target, method) in snapshot)
                    {
                        try
                        {
                            method.Invoke(target, new object[] { busEvent });
                        }
                        catch (Exception ex)
                        {
                            ScavLibPlugin.Log.LogError(
                                $"[EventBus] Handler '{method.Name}' in " +
                                $"'{target.GetType().Name}' threw while processing " +
                                $"'{busEvent.GetType().Name}': {ex.InnerException ?? ex}");
                        }
                    }
                }

                if (type == typeof(BusEvent)) break;
                type = type.BaseType;
            }
        }

        public static int GetHandlerCount(Type eventType)
        {
            if (eventType == null) return 0;
            return _handlers.TryGetValue(eventType, out var list) ? list.Count : 0;
        }

        public static int GetHandlerCount<T>() where T : BusEvent
        {
            return GetHandlerCount(typeof(T));
        }
    }
}
