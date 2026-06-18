using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScavLib.input
{

    public class KeyBindBuilder
    {
        private const string DefaultLang = "EN";

        private readonly string _ownerModName;
        private readonly string _localId;
        private KeyCode _defaultKey = KeyCode.None;
        private string _category;
        private Action _onPressed;
        private readonly Dictionary<string, string> _names = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _descs = new Dictionary<string, string>();

        private KeyBindBuilder(string ownerModName, string localId)
        {
            _ownerModName = ownerModName;
            _localId = localId;
        }

        public static KeyBindBuilder Create(string ownerModName, string localId)
            => new KeyBindBuilder(ownerModName, localId);

        public KeyBindBuilder Default(KeyCode key) { _defaultKey = key; return this; }

        public KeyBindBuilder DisplayName(string en) { _names[DefaultLang] = en; return this; }
        public KeyBindBuilder DisplayName(IDictionary<string, string> byLang) { CopyInto(byLang, _names); return this; }
        public KeyBindBuilder Description(string en) { _descs[DefaultLang] = en; return this; }
        public KeyBindBuilder Description(IDictionary<string, string> byLang) { CopyInto(byLang, _descs); return this; }

        public KeyBindBuilder Category(string category) { _category = category; return this; }

        public KeyBindBuilder OnPressed(Action handler) { _onPressed = handler; return this; }

        public bool Register() => Register(out _);

        public bool Register(out string error)
        {
            error = null;
            string fullId = KeyBindRegistry.BuildFullId(_ownerModName, _localId);
            if (fullId == null)
            {
                error = $"Invalid owner/localId after sanitization " +
                        $"(owner='{_ownerModName}', localId='{_localId}').";
                ScavLibPlugin.Log.LogError($"[KeyBindBuilder] {error}");
                return false;
            }

            var def = new KeyBindDefinition(
                ownerModName: _ownerModName,
                localId: _localId,
                fullId: fullId,
                defaultKey: _defaultKey,
                category: _category,
                onPressed: _onPressed,
                displayNames: new Dictionary<string, string>(_names),
                descriptions: new Dictionary<string, string>(_descs));

            return KeyBindRegistry.TryRegister(def, out error);
        }

        private static void CopyInto(IDictionary<string, string> src,
                                     Dictionary<string, string> dst)
        {
            if (src == null) return;
            foreach (var kv in src) dst[kv.Key] = kv.Value;
        }
    }
}
