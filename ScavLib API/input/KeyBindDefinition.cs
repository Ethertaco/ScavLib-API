using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.input
{
    public sealed class KeyBindDefinition
    {
        public string OwnerModName { get; }
        public string LocalId { get; }
        public string FullId { get; }
        public KeyCode DefaultKey { get; }
        public string Category { get; }
        public IReadOnlyDictionary<string, string> DisplayNames { get; }
        public IReadOnlyDictionary<string, string> Descriptions { get; }

        private readonly List<Action> _handlers = new List<Action>();
        public IReadOnlyList<Action> Handlers => _handlers;

        public Action OnPressed => _handlers.Count == 0 ? null : (Action)InvokeAll;

        private void InvokeAll()
        {

            Action[] snapshot = _handlers.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                try { snapshot[i]?.Invoke(); }
                catch (Exception ex)
                {
                    ScavLibPlugin.Log.LogError(
                        $"[KeyBindDefinition] Handler #{i} on '{FullId}' threw: {ex}");
                }
            }
        }

        internal void AddHandler(Action h)
        {
            if (h != null && !_handlers.Contains(h)) _handlers.Add(h);
        }

        internal bool RemoveHandler(Action h) => _handlers.Remove(h);
        internal void ClearHandlers() => _handlers.Clear();

        internal KeyBindDefinition(
            string ownerModName,
            string localId,
            string fullId,
            KeyCode defaultKey,
            string category,
            Action onPressed,
            Dictionary<string, string> displayNames,
            Dictionary<string, string> descriptions)
        {
            OwnerModName = ownerModName;
            LocalId = localId;
            FullId = fullId;
            DefaultKey = defaultKey;
            Category = category;
            if (onPressed != null) _handlers.Add(onPressed);
            DisplayNames = displayNames ?? new Dictionary<string, string>();
            Descriptions = descriptions ?? new Dictionary<string, string>();
        }

        public override string ToString()
            => $"{FullId} (owner: {OwnerModName}, default: {DefaultKey}, " +
               $"category: {Category ?? "<none>"}, handlers: {_handlers.Count})";
    }
}
