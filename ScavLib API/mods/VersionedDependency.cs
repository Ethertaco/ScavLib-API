using System;

namespace ScavLib.mods
{
    public struct VersionedDependency
    {
        public string Name { get; }
        public string MinVersion { get; }
        public string MaxVersion { get; }

        public VersionedDependency(string name, string minVersion = null, string maxVersion = null)
        {
            Name = name;
            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }

        public bool IsSatisfiedBy(string actualVersion)
        {
            if (string.IsNullOrEmpty(actualVersion)) return false;

            Version actual;
            if (!TryParseLoose(actualVersion, out actual))
            {
                ScavLibPlugin.Log.LogWarning(
                    $"[VersionedDependency] Cannot parse actual version " +
                    $"'{actualVersion}' for dependency '{Name}'. Skipping version range check.");
                return true;
            }

            if (!string.IsNullOrEmpty(MinVersion))
            {
                Version min;
                if (TryParseLoose(MinVersion, out min))
                {
                    if (actual < min) return false;
                }
                else
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[VersionedDependency] Cannot parse MinVersion " +
                        $"'{MinVersion}' for dependency '{Name}'. Skipping min check.");
                }
            }

            if (!string.IsNullOrEmpty(MaxVersion))
            {
                Version max;
                if (TryParseLoose(MaxVersion, out max))
                {
                    if (actual > max) return false;
                }
                else
                {
                    ScavLibPlugin.Log.LogWarning(
                        $"[VersionedDependency] Cannot parse MaxVersion " +
                        $"'{MaxVersion}' for dependency '{Name}'. Skipping max check.");
                }
            }

            return true;
        }

        private static bool TryParseLoose(string s, out Version v)
        {
            v = null;
            if (string.IsNullOrEmpty(s)) return false;

            int dashIdx = s.IndexOf('-');
            int plusIdx = s.IndexOf('+');
            int cutIdx = -1;
            if (dashIdx >= 0) cutIdx = dashIdx;
            if (plusIdx >= 0 && (cutIdx < 0 || plusIdx < cutIdx)) cutIdx = plusIdx;
            string core = cutIdx >= 0 ? s.Substring(0, cutIdx) : s;

            if (!core.Contains(".")) core += ".0";

            return Version.TryParse(core, out v);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(MinVersion) && string.IsNullOrEmpty(MaxVersion))
                return Name;
            if (string.IsNullOrEmpty(MaxVersion))
                return $"{Name} (>={MinVersion})";
            if (string.IsNullOrEmpty(MinVersion))
                return $"{Name} (<={MaxVersion})";
            return $"{Name} ({MinVersion}~{MaxVersion})";
        }
    }
}
