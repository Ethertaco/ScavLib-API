using System.IO;
using System.Text;
using UnityEngine;

namespace ScavLib.input
{

    public static class ScavLibPaths
    {
        public static string UserDataRoot
            => Path.Combine(Application.persistentDataPath, "scavlib");

        public static string ModsRoot
            => Path.Combine(UserDataRoot, "mods");

        public static string ModDataDir(string owner)
        {
            string safe = SafeOwnerName(owner);
            if (safe == null) return null;
            return Path.Combine(ModsRoot, safe);
        }

        public static string KeybindsFile(string owner)
        {
            string dir = ModDataDir(owner);
            return dir == null ? null : Path.Combine(dir, "keybinds.json");
        }

        public static string EnsureModDir(string owner)
        {
            string dir = ModDataDir(owner);
            if (dir == null) return null;
            try { Directory.CreateDirectory(dir); }
            catch (System.Exception ex)
            {
                ScavLibPlugin.Log.LogError(
                    $"[ScavLibPaths] Failed to create mod dir for '{owner}': {ex.Message}");
                return null;
            }
            return dir;
        }

        public static string SafeOwnerName(string owner)
        {
            if (string.IsNullOrEmpty(owner)) return null;
            var sb = new StringBuilder(owner.Length);
            foreach (char c in owner)
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' || c == '-')
                    sb.Append(c);
                else if (c >= 'A' && c <= 'Z')
                    sb.Append((char)(c + 32));
                else
                    sb.Append('_');
            }
            string s = sb.ToString();
            return s.Length == 0 ? null : s;
        }

        public static string SafeLocalId(string localId)
        {
            return SafeOwnerName(localId);
        }
    }
}
