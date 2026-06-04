using System;

namespace ScavLib.mods
{
    public class ModInfo
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public string Author { get; }
        public VersionedDependency[] VersionedDependencies { get; }

        public string[] Dependencies
        {
            get
            {
                if (VersionedDependencies == null || VersionedDependencies.Length == 0)
                    return Array.Empty<string>();

                var result = new string[VersionedDependencies.Length];
                for (int i = 0; i < VersionedDependencies.Length; i++)
                    result[i] = VersionedDependencies[i].Name;
                return result;
            }
        }

        public ModInfo(string name, string version, string description, string author = "Unknown")
            : this(name, version, description, author, Array.Empty<VersionedDependency>())
        {
        }

        public ModInfo(string name, string version, string description, string author, string[] dependencies)
            : this(name, version, description, author, ToVersioned(dependencies))
        {
        }

        public ModInfo(string name, string version, string description, string author, VersionedDependency[] dependencies)
        {
            Name = name;
            Version = version;
            Description = description;
            Author = author ?? "Unknown";
            VersionedDependencies = dependencies ?? Array.Empty<VersionedDependency>();
        }

        private static VersionedDependency[] ToVersioned(string[] names)
        {
            if (names == null || names.Length == 0)
                return Array.Empty<VersionedDependency>();

            var result = new VersionedDependency[names.Length];
            for (int i = 0; i < names.Length; i++)
                result[i] = new VersionedDependency(names[i]);
            return result;
        }

        public override string ToString() => $"{Name} v{Version} by {Author}";
    }
}
